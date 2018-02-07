using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.WsFederation.EntityFramework.DbContexts;
using IdentityServer4.WsFederation.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Rsk.IdentityServer4.WsFederation.EntityFramework.Tests.DbContext
{
    public class WsFederationConfigurationDbContextTests : IDisposable
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly TheoryData<DbContextOptions<WsFederationConfigurationDbContext>> TestDatabaseProviders = new TheoryData<DbContextOptions<WsFederationConfigurationDbContext>>
        {
            new DbContextOptionsBuilder<WsFederationConfigurationDbContext>().UseInMemoryDatabase("WsFederationConfigurationDbContextTests").Options,
            new DbContextOptionsBuilder<WsFederationConfigurationDbContext>().UseSqlite("Filename=./WsFederationConfigurationDbContextTests.db").Options,
            new DbContextOptionsBuilder<WsFederationConfigurationDbContext>().UseSqlServer(@"Data Source=(LocalDb)\MSSQLLocalDB;database=Test.WsFederationConfigurationDbContextTests;trusted_connection=yes;").Options
        };

        public WsFederationConfigurationDbContextTests()
        {
            foreach (var options in TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<WsFederationConfigurationDbContext>)y)).ToList())
            {
                using (var ctx = new WsFederationConfigurationDbContext(options))
                    ctx.Database.EnsureCreated();
            }
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public async Task AddAsync_WhenRelyingPartyContainingBasicPropertiesAdded_ExpectSaveChangesSuccess(DbContextOptions<WsFederationConfigurationDbContext> options)
        {
            // arrange
            var entity = new RelyingParty
            {
                Realm = Guid.NewGuid().ToString(),
                TokenType = Guid.NewGuid().ToString(),
                DigestAlgorithm = Guid.NewGuid().ToString(),
                SignatureAlgorithm = Guid.NewGuid().ToString(),
                SamlNameIdentifierFormat = Guid.NewGuid().ToString()
            };

            // act
            using (var ctx = new WsFederationConfigurationDbContext(options))
            {
                await ctx.RelyingParties.AddAsync(entity);
                await ctx.SaveChangesAsync();
            }

            // assert
            RelyingParty foundEntity;
            using (var ctx = new WsFederationConfigurationDbContext(options))
            {
                foundEntity = ctx.RelyingParties.FirstOrDefault(x => x.Realm == entity.Realm);
            }

            foundEntity.Should().NotBeNull();
            foundEntity.Realm.Should().Be(entity.Realm);
            foundEntity.TokenType.Should().Be(entity.TokenType);
            foundEntity.DigestAlgorithm.Should().Be(entity.DigestAlgorithm);
            foundEntity.SignatureAlgorithm.Should().Be(entity.SignatureAlgorithm);
            foundEntity.SamlNameIdentifierFormat.Should().Be(entity.SamlNameIdentifierFormat);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public async Task AddAsync_WhenRelyingPartyHasNullEncryptionCert_ExpectEncryptionCertReturnedAsNull(DbContextOptions<WsFederationConfigurationDbContext> options)
        {
            // arrange
            var entity = new RelyingParty
            {
                Realm = Guid.NewGuid().ToString(),
                EncryptionCertificate = null
            };

            // act
            using (var ctx = new WsFederationConfigurationDbContext(options))
            {
                await ctx.RelyingParties.AddAsync(entity);
                await ctx.SaveChangesAsync();
            }

            // assert
            RelyingParty foundEntity;
            using (var ctx = new WsFederationConfigurationDbContext(options))
            {
                foundEntity = ctx.RelyingParties.FirstOrDefault(x => x.Realm == entity.Realm);
            }

            foundEntity.Should().NotBeNull();
            foundEntity.EncryptionCertificate.Should().BeNull();
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public async Task AddAsync_WhenRelyingPartyHasPublicEncryptionCert_ExpectEncryptionCertReturned(DbContextOptions<WsFederationConfigurationDbContext> options)
        {
            // arrange
            var certToStore = new X509Certificate2("Resources/idsrv3test.cer");
            var entity = new RelyingParty
            {
                Realm = Guid.NewGuid().ToString(),
                EncryptionCertificate = certToStore.GetRawCertData()
            };

            // act
            using (var ctx = new WsFederationConfigurationDbContext(options))
            {
                await ctx.RelyingParties.AddAsync(entity);
                await ctx.SaveChangesAsync();
            }

            // assert
            RelyingParty foundEntity;
            using (var ctx = new WsFederationConfigurationDbContext(options))
            {
                foundEntity = ctx.RelyingParties.FirstOrDefault(x => x.Realm == entity.Realm);
            }

            foundEntity.Should().NotBeNull();
            foundEntity.EncryptionCertificate.Should().NotBeNull();

            var parsedCert = new X509Certificate2(entity.EncryptionCertificate);
            parsedCert.Should().NotBeNull();
            parsedCert.Thumbprint.Should().Be(certToStore.Thumbprint);
            parsedCert.SubjectName.Name.Should().Be(certToStore.SubjectName.Name);
            parsedCert.HasPrivateKey.Should().Be(certToStore.HasPrivateKey);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public async Task AddAsync_WhenModelHasClaimsMappings_ExpectClaimsMappingsMappedCorrectly(DbContextOptions<WsFederationConfigurationDbContext> options)
        {
            // arrange
            var mappings = new List<WsFedClaimMap>
            {
                new WsFedClaimMap{OriginalClaimType = "sub", NewClaimType = ClaimTypes.NameIdentifier},
                new WsFedClaimMap{OriginalClaimType = "name", NewClaimType = ClaimTypes.Name}
            };

            var entity = new RelyingParty
            {
                Realm = Guid.NewGuid().ToString(),
                ClaimMapping = mappings
            };

            // act
            using (var ctx = new WsFederationConfigurationDbContext(options))
            {
                await ctx.RelyingParties.AddAsync(entity);
                await ctx.SaveChangesAsync();
            }

            // assert
            RelyingParty foundEntity;
            using (var ctx = new WsFederationConfigurationDbContext(options))
            {
                foundEntity = ctx.RelyingParties.Include(x => x.ClaimMapping).FirstOrDefault(x => x.Realm == entity.Realm);
            }

            foundEntity.ClaimMapping.Should().NotBeNull();
            foundEntity.ClaimMapping.Should().NotBeEmpty();
            foundEntity.ClaimMapping.Should().HaveCount(mappings.Count);

            foreach (var mappedMapping in foundEntity.ClaimMapping)
            {
                var matchingMapping = mappings.FirstOrDefault(x => x.OriginalClaimType == mappedMapping.OriginalClaimType);
                matchingMapping.Should().NotBeNull();
                mappedMapping.NewClaimType.Should().Be(matchingMapping.NewClaimType);
            }
        }

        public void Dispose()
        {
            foreach (var options in TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<WsFederationConfigurationDbContext>)y)).ToList())
            {
                using (var ctx = new WsFederationConfigurationDbContext(options))
                    ctx.Database.EnsureDeleted();
            }
        }
    }
}
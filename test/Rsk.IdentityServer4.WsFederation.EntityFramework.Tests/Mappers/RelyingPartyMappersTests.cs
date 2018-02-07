using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using IdentityServer4.WsFederation.EntityFramework.Entities;
using IdentityServer4.WsFederation.EntityFramework.Mappers;
using Xunit;
using RelyingParty = IdentityServer4.WsFederation.Stores.RelyingParty;

namespace Rsk.IdentityServer4.WsFederation.EntityFramework.Tests.Mappers
{
    public class RelyingPartyMappersTests
    {
        [Fact]
        public void ToEntity_WhenPropertiesSet_ExpectValuesMapped()
        {
            var model = new RelyingParty
            {
                Realm = Guid.NewGuid().ToString(),
                TokenType = Guid.NewGuid().ToString(),
                DigestAlgorithm = Guid.NewGuid().ToString(),
                SignatureAlgorithm = Guid.NewGuid().ToString(),
                SamlNameIdentifierFormat = Guid.NewGuid().ToString()
            };

            var entity = model.ToEntity();

            entity.Should().NotBeNull();
            entity.Realm.Should().Be(model.Realm);
            entity.TokenType.Should().Be(model.TokenType);
            entity.DigestAlgorithm.Should().Be(model.DigestAlgorithm);
            entity.SignatureAlgorithm.Should().Be(model.SignatureAlgorithm);
            entity.SamlNameIdentifierFormat.Should().Be(model.SamlNameIdentifierFormat);

            entity.EncryptionCertificate.Should().BeNull();
            entity.ClaimMapping.Should().BeEmpty();
        }

        [Fact]
        public void ToEntity_WhenModelHasEncryptionCertificate_ExpectCertificateMappedCorrectly()
        {
            var certToStore = new X509Certificate2("Resources/idsrv3test.cer");
            var model = new RelyingParty { EncryptionCertificate = certToStore };

            var entity = model.ToEntity();

            entity.EncryptionCertificate.Should().NotBeNull();

            var parsedCert = new X509Certificate2(entity.EncryptionCertificate);
            parsedCert.Should().NotBeNull();
            parsedCert.Thumbprint.Should().Be(certToStore.Thumbprint);
            parsedCert.SubjectName.Name.Should().Be(certToStore.SubjectName.Name);
            parsedCert.HasPrivateKey.Should().Be(certToStore.HasPrivateKey);
        }

        [Fact]
        public void ToEntity_WhenModelHasClaimsMappings_ExpectClaimsMappingsMappedCorrectly()
        {
            var mappings = new Dictionary<string, string>
            {
                {"sub", ClaimTypes.NameIdentifier},
                {"name", ClaimTypes.Name}
            };

            var model = new RelyingParty { ClaimMapping = mappings };
            
            var entity = model.ToEntity();

            entity.ClaimMapping.Should().NotBeNullOrEmpty();
            entity.ClaimMapping.Should().HaveSameCount(mappings);

            foreach (var mappedMapping in entity.ClaimMapping)
            {
                var matchingMapping = mappings.FirstOrDefault(x => x.Key == mappedMapping.OriginalClaimType);
                matchingMapping.Should().NotBeNull();
                mappedMapping.NewClaimType.Should().Be(matchingMapping.Value);
            }
        }

        [Fact]
        public void ToModel_WhenPropertiesSet_ExpectValuesMapped()
        {
            var entity = new global::IdentityServer4.WsFederation.EntityFramework.Entities.RelyingParty
            {
                Realm = Guid.NewGuid().ToString(),
                TokenType = Guid.NewGuid().ToString(),
                DigestAlgorithm = Guid.NewGuid().ToString(),
                SignatureAlgorithm = Guid.NewGuid().ToString(),
                SamlNameIdentifierFormat = Guid.NewGuid().ToString(),
            };

            var model = entity.ToModel();

            model.Should().NotBeNull();
            model.Realm.Should().Be(entity.Realm);
            model.TokenType.Should().Be(entity.TokenType);
            model.DigestAlgorithm.Should().Be(entity.DigestAlgorithm);
            model.SignatureAlgorithm.Should().Be(entity.SignatureAlgorithm);
            model.SamlNameIdentifierFormat.Should().Be(entity.SamlNameIdentifierFormat);

            model.EncryptionCertificate.Should().BeNull();
            model.ClaimMapping.Should().BeEmpty();
        }

        [Fact]
        public void ToModel_WhenModelHasEncryptionCertificate_ExpectCertificateMappedCorrectly()
        {
            var certToStore = new X509Certificate2("Resources/idsrv3test.cer");
            var entity = new global::IdentityServer4.WsFederation.EntityFramework.Entities.RelyingParty
            {
                EncryptionCertificate = certToStore.GetRawCertData()
            };

            var model = entity.ToModel();

            model.EncryptionCertificate.Should().NotBeNull();

            model.EncryptionCertificate.Should().NotBeNull();
            model.EncryptionCertificate.Thumbprint.Should().Be(certToStore.Thumbprint);
            model.EncryptionCertificate.SubjectName.Name.Should().Be(certToStore.SubjectName.Name);
            model.EncryptionCertificate.HasPrivateKey.Should().Be(certToStore.HasPrivateKey);
        }

        [Fact]
        public void ToModel_WhenModelHasClaimsMappings_ExpectClaimsMappingsMappedCorrectly()
        {
            var mappings = new List<WsFedClaimMap>
            {
                new WsFedClaimMap{OriginalClaimType = "sub", NewClaimType = ClaimTypes.NameIdentifier},
                new WsFedClaimMap{OriginalClaimType = "name", NewClaimType = ClaimTypes.Name}
            };

            var entity = new global::IdentityServer4.WsFederation.EntityFramework.Entities.RelyingParty { ClaimMapping = mappings };

            var model = entity.ToModel();

            model.ClaimMapping.Should().NotBeNull();
            model.ClaimMapping.Should().NotBeEmpty();
            model.ClaimMapping.Should().HaveCount(mappings.Count);

            foreach (var mappedMapping in model.ClaimMapping)
            {
                var matchingMapping = mappings.FirstOrDefault(x => x.OriginalClaimType == mappedMapping.Key);
                matchingMapping.Should().NotBeNull();
                mappedMapping.Value.Should().Be(matchingMapping.NewClaimType);
            }
        }
    }
}
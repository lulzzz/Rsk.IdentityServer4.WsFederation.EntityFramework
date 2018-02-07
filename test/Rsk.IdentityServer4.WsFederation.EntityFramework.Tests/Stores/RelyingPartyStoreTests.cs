using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.WsFederation.EntityFramework.DbContexts;
using IdentityServer4.WsFederation.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Rsk.IdentityServer4.WsFederation.EntityFramework.Stores;
using Xunit;

namespace Rsk.IdentityServer4.WsFederation.EntityFramework.Tests.Stores
{
    public class RelyingPartyStoreTests
    {
        private readonly WsFederationConfigurationDbContext inMemContext;
        private readonly RelyingPartyStore store;

        public RelyingPartyStoreTests()
        {
            inMemContext = new WsFederationConfigurationDbContext(new DbContextOptionsBuilder<WsFederationConfigurationDbContext>()
                .UseInMemoryDatabase(nameof(RelyingPartyStoreTests)).Options);
            store = new RelyingPartyStore(inMemContext);
        }

        [Fact]
        public async Task FindRelyingPartyByRealm_WhenEntityIdIsNull_ExpectArgumentExcpetion()
        {
            // act & assert
            await Assert.ThrowsAsync<ArgumentException>(() => store.FindRelyingPartyByRealm(null));
        }

        [Fact]
        public async Task FindRelyingPartyByRealm_WhenrelyingPartyDoesNotExist_ExpectNullReturned()
        {
            // act
            var relyingParty = await store.FindRelyingPartyByRealm(Guid.NewGuid().ToString());

            // assert
            relyingParty.Should().BeNull();
        }

        [Fact]
        public async Task FindRelyingPartyByRealm_WhenrelyingPartyExists_ExpectModelReturned()
        {
            // arrange 
            var entity = new RelyingParty {Realm = Guid.NewGuid().ToString()};
            inMemContext.Add(entity);
            inMemContext.SaveChanges();

            // act
            var relyingParty = await store.FindRelyingPartyByRealm(entity.Realm);

            // assert
            relyingParty.Should().NotBeNull();
            relyingParty.Realm.Should().Be(entity.Realm);
        }
    }
}
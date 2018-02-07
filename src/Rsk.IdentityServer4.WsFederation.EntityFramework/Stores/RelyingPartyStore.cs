using System;
using System.Threading.Tasks;
using IdentityServer4.WsFederation.EntityFramework.Interfaces;
using IdentityServer4.WsFederation.EntityFramework.Mappers;
using IdentityServer4.WsFederation.Stores;
using Microsoft.EntityFrameworkCore;

namespace Rsk.IdentityServer4.WsFederation.EntityFramework.Stores
{
    public class RelyingPartyStore : IRelyingPartyStore
    {
        private readonly IWsFederationConfigurationDbContext context;

        public RelyingPartyStore(IWsFederationConfigurationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<RelyingParty> FindRelyingPartyByRealm(string realm)
        {
            if (string.IsNullOrWhiteSpace(realm)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(realm));

            var relyingParty = await context.RelyingParties.FirstOrDefaultAsync(x => x.Realm == realm);

            return relyingParty?.ToModel();
        }
    }
}
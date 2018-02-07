using System.Threading.Tasks;
using IdentityServer4.WsFederation.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.WsFederation.EntityFramework.Extensions;
using IdentityServer4.WsFederation.EntityFramework.Interfaces;

namespace IdentityServer4.WsFederation.EntityFramework.DbContexts
{
    public class WsFederationConfigurationDbContext : DbContext, IWsFederationConfigurationDbContext
    {
        public WsFederationConfigurationDbContext(DbContextOptions<WsFederationConfigurationDbContext> options) : base(options)
        {
        }

        public DbSet<RelyingParty> RelyingParties { get; set; }
        public Task<int> SaceChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureRelyingPartyContext();
            base.OnModelCreating(modelBuilder);
        }
    }
}
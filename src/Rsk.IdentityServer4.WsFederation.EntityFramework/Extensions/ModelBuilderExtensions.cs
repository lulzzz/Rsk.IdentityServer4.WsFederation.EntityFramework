using IdentityServer4.WsFederation.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.WsFederation.EntityFramework.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureRelyingPartyContext(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RelyingParty>(relyingParty =>
            {
                relyingParty.ToTable("RelyingParties");
                relyingParty.HasKey(x => x.Id);

                relyingParty.Property(x => x.Realm).HasMaxLength(200).IsRequired();

                relyingParty.HasIndex(x => x.Realm).IsUnique();

                relyingParty.HasMany(x => x.ClaimMapping).WithOne(x => x.RelyingParty).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WsFedClaimMap>(claimMap =>
            {
                claimMap.ToTable("RelyingPartyClaimMappings");
                claimMap.HasKey(x => x.Id);

                claimMap.Property(x => x.NewClaimType).HasMaxLength(250).IsRequired();
                claimMap.Property(x => x.OriginalClaimType).HasMaxLength(250).IsRequired();
            });
        }
    }
}
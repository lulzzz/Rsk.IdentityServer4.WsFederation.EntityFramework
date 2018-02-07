using System;
using System.Threading.Tasks;
using IdentityServer4.WsFederation.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.WsFederation.EntityFramework.Interfaces
{
    public interface IWsFederationConfigurationDbContext : IDisposable
    {
        DbSet<RelyingParty> RelyingParties { get; set; }
        Task<int> SaceChangesAsync();
    }
}
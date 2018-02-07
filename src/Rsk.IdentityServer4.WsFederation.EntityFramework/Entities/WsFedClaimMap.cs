namespace IdentityServer4.WsFederation.EntityFramework.Entities
{
    public class WsFedClaimMap
    {
        public int Id { get; set; }
        public string OriginalClaimType { get; set; }
        public string NewClaimType { get; set; }
        public RelyingParty RelyingParty { get; set; }
    }
}
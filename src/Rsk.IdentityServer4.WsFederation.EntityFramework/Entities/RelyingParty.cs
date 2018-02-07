using System.Collections.Generic;

namespace IdentityServer4.WsFederation.EntityFramework.Entities
{
    public class RelyingParty
    {
        public int Id { get; set; }
        public string Realm { get; set; }
        public string TokenType { get; set; }
        public string DigestAlgorithm { get; set; }
        public string SignatureAlgorithm { get; set; }
        public string SamlNameIdentifierFormat { get; set; }
        public byte[] EncryptionCertificate { get; set; }

        public virtual List<WsFedClaimMap> ClaimMapping { get; set; }
    }
}
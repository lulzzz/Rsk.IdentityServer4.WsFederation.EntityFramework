using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using IdentityServer4.WsFederation.EntityFramework.Entities;

namespace IdentityServer4.WsFederation.EntityFramework.Mappers
{
    public class RelyingPartyMapperProfile : Profile
    {
        public RelyingPartyMapperProfile()
        {
            CreateMap<RelyingParty, Stores.RelyingParty>()
                .ForMember(x => x.EncryptionCertificate,
                    opt => opt.MapFrom(src =>
                        src.EncryptionCertificate != null && 0 < src.EncryptionCertificate.Length
                            ? new X509Certificate2(src.EncryptionCertificate)
                            : null))
                .ReverseMap()
                .ForMember(x => x.EncryptionCertificate,
                    opt => opt.MapFrom(src =>
                        src.EncryptionCertificate != null
                            ? src.EncryptionCertificate.GetRawCertData()
                            : null))
                .AfterMap((src, dest) => dest.EncryptionCertificate = 0 < dest.EncryptionCertificate?.Length
                    ? dest.EncryptionCertificate
                    : null);

            CreateMap<WsFedClaimMap, KeyValuePair<string, string>>()
                .ConstructUsing(src => new KeyValuePair<string, string>(src.OriginalClaimType, src.NewClaimType))
                .ReverseMap()
                .ConstructUsing(src => new WsFedClaimMap {OriginalClaimType = src.Key, NewClaimType = src.Value});
        }
    }
}
using AutoMapper;
using IdentityServer4.WsFederation.Stores;

namespace IdentityServer4.WsFederation.EntityFramework.Mappers
{
    public static class RelyingPartyMappers
    {
        static RelyingPartyMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<RelyingPartyMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static RelyingParty ToModel(this Entities.RelyingParty entity)
        {
            if (entity == null) return null;
            return Mapper.Map<RelyingParty>(entity);
        }

        public static Entities.RelyingParty ToEntity(this RelyingParty model)
        {
            if (model == null) return null;
            return Mapper.Map<Entities.RelyingParty>(model);
        }
    }
}
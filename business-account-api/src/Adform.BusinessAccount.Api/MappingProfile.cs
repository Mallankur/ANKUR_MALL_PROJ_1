using Mapster;

namespace Adform.BusinessAccount.Api
{
    public class MappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AspNetCore.Paging.Page, Contracts.Entities.Page>();
            config.NewConfig<AspNetCore.Paging.Order, Contracts.Entities.Order>();
        }
    }
}
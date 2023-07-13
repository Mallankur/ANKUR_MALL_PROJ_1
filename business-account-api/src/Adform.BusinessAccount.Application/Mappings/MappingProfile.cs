using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Contracts.Entities;
using Mapster;

namespace Adform.BusinessAccount.Application.Mappings
{
	public class MappingProfile : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<CreateSsoConfigurationInput, SsoConfigurationCreateCommand>();

           
            config.NewConfig<CreateBusinessAccountInput, BusinessAccountCreateCommand>();
			config.NewConfig<BusinessAccountCreateCommand, Domain.Repositories.Entities.BusinessAccount>()
				.Map(dest => dest.Name, src => src.Name.ToLower())
				.Map(dest => dest.Type, src => src.Type.ToString().ToLower());

			config.NewConfig<SsoConfigurationCreateCommand, Domain.Repositories.Entities.SsoConfiguration>()
				.Map(dest => dest.Name, src => src.Name.ToLower())
				.Map(dest => dest.Type, src => src.Type.ToString().ToLower())
				.Map(dest => dest.Domains, src => src.Domains.Select(x => x.ToLower()));

			config.NewConfig<Domain.Repositories.Entities.SsoConfiguration, SsoConfiguration>();
			config.NewConfig<Domain.Repositories.Entities.BusinessAccount, Contracts.Entities.BusinessAccount>();
		}
	}
}

using Adform.BusinessAccount.Application.Services;
using Adform.BusinessAccount.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Adform.BusinessAccount.Application;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
	{
		services.AddTransient<IBusinessAccountService, BusinessAccountService>();
		services.AddTransient<ISsoConfigurationService, SsoConfigurationService>();

		var config = TypeAdapterConfig.GlobalSettings;
		config.Scan(typeof(AssemblyReference).Assembly);
		var mapperConfig = new Mapper(config);
		services.AddSingleton<IMapper>(mapperConfig);

		return services;
	}
}
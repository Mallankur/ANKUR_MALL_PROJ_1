//using Adform.NetStandard.Aerospike;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Adform.BusinessAccount.Api.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfigurationRoot configuration)
        {
            //services.Configure<AerospikeSettings>(opt => configuration.GetSection("Aerospike").Bind(opt));
            //services.Configure<BusinessAccountApiCheckSettings>(opt => configuration.GetSection("BusinessAccountApiCheck").Bind(opt));

            return services;
        }
    }
}
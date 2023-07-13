using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Adform.BusinessAccount.Api.Capabilities
{
    public static class StartupConfig
    {
        public static IConfigurationRoot ConfigureConfig(this IHostEnvironment environment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();

            return builder
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}

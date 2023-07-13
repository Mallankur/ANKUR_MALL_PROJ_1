using System;
using Adform.BusinessAccount.Api.Swagger.Examples;
using Adform.Ciam.Swagger.Extensions;
using Adform.Ciam.Swagger.Options;
using Adform.Ciam.Swagger.Schemes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Adform.BusinessAccount.Api.Capabilities;

public static class StartupSwagger
{
    private const string MitLicenseUrl = "https://opensource.org/licenses/MIT";

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSwagger(options =>
            {
                options.RegisterDefaultDocs(new OpenApiInfo
                {
                    Title = configuration.GetValue<string>("Swagger:Title"),
                    Description = configuration.GetValue<string>("Swagger:Description"),
                    TermsOfService = new Uri("Shareware", UriKind.Relative),
                    License = new OpenApiLicense { Name = "MIT", Url = new Uri(MitLicenseUrl) },
                    Contact = new OpenApiContact
                    {
                        Name = configuration.GetValue<string>("Swagger:Contact:Name"),
                        Email = configuration.GetValue<string>("Swagger:Contact:Email")
                    }
                });

                options.AddAuth(new OAuth2ClientCredentialsScheme(
                    $"{configuration.GetValue<string>("OAuth:Authority")}/connect/authorize",
                    $"{configuration.GetValue<string>("OAuth:Authority")}/connect/token",
                    new[] { StartupOAuth.Scopes.Readonly, StartupOAuth.Scopes.Full }));
            })
            .AddSwaggerExamplesFromAssemblyOf<BusinessAccountResponseExample>();
    }

    public static IApplicationBuilder UseSwaggerUi(this IApplicationBuilder app,
        IApiVersionDescriptionProvider provider,
        IConfiguration configuration)
    {
        return app.UseSwaggerWithUi(options =>
        {
            options.SetDefaults();
            options.RegisterDefaultEndpoints(provider);
            options.AddAuth(new SwaggerOAuth2Options
            {
                ClientId = configuration.GetValue<string>("OAuth:ClientId"),
                ClientSecret = configuration.GetValue<string>("OAuth:ClientSecret")
            });
        }, setup => { setup.SerializeAsV2 = true; });
    }
}
﻿using Adform.AspNetCore.OAuth;
using Adform.Ciam.Authentication.Configuration;
using Adform.Ciam.Authentication.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Adform.BusinessAccount.Api.Capabilities
{
    public static class StartupOAuth
    {
        public static IServiceCollection ConfigureOAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var oauthConfig = configuration.GetSection("OAuth").Get<AuthConfiguration>();
            oauthConfig.Mode = new[] { AuthMode.ClientCredentials };
            services.ConfigureAuthentication(p =>
            {
                p.Authority = oauthConfig.Authority;
                p.Audience = oauthConfig.Audience;
                p.ClientId = oauthConfig.ClientId;
                p.ClientSecret = oauthConfig.ClientSecret;
                p.Scopes = oauthConfig.Scopes;
                p.Mode = oauthConfig.Mode;
                p.UseHttps = oauthConfig.UseHttps;
            });
            return services;
        }

        public static IMvcCoreBuilder ConfigureAuthorization(this IMvcCoreBuilder services)
        {
            return services.AddScopeAuthorization(Scopes.Full, Scopes.Readonly);
        }

        public static class Scopes
        {
            public const string Readonly = "https://api.adform.com/scope/businessaccounts.readonly";
            public const string Full = "https://api.adform.com/scope/businessaccounts";
        }
    }

}
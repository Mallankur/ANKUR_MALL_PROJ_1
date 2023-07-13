using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.Ciam.Authentication.Configuration;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;

namespace Adform.BusinessAccount.Acceptance.Test;

public abstract class AcceptanceTestsBase
{
    protected readonly HttpClient _client;
    private readonly HttpClient _oAuthClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly BusinessAccountWebApplicationFixture _factory;
    private static IMongoDatabase _mongodb = null;

    public AcceptanceTestsBase(BusinessAccountWebApplicationFixture fixture)
    {
        _factory = fixture;
        _client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _mongodb = _factory.Services.GetRequiredService<IMongoDatabase>();
        
        var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        var oauthConfig = configuration.GetSection("OAuth").Get<TokenAuthConfiguration>();
        _oAuthClient = new HttpClient
        {
            BaseAddress = new Uri(oauthConfig.TokenEndpointUri)
        };
        _clientId = oauthConfig.ClientId;
        _clientSecret = oauthConfig.ClientSecret;
    }

    protected async Task<KeyValuePair<HttpStatusCode, Contracts.Entities.SsoConfiguration>> CreateNewSsoConfigurationAsync()
    {
        var input = GetNewSsoConfigurationInput();

        var request = await CreateHttpRequestMessage(HttpMethod.Post,
	        $"v1.0/SsoConfiguration", ScopeType.Full,
	        (httpRequestMessage) =>
	        {
		        return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
	        });

        var response = await _client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        var ssoConfiguration = JsonConvert.DeserializeObject<SsoConfiguration>(responseString);
        return new KeyValuePair<HttpStatusCode, Contracts.Entities.SsoConfiguration>(response.StatusCode,
            ssoConfiguration);
    }

    protected CreateSsoConfigurationInput GetNewSsoConfigurationInput()
    {
        Guid guid = Guid.NewGuid();
        return new CreateSsoConfigurationInput
        {
            Type = SsoConfigurationType.Saml2,
            Name = $"Test Configuration {guid}",
            Domains = new[] { guid + "*.adform.com" },
            Saml2 = new Saml2
            {
                EntityID = "https://adform.com/Saml2"
            }
        };
    }

    protected async Task<string> GetTokenAsync(bool readonlyScope = true, bool authorized = true)
    {
        var scope = readonlyScope
            ? Api.Capabilities.StartupOAuth.Scopes.Readonly
            : Api.Capabilities.StartupOAuth.Scopes.Full;

        var tokenResponse = await _oAuthClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret,
            Scope = authorized ? scope : "https://api.adform.com/scope/scope.test"
        });

        if (tokenResponse.IsError)
        {
            throw new Exception(tokenResponse.Error);
        }

        return tokenResponse.AccessToken;
    }

    protected async Task<HttpRequestMessage> CreateHttpRequestMessage(HttpMethod httpMethod, 
        string requestUri, ScopeType scope = ScopeType.None, 
        Func < HttpRequestMessage, HttpContent> contentAction = null)
    {
        var request = new HttpRequestMessage(httpMethod, requestUri);
        request.Content = contentAction?.Invoke(request);

        if (scope == ScopeType.None)
        {
            return request;
        }
        var token = await GetTokenAsync(scope == ScopeType.Readonly);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

        return request;
    }

    protected async Task DropBusinessAccounts()
    {
        await _mongodb.DropCollectionAsync("BusinessAccounts");
	}

    protected async Task DropSsoConfiguration()
    {
	    await _mongodb.DropCollectionAsync("SsoConfigurations");
    }

    public class TokenAuthConfiguration
    {
        public string ClientId{ get; set; }
        public string ClientSecret{ get; set; }
        public string TokenEndpointUri { get; set; }
    }

    public enum ScopeType
    {
        None = 0,
        Readonly,
        Full
    }
}
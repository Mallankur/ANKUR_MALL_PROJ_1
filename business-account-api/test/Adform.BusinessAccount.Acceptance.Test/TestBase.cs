using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Adform.BusinessAccountApi.Acceptance.Test;

public abstract class TestBase
{
    protected readonly HttpClient Client;
    protected readonly IConfigurationRoot Config;
    private readonly HttpClient _oAuthClient;
    private readonly string _clientId;
    private readonly string _clientSecret;

    protected TestBase()
    {
        var configurationBuilder = new ConfigurationBuilder();

#if (DEBUG)
        configurationBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "testsettings.json"), false);
#elif (RELEASE)
            configurationBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "testsettings.Release.json"), false);
#endif


        Config = configurationBuilder.Build();
        var host = Config.GetValue<string>("host");

        Client = new HttpClient
        {
            BaseAddress = new Uri(host)
        };

        var oAuthHost = Config.GetValue<string>("oAuth2:tokenEndpointUri");
        _oAuthClient = new HttpClient
        {
            BaseAddress = new Uri(oAuthHost)
        };

        _clientId = Config.GetValue<string>("oAuth2:clientId");
        _clientSecret = Config.GetValue<string>("oAuth2:clientSecret");

    }

    protected async Task<HttpRequestMessage> CreateEvaluateAccountsRequestAsync(object content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"v1/authrestrictions/evaluate/accounts")
        {
            Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
        };

        var token = await GetTokenAsync();
        request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

        return request;
    }

    protected async Task<HttpRequestMessage> CreateGetAgencyAuthRestrictionsRequestAsync(int legacyId,
        bool authorized = true)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"v1/authrestrictions/agency-restriction/{legacyId}");

        var token = await GetTokenAsync(authorized: authorized);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

        return request;
    }

    /* protected async Task<HttpRequestMessage> CreateCreateAgencyAuthRestrictionsRequestAsync(CreateAgencyRestrictionsRequest r, bool authorized = true)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"v1/authrestrictions/agency-restriction")
        {
            Content = new StringContent(JsonConvert.SerializeObject(r), Encoding.UTF8, "application/json")
        };

        var token = await GetTokenAsync(readonlyScope: false, authorized: authorized);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

        return request;
    }

    protected async Task<HttpRequestMessage> CreateUpdateAgencyAuthRestrictionsRequestAsync(int agencyId, long timestamp, UpdateAgencyRestrictionsRequest r, bool authorized = true, bool weakETag = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"v1/authrestrictions/agency-restriction/{agencyId}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(r), Encoding.UTF8, "application/json")
        };

        var token = await GetTokenAsync(readonlyScope: false, authorized: authorized);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

        if (weakETag)
        {
            var etag = new EntityTagHeaderValue($"\"{timestamp}\"", true);
            request.Headers.Add("ETag", etag.ToString());
        }
        else
        {
            request.Headers.Add("ETag", timestamp.ToString());
        }

        return request;
    } */

    protected async Task<HttpRequestMessage> CreateDeleteAgencyAuthRestrictionsRequestAsync(int agencyId,
        long timestamp, bool authorized = true, bool weakETag = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"v1/authrestrictions/agency-restriction/{agencyId}");

        var token = await GetTokenAsync(readonlyScope: false, authorized: authorized);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

        if (weakETag)
        {
            var etag = new EntityTagHeaderValue($"\"{timestamp}\"", true);
            request.Headers.Add("ETag", etag.ToString());
        }
        else
        {
            request.Headers.Add("ETag", timestamp.ToString());
        }

        return request;
    }

    protected async Task<string> GetTokenAsync(bool readonlyScope = true, bool authorized = true)
    {
        var scope = readonlyScope
            ? "https://api.adform.com/scope/authrestrictions.readonly"
            : "https://api.adform.com/scope/authrestrictions";

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

    protected long GetTimestamp(HttpResponseMessage response)
    {
        throw new NotImplementedException();
        //return ETagHelper.FromWeakETagHeader(response.Headers.GetValues("ETag").Single());
    }
}
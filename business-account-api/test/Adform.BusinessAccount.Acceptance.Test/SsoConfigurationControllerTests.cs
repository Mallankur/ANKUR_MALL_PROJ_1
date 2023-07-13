using Adform.BusinessAccount.Contracts.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Adform.Ciam.ExceptionHandling.Abstractions.Contracts;
using Xunit;
using Adform.BusinessAccount.Domain.Exceptions;

namespace Adform.BusinessAccount.Acceptance.Test;

[Collection(BusinessAccountWebApplicationCollection.COLLECTION_NAME)]
public class SsoConfigurationControllerTests : AcceptanceTestsBase
{
    public SsoConfigurationControllerTests(BusinessAccountWebApplicationFixture fixture) : base(fixture)
    { }

	[Fact]
	public async Task Post_CreateSsoConfiguration_Returns201HttpStatusCode()
	{
		// Arrange
		var input = GetNewSsoConfigurationInput();

		var request = await CreateHttpRequestMessage(HttpMethod.Post, $"v1.0/ssoconfiguration", ScopeType.Full,
			(httpRequestMessage) =>
			{
				return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8,
					"application/json");
			});

		var response = await _client.SendAsync(request);
		var responseString = await response.Content.ReadAsStringAsync();
		var responseObject = JsonConvert.DeserializeObject<SsoConfiguration>(responseString);

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		Assert.True(response.Headers.Contains("Location"));
		Assert.True(response.Headers.Contains("CorrelationId"));

		Assert.NotNull(responseObject);
		Assert.Equal(input.Name.ToLower(), responseObject.Name);
		Assert.Equal(input.Type, responseObject.Type);
		Assert.True(responseObject.Domains.Length == 1);
		Assert.NotNull(responseObject.Saml2);
		Assert.Equal(input.Saml2.EntityID, responseObject.Saml2.EntityID);
	}

	[Fact]
	public async Task Post_CreateSsoConfiguration_Returns400HttpStatusCode()
	{
		// Arrange
		var input = new CreateSsoConfigurationInput
		{
			Type = SsoConfigurationType.Saml2,
			Name = "Test Configuration New",
			Domains = new[] { "*.adform.com" },
			Saml2 = new Saml2
			{
				EntityID = ""
			}
		};

		var request = await CreateHttpRequestMessage(HttpMethod.Post, $"v1.0/ssoconfiguration", ScopeType.Full,
			(httpRequestMessage) =>
			{
				return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8,
					"application/json");
			});

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.True(response.Headers.Contains("CorrelationId"));

		var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
		Assert.NotNull(failedResponse);
		Assert.True(failedResponse.Params.ContainsKey("entityID"));
		Assert.Equal(1, failedResponse.Params.Count);
		ErrorDto error = JsonConvert.DeserializeObject<ErrorDto>(failedResponse.Params["entityID"].ToString());
		Assert.NotNull(error);
		Assert.Equal(ErrorReasons.Invalid, error.Reason);
		Assert.Equal(Messages.InvalidEntityId, error.Message);
	}

	[Fact]
	public async Task Post_CreateSsoConfiguration_BlankName_Returns400HttpStatusCode()
	{
		// Arrange
		var input = GetNewSsoConfigurationInput();
		input.Name = "";

		var request = await CreateHttpRequestMessage(HttpMethod.Post, $"v1.0/ssoconfiguration", ScopeType.Full,
			(httpRequestMessage) =>
			{
				return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8,
					"application/json");
			});

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.True(response.Headers.Contains("CorrelationId"));

		var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
		Assert.NotNull(failedResponse);
		Assert.True(failedResponse.Params.ContainsKey("name"));
		Assert.Equal(1, failedResponse.Params.Count);
		ErrorDto error = JsonConvert.DeserializeObject<ErrorDto>(failedResponse.Params["name"].ToString());
		Assert.NotNull(error);
		Assert.Equal(ErrorReasons.Required, error.Reason);
		Assert.Equal(Messages.NameIsNullorEmpty, error.Message);
	}

	[Theory]
	[InlineData(ScopeType.None, HttpStatusCode.Found, true)]
	[InlineData(ScopeType.Readonly, HttpStatusCode.Forbidden, false)]
	public async Task Post_CreateSsoConfiguration_ScopeFail(ScopeType scopeType, HttpStatusCode statusCode, bool returnRedirectLocation)
	{
		// Arrange
		var input = new CreateSsoConfigurationInput
		{
			Type = SsoConfigurationType.Saml2,
			Name = "Test Configuration New",
			Domains = new[] { "*.adform.com" },
			Saml2 = new Saml2
			{
				EntityID = ""
			}
		};

		var request = await CreateHttpRequestMessage(HttpMethod.Post, $"v1.0/ssoconfiguration", scopeType,
			(httpRequestMessage) =>
			{
				return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8,
					"application/json");
			});

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(statusCode, response.StatusCode);
		Assert.True(response.Headers.Contains("CorrelationId"));

		if (returnRedirectLocation)
		{
			Assert.True(response.Headers.Contains("Location"));
			Assert.True(!string.IsNullOrWhiteSpace(response.Headers.Location.ToString()));
		}
	}

	[Theory]
	[InlineData("*.testdomain.com", HttpStatusCode.NotFound)]
	public async Task Get_SsoConfigurationByDomain_Fail(string domain, HttpStatusCode httpStatusCode)
	{
		var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();
		// Arrange
		var request = await CreateHttpRequestMessage(HttpMethod.Get, $"v1.0/ssoconfiguration/domain:" + domain, ScopeType.Readonly);
		//Act
		var response = await _client.SendAsync(request);
		// Assert
		Assert.Equal(httpStatusCode, response.StatusCode);
		if (httpStatusCode == HttpStatusCode.OK)
		{
			var ssoConfiguration = await response.Content.ReadAsAsync<SsoConfiguration>();
			Helper.AssertSsoConfiguration(ssoConfiguration, newSsoconfigurationDetails.Value);
		}
		else if(httpStatusCode == HttpStatusCode.NotFound)
		{
			var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
			Assert.Equal(ErrorReasons.NotFound, failedResponse.Reason);
			Assert.Equal(Messages.SsoConfigurationNotFound, failedResponse.Message);
		}
	}
	[Theory]
	//[InlineData(ScopeType.Full)]
	[InlineData(ScopeType.Readonly)]
	public async Task Get_SsoConfigurationByDomain_success(ScopeType scopeType)
	{
		var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();
		// Arrange
		var request = await CreateHttpRequestMessage(HttpMethod.Get, $"v1.0/ssoconfiguration/domain:" + newSsoconfigurationDetails.Value.Domains[0], scopeType);
		//Act
		var response = await _client.SendAsync(request);
		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var ssoConfiguration = await response.Content.ReadAsAsync<SsoConfiguration>();
		Helper.AssertSsoConfiguration(ssoConfiguration, newSsoconfigurationDetails.Value);
	}

	[Fact]
	public async Task Get_SsoConfigurationByName_Fail()
	{
		// Arrange
		var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();
		var request = await CreateHttpRequestMessage(HttpMethod.Get,
			$"v1.0/ssoconfiguration/name:test", ScopeType.Readonly);

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
		Assert.Equal(ErrorReasons.NotFound, failedResponse.Reason);
		Assert.Equal(Messages.SsoConfigurationNotFound, failedResponse.Message);
	}

	[Fact]
	public async Task Get_SsoConfigurationByName_success()
	{
		// Arrange
		var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();
		var request = await CreateHttpRequestMessage(HttpMethod.Get,
			$"v1.0/ssoconfiguration/name:" + newSsoconfigurationDetails.Value.Name, ScopeType.Readonly);

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var ssoConfiguration = await response.Content.ReadAsAsync<SsoConfiguration>();
		Helper.AssertSsoConfiguration(ssoConfiguration, newSsoconfigurationDetails.Value);
	}

	[Fact]
	public async Task Get_SsoConfigurationByType_SuccessNoRecord()
	{
		// Arrange
		var request = await CreateHttpRequestMessage(HttpMethod.Get,
			$"v1.0/ssoconfiguration?type=oidc", ScopeType.Readonly);
		request.Headers.Add("Return-Total-Count", "true");

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
		Assert.True(response.Headers.Contains("Offset"));
		Assert.True(response.Headers.Contains("Limit"));
		Assert.True(response.Headers.Contains("Total-Count"));
		Assert.True(int.Parse(response.Headers.Single(x => x.Key == "Offset").Value.Single()) == 0);
		Assert.True(int.Parse(response.Headers.Single(x => x.Key == "Limit").Value.Single()) == 100);
		Assert.True(int.Parse(response.Headers.Single(x => x.Key == "Total-Count").Value.Single()) == 0);
		var ssoConfigurations = await response.Content.ReadAsAsync<IEnumerable<SsoConfiguration>>();
		Assert.True(!ssoConfigurations.Any());
	}

	[Fact]
	public async Task Get_SsoConfigurationByType_success()
	{
		await DropSsoConfiguration();
		// Arrange
		var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();
		var request = await CreateHttpRequestMessage(HttpMethod.Get,
			$"v1.0/ssoconfiguration?type={newSsoconfigurationDetails.Value.Type}", ScopeType.Readonly);

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var ssoConfiguration = await response.Content.ReadAsAsync<IEnumerable<SsoConfiguration>>();
		Assert.True(ssoConfiguration.Count() > 0);

		Helper.AssertSsoConfiguration(newSsoconfigurationDetails.Value, ssoConfiguration.ElementAt(0));
	}
	   
	// Put 

    [Fact]
    public async Task Put_UpdateSsoConfiguration_Returns400HttpStatusCode()
    {
        // Arrange
        var input = new UpdateSsoConfigurationInput
        {
            Type = SsoConfigurationType.Saml2,
            Name = "Test Configuration New",
            Domains = new[] { "*.adform.com" },
            Saml2 = new Saml2
            {
                EntityID = ""
            }
        };

        var request = await CreateHttpRequestMessage(HttpMethod.Put, $"v1.0/ssoconfiguration", ScopeType.Full,
            (httpRequestMessage) =>
            {
                return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8,
                    "application/json");
            });

        //Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);  
        Assert.True(response.Headers.Contains("CorrelationId"));

        var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(failedResponse);
        Assert.True(failedResponse.Params.ContainsKey("entityID"));
        Assert.Equal(1, failedResponse.Params.Count);
        ErrorDto error = JsonConvert.DeserializeObject<ErrorDto>(failedResponse.Params["entityID"].ToString());
        Assert.NotNull(error);
        Assert.Equal(ErrorReasons.Invalid, error.Reason);
        Assert.Equal(Messages.InvalidEntityId, error.Message);
    }

    [Fact]
    public async Task Put_UpdateSsoConfiguration_IdNotExist_Returns400HttpStatusCode()
    {
        // Arrange
        await DropSsoConfiguration();
        // Arrange
        var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();

        var input = new UpdateSsoConfigurationInput
        {
            Type = newSsoconfigurationDetails.Value.Type,
            Name = "Test Configuration New",
            Domains = newSsoconfigurationDetails.Value.Domains,
            Saml2 = new Saml2
            {
                EntityID = "http://localhost:8080/realms/Shraddha",
                MetadataLocation="http://localhost:8080/realms/Shraddha/protocol/saml/descriptor"
            }
        };

        var request = await CreateHttpRequestMessage(HttpMethod.Put, $"v1.0/ssoconfiguration?id={Guid.NewGuid()}", ScopeType.Full,
            (httpRequestMessage) =>
            {
                return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8,
                    "application/json");
            });

        //Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(response.Headers.Contains("CorrelationId"));

        var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(failedResponse);
        Assert.True(failedResponse.Params.ContainsKey("id"));
        ErrorDto error = JsonConvert.DeserializeObject<ErrorDto>(failedResponse.Params["id"].ToString());
        Assert.NotNull(error);
        Assert.Equal(ErrorReasons.Invalid, error.Reason);
        Assert.Equal(Messages.SsoConfigurationNotExist, error.Message);
    }

    [Fact]
    public async Task Put_UpdateSsoConfiguration_DuplicateDomainName_Returns400HttpStatusCode()
    {
        // Arrange
        await DropSsoConfiguration();
        // Arrange
        var newSsoconfigurationDetails1 = await CreateNewSsoConfigurationAsync();
        var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();

        var input = new UpdateSsoConfigurationInput
        {
            Type = newSsoconfigurationDetails.Value.Type,
            Name = "Test Configuration New",
            Domains = newSsoconfigurationDetails1.Value.Domains,
            Saml2 = new Saml2
            {
                EntityID = "http://localhost:8080/realms/Shraddha",
                MetadataLocation = "http://localhost:8080/realms/Shraddha/protocol/saml/descriptor"
            }
        };

        var request = await CreateHttpRequestMessage(HttpMethod.Put, $"v1.0/ssoconfiguration?id={newSsoconfigurationDetails.Value.Id}", ScopeType.Full,
            (httpRequestMessage) =>
            {
                return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8,
                    "application/json");
            });

        //Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(response.Headers.Contains("CorrelationId"));

        var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(failedResponse);
        Assert.True(failedResponse.Params.ContainsKey("domains"));
        ErrorDto error = JsonConvert.DeserializeObject<ErrorDto>(failedResponse.Params["domains"].ToString());
        Assert.NotNull(error);
        Assert.Equal(ErrorReasons.Invalid, error.Reason);
        Assert.Equal(Messages.DomainExist, error.Message);
    }

    [Fact]
    public async Task Put_UpdateSsoConfiguration_Success_Returns200HttpStatusCode()
    {
        // Arrange
        await DropSsoConfiguration();
        // Arrange
      
        var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();

        var input = new UpdateSsoConfigurationInput
        {
            Type = newSsoconfigurationDetails.Value.Type,
            Name = "Test Configuration New",
            Domains = newSsoconfigurationDetails.Value.Domains,
            Saml2 = new Saml2
            {
                EntityID = "http://localhost:8080/realms/Shraddha",
                MetadataLocation = "http://localhost:8080/realms/Shraddha/protocol/saml/descriptor"
            }
        };

        var request = await CreateHttpRequestMessage(HttpMethod.Put, $"v1.0/ssoconfiguration?id={newSsoconfigurationDetails.Value.Id}", ScopeType.Full,
            (httpRequestMessage) =>
            {
                return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8,
                    "application/json");
            });

        // Assert
        var response = await _client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<SsoConfiguration>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(responseObject);
        Assert.Equal(input.Type, responseObject.Type);
        Assert.True(responseObject.Domains.Length == 1);
        Assert.NotNull(responseObject.Saml2);
        Assert.Equal(input.Saml2.EntityID, responseObject.Saml2.EntityID);
    }
}
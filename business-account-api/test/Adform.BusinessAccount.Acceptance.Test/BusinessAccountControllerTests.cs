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
public class BusinessAccountApiTest : AcceptanceTestsBase
{
	public BusinessAccountApiTest(BusinessAccountWebApplicationFixture fixture) : base(fixture)
	{ }

    [Fact]
	public async Task Post_CreateBusinessAccount_Returns201HttpStatusCode()
	{
		var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();
		var businessAcctInput = new CreateBusinessAccountInput
		{
			Type = BusinessAccountType.Agency,
			Name = "Test Agency",
			IsActive = true,
			SsoConfigurationId = newSsoconfigurationDetails.Value.Id
		};

        var request = await CreateHttpRequestMessage(HttpMethod.Post,
            $"v1.0/businessaccount", ScopeType.Full,
            (httpRequestMessage) =>
            {
                return new StringContent(JsonConvert.SerializeObject(businessAcctInput), Encoding.UTF8, "application/json");
            });

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		var responseString = await response.Content.ReadAsStringAsync();
		var responseObject = JsonConvert.DeserializeObject<Contracts.Entities.BusinessAccount>(responseString);
		
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		Assert.True(response.Headers.Contains("Location"));
		Assert.True(response.Headers.Contains("CorrelationId"));

		Assert.NotNull(responseObject);
		Assert.Equal(businessAcctInput.Name.ToLower(), responseObject.Name);
		Assert.Equal(businessAcctInput.Type, responseObject.Type);
		Assert.Equal(businessAcctInput.IsActive, responseObject.IsActive);
		Assert.NotNull(responseObject.SsoConfiguration);
		Assert.Equal(businessAcctInput.SsoConfigurationId, responseObject.SsoConfiguration.Id);
	}

	[Theory]
	[InlineData(ScopeType.None, HttpStatusCode.Found, true)]
	[InlineData(ScopeType.Readonly, HttpStatusCode.Forbidden, false)]
	public async Task Post_CreateBusinessAccount_ScopeFail(ScopeType scopeType, HttpStatusCode statusCode, bool returnRedirectLocation)
	{
		// Arrange
		var input = new CreateBusinessAccountInput
		{
			Type = BusinessAccountType.Agency,
			Name = "Test Agency1",
			IsActive = true,
			SsoConfigurationId = Guid.NewGuid()
		};
		var request = await CreateHttpRequestMessage(HttpMethod.Post,
			$"v1.0/businessaccount", scopeType,
			(httpRequestMessage) =>
			{
				return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
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

	[Fact]
	public async Task Post_CreateBusinessAccount_Returns400HttpStatusCode()
	{
		// Arrange
		var guid = Guid.NewGuid();
		var input = new CreateBusinessAccountInput
		{
			Type = BusinessAccountType.Agency,
			Name = "Test Agency1",
			IsActive = true,
			SsoConfigurationId = guid
		};
		var request = await CreateHttpRequestMessage(HttpMethod.Post,
			$"v1.0/businessaccount", ScopeType.Full,
			(httpRequestMessage) =>
			{
				return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
			});

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.True(response.Headers.Contains("CorrelationId"));

		var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
		Assert.NotNull(failedResponse);
		Assert.Equal(1, failedResponse.Params.Count);
		Assert.True(failedResponse.Params.ContainsKey("ssoConfigurationId"));
		ErrorDto error = JsonConvert.DeserializeObject<ErrorDto>(failedResponse.Params["ssoConfigurationId"].ToString());
		Assert.NotNull(error);
		Assert.Equal(ErrorReasons.Invalid, error.Reason);
		Assert.Equal(Messages.SsoConfigurationNotExist,error.Message);
	}

    [Fact]
	public async Task Post_CreateBusinessAccount_BlankName_Returns400HttpStatusCode()
	{
		// Arrange
		var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();
		var businessAcctInput = new CreateBusinessAccountInput
		{
			Type = BusinessAccountType.Agency,
			Name = "",
			IsActive = true,
			SsoConfigurationId = newSsoconfigurationDetails.Value.Id
		};
		var request = await CreateHttpRequestMessage(HttpMethod.Post,
			$"v1.0/businessaccount", ScopeType.Full,
			(httpRequestMessage) =>
			{
				return new StringContent(JsonConvert.SerializeObject(businessAcctInput), Encoding.UTF8, "application/json");
			});

		//Act
		var response = await _client.SendAsync(request);

		// Assert
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.True(response.Headers.Contains("CorrelationId"));

		var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
		Assert.NotNull(failedResponse);
		Assert.Equal(1, failedResponse.Params.Count);
		Assert.True(failedResponse.Params.ContainsKey("name"));
		ErrorDto error = JsonConvert.DeserializeObject<ErrorDto>(failedResponse.Params["name"].ToString());
		Assert.NotNull(error);
		Assert.Equal(ErrorReasons.Required, error.Reason);
		Assert.Equal(Messages.NameIsNullorEmpty, error.Message);
	}

	[Theory]
	[InlineData("aaad6cf8-b706-4eca-894c-3ea09b88bad5", HttpStatusCode.NotFound)]
	[InlineData("caad6cf8-b706-4eca-894c-3ea09b88bad5", HttpStatusCode.NotFound)]
	public async Task Get_BusinessAccountById_Fail(string strguid, HttpStatusCode httpStatusCode)
	{
		// Arrange
		var request = await CreateHttpRequestMessage(HttpMethod.Get,$"v1.0/businessaccount/" + strguid, ScopeType.Readonly);
		//Act
		var response = await _client.SendAsync(request);
		// Assert
		Assert.Equal(httpStatusCode, response.StatusCode);
		var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
		Assert.Equal(ErrorReasons.NotFound, failedResponse.Reason);
		Assert.Equal(Messages.BusinessAccountNotFound, failedResponse.Message);
	}

	[Theory]
	//[InlineData(ScopeType.Full)]
	[InlineData(ScopeType.Readonly)]

	public async Task Get_BusinessAccountById_Success(ScopeType scopeType)
	{
		// Arrange
		var newAccountDetails = await CreateNewBusinessAccountAsync();
		var request = await CreateHttpRequestMessage(HttpMethod.Get, $"v1.0/businessaccount/" + newAccountDetails.Value.Id, scopeType);
		//Act
		var response = await _client.SendAsync(request);
		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var businessAccount = await response.Content.ReadAsAsync<BusinessAccount.Contracts.Entities.BusinessAccount>();
		Helper.AssertBusinessAccount(businessAccount, newAccountDetails.Value);
	}

	

    [Fact]
    public async Task Delete_DeleteBusinessAccount_Returns404()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        long version = 0;
        var request = await CreateHttpRequestMessage(HttpMethod.Delete,
            $"v1.0/businessaccount/{accountId}/{version}", ScopeType.Full);
        //Act
		var response = await _client.SendAsync(request);
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		var failedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
		Assert.Equal(ErrorReasons.NotFound, failedResponse.Reason);
		Assert.Equal(Messages.BusinessAccountNotFound, failedResponse.Message);
	}

    [Fact]
    public async Task Delete_DeleteExistingBusinessAccount_Returns204()
    {
		// Arrange
		var newAccountDetails = await CreateNewBusinessAccountAsync();
		//Act
		var accountId = newAccountDetails.Value.Id;
		long version = 0;
        var request = await CreateHttpRequestMessage(HttpMethod.Delete,
            $"v1.0/businessaccount/{accountId}/{version}", ScopeType.Full);
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		var getRequest = await CreateHttpRequestMessage(HttpMethod.Get, $"v1.0/businessaccount/" + accountId, ScopeType.Readonly);

	    //Act
        var getResponse = await _client.SendAsync(getRequest);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
		var failedResponse = await getResponse.Content.ReadFromJsonAsync<ErrorResponse>();
		Assert.Equal(ErrorReasons.NotFound, failedResponse.Reason);
		Assert.Equal(Messages.BusinessAccountNotFound, failedResponse.Message);
	}

	[Fact]
	public async Task Get_BusinessAccountGetAllAsync_Success()
	{
		// Arrange
		await DropBusinessAccounts();
		var newAccountDetails = await CreateNewBusinessAccountAsync();
		var request = await CreateHttpRequestMessage(HttpMethod.Get, $"v1.0/businessaccount", ScopeType.Readonly);
		request.Headers.Add("Return-Total-Count", "true");

		//Act
		var response = await _client.SendAsync(request);
		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var businessAccounts = await response.Content.ReadAsAsync<IEnumerable<BusinessAccount.Contracts.Entities.BusinessAccount>>();
		Assert.True(businessAccounts.Count() == 1);

		Assert.True(response.Headers.Contains("Offset"));
		Assert.True(response.Headers.Contains("Limit"));
		Assert.True(response.Headers.Contains("Total-Count"));
		Assert.True(int.Parse(response.Headers.Single(x => x.Key == "Offset").Value.Single()) == 0);
		Assert.True(int.Parse(response.Headers.Single(x => x.Key == "Limit").Value.Single()) == 100);
		Assert.True(int.Parse(response.Headers.Single(x => x.Key == "Total-Count").Value.Single()) == 1 );

		Helper.AssertBusinessAccount(businessAccounts.First(), newAccountDetails.Value);
	}

	[Fact]
	public async Task Get_BusinessAccountGetAllAsync_SuccessNoRecord()
	{
		// Arrange
		await DropBusinessAccounts();
		var request = await CreateHttpRequestMessage(HttpMethod.Get, $"v1.0/businessaccount", ScopeType.Readonly);
		request.Headers.Add("Return-Total-Count", "true");
		//Act
		var response = await _client.SendAsync(request);
		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode); Assert.True(response.Headers.Contains("Offset"));
		Assert.True(response.Headers.Contains("Limit"));
		Assert.True(response.Headers.Contains("Total-Count"));
		Assert.True(int.Parse(response.Headers.Single(x => x.Key == "Offset").Value.Single()) == 0);
		Assert.True(int.Parse(response.Headers.Single(x => x.Key == "Limit").Value.Single()) == 100);
		Assert.True(int.Parse(response.Headers.Single(x => x.Key == "Total-Count").Value.Single()) == 0);
		var businessAccounts = await response.Content.ReadAsAsync<IEnumerable<BusinessAccount.Contracts.Entities.BusinessAccount>>();
		Assert.True(!businessAccounts.Any());
	}

	private async Task<KeyValuePair<HttpStatusCode, Contracts.Entities.BusinessAccount>> CreateNewBusinessAccountAsync()
    {
		var newSsoconfigurationDetails = await CreateNewSsoConfigurationAsync();

		var input = new CreateBusinessAccountInput
        {
            Type = BusinessAccountType.Agency,
            Name = $"Test Agency {Guid.NewGuid()}",
            IsActive = true,
			SsoConfigurationId=newSsoconfigurationDetails.Value.Id
        };

		var request = await CreateHttpRequestMessage(
			HttpMethod.Post, $"v1.0/businessaccount",
			ScopeType.Full,
			(httpRequestMessage) =>
			{
				return new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
			});

        var response = await _client.SendAsync(request);
        var newBusinessAccount = await response.Content.ReadAsStringAsync();
        var businessAccount = JsonConvert.DeserializeObject<Contracts.Entities.BusinessAccount>(newBusinessAccount);

        return new KeyValuePair<HttpStatusCode, Contracts.Entities.BusinessAccount>(response.StatusCode,
            businessAccount);
    }
}
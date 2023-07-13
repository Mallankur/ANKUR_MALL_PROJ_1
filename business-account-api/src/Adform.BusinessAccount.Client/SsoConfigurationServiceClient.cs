using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Adform.BusinessAccount.Contracts;
using Adform.BusinessAccount.Contracts.Entities;
using Microsoft.AspNetCore.WebUtilities;

namespace Adform.BusinessAccount.Client
{
    public class SsoConfigurationServiceClient : ApiClientBase<SsoConfigurationServiceClient>, ISsoConfigurationService
	{
		public SsoConfigurationServiceClient(HttpClient client)
			: base(client)
		{

		}

		public async Task<SsoConfiguration> GetByDomainNameAsync(string domainName,
			CancellationToken cancellationToken = default)
		{
			var request = await CreateHttpRequestMessageAsync(HttpMethod.Get,
				$"/v1/ssoconfiguration/domain:{Uri.EscapeDataString(domainName)}",
				"/v1/ssoconfiguration/domain:{domainName}").ConfigureAwait(false);

			var response = await Instance.SendAsync(request, cancellationToken).ConfigureAwait(false);
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}

			if (!response.IsSuccessStatusCode)
			{
				var error = await ParseErrorAsync(response, cancellationToken);
				var errorString =
					$"Failed to get Sso Configuration by domain {domainName}. Error: {JsonSerializer.Serialize(error)}";
				//_logger.LogError(errorString);
				throw new Exception(errorString);
			}

			return await ReadAsJsonAsync<SsoConfiguration>(response.Content); ;
		}

		public async Task<SsoConfiguration> CreateAsync(CreateSsoConfigurationInput configuration,
			CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public async Task<SsoConfiguration> GetByNameAsync(string name, CancellationToken cancellationToken = default)
		{
			var request = await CreateHttpRequestMessageAsync(HttpMethod.Get,
				$"/v1/ssoconfiguration/name:{Uri.EscapeDataString(name)}",
				"/v1/ssoconfiguration/name:{name}").ConfigureAwait(false);

			var response = await Instance.SendAsync(request, cancellationToken).ConfigureAwait(false);
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}

			if (!response.IsSuccessStatusCode)
			{
				var error = await ParseErrorAsync(response, cancellationToken);
				var errorString =
					$"Failed to get Sso Configuration by name {name}. Error: {JsonSerializer.Serialize(error)}";
				//_logger.LogError(errorString);
				throw new Exception(errorString);
			}

			return await ReadAsJsonAsync<SsoConfiguration>(response.Content);
		}

        public async Task<PagedList<SsoConfiguration>> GetAsync(SsoConfigurationType? type, Page page, Order order, CancellationToken cancellationToken = default)
        {
			Dictionary<string, string> queryParams = new Dictionary<string, string>();
			queryParams.Add("limit", page.Limit.ToString());
			queryParams.Add("offset", page.Offset.ToString());
			queryParams.Add("orderBy", order.OrderBy);
			queryParams.Add("orderDirection", order.OrderDirection.ToString());
			if (type != null)
			{
				queryParams.Add("type", type.ToString());
			}
			var url =QueryHelpers.AddQueryString("/v1/ssoconfiguration",queryParams);
			var request = await CreateHttpRequestMessageAsync(HttpMethod.Get,
				url,"/v1/ssoconfiguration").ConfigureAwait(false);

			var response = await Instance.SendAsync(request, cancellationToken).ConfigureAwait(false);
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}

			if (!response.IsSuccessStatusCode)
			{
				var error = await ParseErrorAsync(response, cancellationToken);
				var errorString =
					$"Failed to get Sso Configuration by type {type}. Error: {JsonSerializer.Serialize(error)}";
				throw new Exception(errorString);
			}

			return await ReadAsJsonAsync<PagedList<SsoConfiguration>>(response.Content);
		}

		public Task<SsoConfiguration> UpdateAsync(Guid Id, UpdateSsoConfigurationInput configuration, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
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
    public class BusinessAccountServiceClient : ApiClientBase<BusinessAccountServiceClient>, IBusinessAccountService
    {
        public BusinessAccountServiceClient(HttpClient client)
            : base(client)
        {

        }

        public async Task<Contracts.Entities.BusinessAccount> GetByIdAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var request = await CreateHttpRequestMessageAsync(HttpMethod.Get,
                $"/v1/businessaccount/{Uri.EscapeDataString(id.ToString())}",
                "/v1/businessaccount/{id}").ConfigureAwait(false);

            var response = await Instance.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await ParseErrorAsync(response, cancellationToken);
                //_logger.LogError($"Failed to Business Account {id}. Error: {JsonConvert.SerializeObject(error)}");
                throw new Exception(
                    $"Business Account retrieval failed for Id: {id}. Error: {JsonSerializer.Serialize(error)}");
            }

            return await ReadAsJsonAsync<Contracts.Entities.BusinessAccount>(response.Content);
        }

        public async Task<PagedList<Contracts.Entities.BusinessAccount>> GetAsync(Page page, Order order,
            CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("limit", page.Limit.ToString());
            queryParams.Add("offset", page.Offset.ToString());
            queryParams.Add("orderBy", order.OrderBy);
            queryParams.Add("orderDirection", order.OrderDirection.ToString());
            var url = QueryHelpers.AddQueryString("/v1/businessaccount", queryParams);
            var request = await CreateHttpRequestMessageAsync(HttpMethod.Get,
                url,"/v1/businessaccount").ConfigureAwait(false);

            var response = await Instance.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await ParseErrorAsync(response, cancellationToken);
                var errorString =
                    $"Failed to Get Business Accounts {url}. Error: {JsonSerializer.Serialize(error)}";
                //_logger.LogError(errorString);
                throw new Exception(errorString);
            }

            return await ReadAsJsonAsync<PagedList<Contracts.Entities.BusinessAccount>>(response.Content);
        }

        public async Task<Contracts.Entities.BusinessAccount> CreateAsync(CreateBusinessAccountInput input,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid?> DeleteAsync(DeleteBusinessAccountInput input,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Client
{
    public abstract class ApiClientBase<T>
    {
        private const string MetricPath = "metric-path";
        private const string ThrottlingHeaderName = "THROTTLING-CALLER-TYPE";
        protected readonly JsonSerializerOptions _serializerOptions;

        protected ApiClientBase(HttpClient client)
        {
            Instance = client;
            _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _serializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        protected HttpClient Instance { get; }

        protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync(HttpMethod method, string path, 
            string metricName, object content = null,
            string throttlingHeader = null, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(method, path);
            string stringContent = JsonSerializer.Serialize(content, _serializerOptions);
            if (content != null)
            {
	            request.Content = new StringContent(stringContent);
            }

            request.Properties.Add(MetricPath, metricName);

            if (!string.IsNullOrEmpty(throttlingHeader))
            {
                request.Headers.Add(ThrottlingHeaderName, throttlingHeader);
            }

            return Task.FromResult(request);
        }

        protected async Task<ErrorDto> ParseErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return await JsonSerializer.DeserializeAsync<ErrorDto>(await response.Content.ReadAsStreamAsync(), _serializerOptions, cancellationToken).ConfigureAwait(false);
        }

        protected async Task<I> ReadAsJsonAsync<I>(HttpContent content) where I : class
        {
	        return await JsonSerializer
		        .DeserializeAsync<I>(await content.ReadAsStreamAsync(), _serializerOptions)
		        .ConfigureAwait(false);
        }

        /* protected ApiClientBase(string uri,
            string clientId,
            string secret, string[] scopes,
            string tokenEndpointUri,
            ICorrelationIdProvider correlationIdProvider,
            IStatsSender statsSender)
        {
            var options = new HttpClientOptions()
            {
                UseAccessTokenClient = new DefaultIdSrvTokenClient(clientId, secret, scopes, tokenEndpointUri),
                BaseAddress = new Uri(uri),
                StatsSender = statsSender,
                InnerHandler = new OutgoingCorrelationIdHandler(new HttpClientHandler(), correlationIdProvider)
            };


            Instance = HttpClientBuilder.Build(options);

            Instance.DefaultRequestHeaders.Accept.Clear();
            Instance.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        } 
        
        protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync(HttpMethod httpMethod,
            string requestUri, string metricName,
            Func<HttpRequestMessage, HttpContent> contentAction = null,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(httpMethod, requestUri);
            request.Properties.Add(MetricPath, metricName);

            //request.Headers.Add(Constants.IncludeLegacyFieldsHeaderName, "true");
            request.Content = contentAction?.Invoke(request);

            ////ScopeType scope = ScopeType.None,
            //if (scope == ScopeType.None)
            //{
            //    return request;
            //}
            //var token = await GetTokenAsync(scope == ScopeType.Readonly);
            //request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}"); 

            return Task.FromResult(request);
        } 
         */

    }
}

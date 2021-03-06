namespace GuaranteedRate.Net.Http.HttpService
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;

    public class HttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient _client;
        public readonly string BaseUrl;
        protected readonly NameValueCollection DefaultRequestHeaders;
        private readonly JsonMediaTypeFormatter _formatter;

        public HttpClient(string baseUrl = null, NameValueCollection defaultRequestHeaders = null, JsonMediaTypeFormatter formatter = null)
        {
            DefaultRequestHeaders = defaultRequestHeaders == null
                ? new NameValueCollection()
                : new NameValueCollection(defaultRequestHeaders);

            if (
                DefaultRequestHeaders.AllKeys.All(
                    key => !key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase)))
                DefaultRequestHeaders.Add("Content-Type", "application/json");
            _client = new System.Net.Http.HttpClient();
            BaseUrl = baseUrl ?? string.Empty;
            _formatter = formatter ?? new JsonMediaTypeFormatter();
        }

        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();
        }

        public Task<HttpResponseMessage> GetAsync(string url, NameValueCollection headers = null)
        {
            var request = CreateNewRequest(HttpMethod.Get, url, headers);
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> PostAsync<T>(string url, T body, NameValueCollection headers = null)
        {
            var request = CreateNewRequest(HttpMethod.Post, url, headers, body);
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> PutAsync<T>(string url, T body, NameValueCollection headers = null)
        {
            var request = CreateNewRequest(HttpMethod.Put, url, headers, body);
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> DeleteAsync(string url, NameValueCollection headers = null)
        {
            var request = CreateNewRequest(HttpMethod.Delete, url, headers);
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _client.SendAsync(request);
        }

        private HttpRequestMessage CreateNewRequest(HttpMethod httpMethod, string url, NameValueCollection headers)
        {
            url = url ?? string.Empty;
            var request = new HttpRequestMessage {Method = httpMethod, RequestUri = new Uri(BaseUrl + url)};
            return request.AppendHeaders(DefaultRequestHeaders, headers);
        }

        private HttpRequestMessage CreateNewRequest<T>(HttpMethod httpMethod, string url, NameValueCollection headers,
            T body)
        {
            var request = CreateNewRequest(httpMethod, url, headers);
            if (body != null)
                request.Content = new ObjectContent<T>(body, _formatter);
            return request;
        }
    }
}
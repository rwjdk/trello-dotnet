using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Model;

namespace TrelloDotNet.Control
{
    internal class ApiRequestController
    {
        private const string BaseUrl = "https://api.trello.com/1/";
        private const string NonApiBaseUrl = "https://trello.com/1/";
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _token;
        private readonly ITrelloOAuth2TokenProvider _oauth2TokenProvider;
        private readonly TrelloClient _client;

        internal HttpClient HttpClient => _httpClient;

        internal ApiRequestController(HttpClient httpClient, string apiKey, string token, TrelloClient client)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiKey = apiKey;
            _token = token;
            _client = client;
        }

        internal ApiRequestController(HttpClient httpClient, ITrelloOAuth2TokenProvider oauth2TokenProvider, TrelloClient client)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _oauth2TokenProvider = oauth2TokenProvider;
            _client = client;
        }

        public string Token => _token;

        public string ApiKey => _apiKey;

        internal bool UsesOAuth2 => _oauth2TokenProvider != null;

        internal async Task<T> Get<T>(string suffix, CancellationToken cancellationToken, params QueryParameter[] parameters)
        {
            string json = await Get(suffix, cancellationToken, 0, parameters);
            T @object = JsonSerializer.Deserialize<T>(json);
            return @object;
        }

        internal async Task<string> Get(string suffix, CancellationToken cancellationToken, int retryCount, params QueryParameter[] parameters)
        {
            Uri uri = BuildUri(suffix, parameters);
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                await AddCredentialsToHeaderIfNeededAsync(request, cancellationToken);
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, response.StatusCode, responseContent, retry => Get(suffix, cancellationToken, retry, parameters), retryCount, cancellationToken);
            }

            return responseContent; //Content is assumed JSON
        }

        internal async Task<T> Post<T>(string suffix, CancellationToken cancellationToken, params QueryParameter[] parameters)
        {
            string json = await Post(suffix, cancellationToken, 0, parameters);
            T @object = JsonSerializer.Deserialize<T>(json);
            return @object;
        }

        internal async Task<T> PostWithAttachmentFileUpload<T>(string suffix, AttachmentFileUpload attachmentFile, CancellationToken cancellationToken, params QueryParameter[] parameters)
        {
            string json = await PostWithAttachmentFileUpload(suffix, attachmentFile, cancellationToken, 0, parameters);
            T @object = JsonSerializer.Deserialize<T>(json);
            return @object;
        }

        internal async Task<string> PostWithAttachmentFileUpload(string suffix, AttachmentFileUpload attachmentFile, CancellationToken cancellationToken, int retryCount, params QueryParameter[] parameters)
        {
            Uri uri = BuildUri(suffix, parameters);
            using (MultipartFormDataContent multipartFormContent = new MultipartFormDataContent())
            {
                multipartFormContent.Add(new StreamContent(attachmentFile.Stream), name: "file", fileName: attachmentFile.Filename);
                HttpResponseMessage response;
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri))
                {
                    request.Content = multipartFormContent;
                    await AddCredentialsToHeaderIfNeededAsync(request, cancellationToken);
                    response = await _httpClient.SendAsync(request, cancellationToken);
                }
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return await PreformRetryIfNeededOrThrow(uri, response.StatusCode, responseContent, retry => PostWithAttachmentFileUpload(suffix, attachmentFile, cancellationToken, retry, parameters), retryCount, cancellationToken);
                }

                return responseContent; //Content is assumed JSON
            }
        }

        internal async Task<string> Post(string suffix, CancellationToken cancellationToken, int retryCount, params QueryParameter[] parameters)
        {
            Uri uri = BuildUri(suffix, parameters);
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                await AddCredentialsToHeaderIfNeededAsync(request, cancellationToken);
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            string content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, response.StatusCode, content, retry => Post(suffix, cancellationToken, retry, parameters), retryCount, cancellationToken);
            }

            return content; //Content is assumed JSON
        }

        internal async Task<T> Put<T>(string suffix, CancellationToken cancellationToken, params QueryParameter[] parameters)
        {
            string json = await Put(suffix, cancellationToken, 0, parameters);
            T @object = JsonSerializer.Deserialize<T>(json);
            return @object;
        }

        internal async Task<string> Put(string suffix, CancellationToken cancellationToken, int retryCount, params QueryParameter[] parameters)
        {
            Uri uri = BuildUri(suffix, parameters);
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, uri))
            {
                await AddCredentialsToHeaderIfNeededAsync(request, cancellationToken);
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, response.StatusCode, responseContent, retry => Put(suffix, cancellationToken, retry, parameters), retryCount, cancellationToken);
            }

            return responseContent; //Content is assumed JSON
        }

        internal async Task<T> PutWithJsonPayload<T>(string suffix, CancellationToken cancellationToken, string payload, params QueryParameter[] parameters)
        {
            string json = await PutWithJsonPayload(suffix, cancellationToken, payload, 0, parameters);
            T @object = JsonSerializer.Deserialize<T>(json);
            return @object;
        }

        internal async Task<string> PutWithJsonPayload(string suffix, CancellationToken cancellationToken, string payload, int retryCount, params QueryParameter[] parameters)
        {
            Uri uri = BuildUri(suffix, parameters);
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, uri))
            {
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                await AddCredentialsToHeaderIfNeededAsync(request, cancellationToken);
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, response.StatusCode, responseContent, retry => PutWithJsonPayload(suffix, cancellationToken, payload, retry, parameters), retryCount, cancellationToken);
            }

            return responseContent; //Content is assumed JSON
        }

        internal async Task<T> PostWithJsonPayload<T>(string suffix, CancellationToken cancellationToken, string payload, params QueryParameter[] parameters)
        {
            string json = await PostWithJsonPayload(suffix, cancellationToken, payload, 0, parameters);
            T @object = JsonSerializer.Deserialize<T>(json);
            return @object;
        }

        internal async Task<string> PostWithJsonPayload(string suffix, CancellationToken cancellationToken, string payload, int retryCount, params QueryParameter[] parameters)
        {
            Uri uri = BuildUri(suffix, parameters);
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                await AddCredentialsToHeaderIfNeededAsync(request, cancellationToken);
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, response.StatusCode, responseContent, retry => PostWithJsonPayload(suffix, cancellationToken, payload, retry, parameters), retryCount, cancellationToken);
            }

            return responseContent; //Content is assumed JSON
        }

        internal async Task<string> PostWithJsonPayloadToNonApi(string suffix, CancellationToken cancellationToken, string payload, int retryCount, params QueryParameter[] parameters)
        {
            Uri uri = BuildNonApiUri(suffix, parameters);
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                await AddCredentialsToHeaderIfNeededAsync(request, cancellationToken);
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, response.StatusCode, responseContent, retry => PostWithJsonPayload(suffix, cancellationToken, payload, retry, parameters), retryCount, cancellationToken);
            }

            return responseContent; //Content is assumed JSON
        }

        private string FormatExceptionUrlAccordingToClientOptions(string fullUrl)
        {
            switch (_client.Options.ApiCallExceptionOption)
            {
                case ApiCallExceptionOption.IncludeUrlAndCredentials:
                    return fullUrl;
                case ApiCallExceptionOption.IncludeUrlButMaskCredentials:
                    // ReSharper disable StringLiteralTypo
                    return fullUrl.Replace($"key={_apiKey}&token={_token}", "key=XXXXX&token=XXXXXXXXXX");
                case ApiCallExceptionOption.DoNotIncludeTheUrl:
                    return string.Empty.PadLeft(5, 'X');
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static StringBuilder GetParametersAsString(QueryParameter[] parameters)
        {
            StringBuilder parameterString = new StringBuilder();
            if (parameters == null || parameters.Length == 0)
            {
                return parameterString;
            }

            foreach (QueryParameter parameter in parameters)
            {
                parameterString.Append($"&{parameter.Name}={parameter.GetValueAsApiFormattedString()}");
            }

            return parameterString;
        }

        internal int GetQueryStringLength(params QueryParameter[] parameters)
        {
            return GetQueryStringCredentialPrefixLength() + GetParametersAsString(parameters).Length;
        }

        private Uri BuildUri(string suffix, params QueryParameter[] parameters)
        {
            string queryString = GetQueryStringCredentials();
            string additionalParameters = GetParametersAsString(parameters).ToString();
            if (string.IsNullOrWhiteSpace(queryString))
            {
                additionalParameters = additionalParameters.TrimStart('&');
            }

            if (string.IsNullOrWhiteSpace(queryString) && string.IsNullOrWhiteSpace(additionalParameters))
            {
                return new Uri($"{BaseUrl}{suffix}");
            }

            string separator = suffix.Contains("?") ? "&" : "?";
            return new Uri($"{BaseUrl}{suffix}{separator}{queryString}{additionalParameters}");
        }
        
        private Uri BuildNonApiUri(string suffix, params QueryParameter[] parameters)
        {
            string queryString = GetQueryStringCredentials();
            string additionalParameters = GetParametersAsString(parameters).ToString();
            if (string.IsNullOrWhiteSpace(queryString))
            {
                additionalParameters = additionalParameters.TrimStart('&');
            }

            if (string.IsNullOrWhiteSpace(queryString) && string.IsNullOrWhiteSpace(additionalParameters))
            {
                return new Uri($"{NonApiBaseUrl}{suffix}");
            }

            string separator = suffix.Contains("?") ? "&" : "?";
            return new Uri($"{NonApiBaseUrl}{suffix}{separator}{queryString}{additionalParameters}");
        }


        internal async Task<string> Delete(string suffix, CancellationToken cancellationToken, int retryCount)
        {
            Uri uri = BuildUri(suffix);
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri))
            {
                await AddCredentialsToHeaderIfNeededAsync(request, cancellationToken);
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, response.StatusCode, responseContent, retry => Delete(suffix, cancellationToken, retry), retryCount, cancellationToken);
            }

            return null;
        }

        internal async Task<string> DeleteToNonApi(string suffix, CancellationToken cancellationToken, int retryCount)
        {
            Uri uri = BuildNonApiUri(suffix);
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri))
            {
                await AddCredentialsToHeaderIfNeededAsync(request, cancellationToken);
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, response.StatusCode, responseContent, retry => Delete(suffix, cancellationToken, retry), retryCount, cancellationToken);
            }

            return null;
        }

        private async Task<string> PreformRetryIfNeededOrThrow(Uri uri, HttpStatusCode statusCode, string responseContent, Func<int, Task<string>> toRetry, int retryCount, CancellationToken cancellationToken)
        {
            int statusCodeAsInteger = (int)statusCode;
            bool isTooManyRequestsStatusCode = statusCodeAsInteger == 429;
            if ((responseContent.Contains("API_TOKEN_LIMIT_EXCEEDED") || isTooManyRequestsStatusCode) && retryCount <= _client.Options.MaxRetryCountForTokenLimitExceeded)
            {
                await Task.Delay(TimeSpan.FromSeconds(_client.Options.DelayInSecondsToWaitInTokenLimitExceededRetry), cancellationToken);
                retryCount++;
                return await toRetry(retryCount);
            }

            throw new TrelloApiException($"{responseContent} [{statusCodeAsInteger}: {statusCode}]", FormatExceptionUrlAccordingToClientOptions(uri.AbsoluteUri), statusCode); //Content is assumed Error Message       
        }

        private int GetQueryStringCredentialPrefixLength()
        {
            if (UsesOAuth2 || _client.Options.SendCredentialsMode == SendCredentialsMode.Header)
            {
                return 0;
            }

            return $"?{GetQueryStringCredentials()}".Length;
        }

        private string GetQueryStringCredentials()
        {
            if (UsesOAuth2 || _client.Options.SendCredentialsMode == SendCredentialsMode.Header)
            {
                return string.Empty;
            }

            return $"key={_apiKey}&token={_token}";
        }

        internal async Task AddCredentialsToHeaderIfNeededAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (UsesOAuth2)
            {
                await AddCredentialsToHeaderAsync(request, cancellationToken);
                return;
            }

            if (_client.Options.SendCredentialsMode != SendCredentialsMode.Header)
            {
                return;
            }

            await AddCredentialsToHeaderAsync(request, cancellationToken);
        }

        internal async Task AddCredentialsToHeaderAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (UsesOAuth2)
            {
                string accessToken = await _oauth2TokenProvider.GetAccessTokenAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new InvalidOperationException("The OAuth 2.0 token provider returned an empty access token.");
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                return;
            }

            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"OAuth oauth_consumer_key=\"{_apiKey}\", oauth_token=\"{_token}\"");
        }
    }
}

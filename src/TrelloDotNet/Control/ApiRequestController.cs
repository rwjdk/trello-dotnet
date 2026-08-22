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
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _token;
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

        public string Token => _token;

        public string ApiKey => _apiKey;

        internal async Task<T> Get<T>(string suffix, CancellationToken cancellationToken, params QueryParameter[] parameters)
        {
            string json = await Get(suffix, cancellationToken, 0, parameters);
            T @object = JsonSerializer.Deserialize<T>(json);
            return @object;
        }

        internal async Task<string> Get(string suffix, CancellationToken cancellationToken, int retryCount, params QueryParameter[] parameters)
        {
            Uri uri = BuildUri(suffix, parameters);
            HttpResponseMessage response = await _httpClient.GetAsync(uri, cancellationToken);
            string responseContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, responseContent, retry => Get(suffix, cancellationToken, retry, parameters), retryCount, cancellationToken);
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
                HttpResponseMessage response = await _httpClient.PostAsync(uri, multipartFormContent, cancellationToken);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return await PreformRetryIfNeededOrThrow(uri, responseContent, retry => PostWithAttachmentFileUpload(suffix, attachmentFile, cancellationToken, retry, parameters), retryCount, cancellationToken);
                }

                return responseContent; //Content is assumed JSON
            }
        }

        internal async Task<string> Post(string suffix, CancellationToken cancellationToken, int retryCount, params QueryParameter[] parameters)
        {
            Uri uri = BuildUri(suffix, parameters);
            HttpResponseMessage response = await _httpClient.PostAsync(uri, null, cancellationToken);
            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, content, retry => Post(suffix, cancellationToken, retry, parameters), retryCount, cancellationToken);
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
            HttpResponseMessage response = await _httpClient.PutAsync(uri, null, cancellationToken);
            string responseContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, responseContent, retry => Put(suffix, cancellationToken, retry, parameters), retryCount, cancellationToken);
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
            HttpResponseMessage response = await _httpClient.PutAsync(uri, new StringContent(payload, Encoding.UTF8, "application/json"), cancellationToken);
            string responseContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, responseContent, retry => PutWithJsonPayload(suffix, cancellationToken, payload, retry, parameters), retryCount, cancellationToken);
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
                    return fullUrl.Replace($"?key={_apiKey}&token={_token}", "?key=XXXXX&token=XXXXXXXXXX");
                case ApiCallExceptionOption.DoNotIncludeTheUrl:
                    return string.Empty.PadLeft(5, 'X');
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static StringBuilder GetParametersAsString(QueryParameter[] parameters)
        {
            StringBuilder parameterString = new StringBuilder();
            foreach (QueryParameter parameter in parameters)
            {
                parameterString.Append($"&{parameter.Name}={parameter.GetValueAsApiFormattedString()}");
            }

            return parameterString;
        }

        private Uri BuildUri(string suffix, params QueryParameter[] parameters)
        {
            string separator = suffix.Contains("?") ? "&" : "?";
            return new Uri($"{BaseUrl}{suffix}{separator}key={_apiKey}&token={_token}" + GetParametersAsString(parameters));
        }

        internal async Task<string> Delete(string suffix, CancellationToken cancellationToken, int retryCount)
        {
            Uri uri = BuildUri(suffix);
            HttpResponseMessage response = await _httpClient.DeleteAsync(uri, cancellationToken);
            string responseContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return await PreformRetryIfNeededOrThrow(uri, responseContent, retry => Delete(suffix, cancellationToken, retry), retryCount, cancellationToken);
            }

            return null;
        }

        private async Task<string> PreformRetryIfNeededOrThrow(Uri uri, string responseContent, Func<int, Task<string>> toRetry, int retryCount, CancellationToken cancellationToken)
        {
            if (responseContent.Contains("API_TOKEN_LIMIT_EXCEEDED") && retryCount <= _client.Options.MaxRetryCountForTokenLimitExceeded)
            {
                await Task.Delay(TimeSpan.FromSeconds(_client.Options.DelayInSecondsToWaitInTokenLimitExceededRetry), cancellationToken);
                retryCount++;
                return await toRetry(retryCount);
            }

            throw new TrelloApiException(responseContent, FormatExceptionUrlAccordingToClientOptions(uri.AbsoluteUri)); //Content is assumed Error Message       
        }
    }
}
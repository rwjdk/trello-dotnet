using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model;

namespace TrelloDotNet
{
    /// <summary>
    /// Provides operations for the Trello OAuth 2.0 authorization code and refresh token flows.
    /// </summary>
    public static class TrelloOAuth2Flow
    {
        private const string AuthorizationEndpoint = "https://auth.atlassian.com/authorize";
        private const string TokenEndpoint = "https://auth.atlassian.com/oauth/token";
        private static readonly HttpClient StaticHttpClient = new HttpClient();

        /// <summary>
        /// Creates an authorization URL and PKCE code verifier for a Trello OAuth 2.0 authorization flow.
        /// </summary>
        /// <param name="clientId">OAuth 2.0 client ID</param>
        /// <param name="redirectUri">Callback URL configured for the OAuth 2.0 client</param>
        /// <param name="scopes">OAuth 2.0 scopes to request</param>
        /// <param name="generateRefreshToken">Whether to request a refresh token for access after the user is no longer present (default: true)</param>
        /// <returns>The authorization URL, PKCE code verifier, and state</returns>
        public static TrelloOAuth2AuthorizationRequest CreateAuthorizationRequest(
            string clientId,
            string redirectUri,
            IEnumerable<TrelloOAuth2Scope> scopes,
            bool generateRefreshToken = true)
        {
            ValidateClientId(clientId);
            ValidateRedirectUri(redirectUri);
            string[] requestedScopes = ValidateScopes(scopes, generateRefreshToken);
            string codeVerifier = CreateCodeVerifier();
            string codeChallenge = CreateCodeChallenge(codeVerifier);
            string state = CreateCodeVerifier();

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["scope"] = string.Join(" ", requestedScopes),
                ["redirect_uri"] = redirectUri,
                ["response_type"] = "code",
                ["prompt"] = "consent",
                ["code_challenge"] = codeChallenge,
                ["code_challenge_method"] = "S256"
            };

            parameters["state"] = state;

            string queryString = string.Join("&", parameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
            return new TrelloOAuth2AuthorizationRequest
            {
                AuthorizationUri = new Uri($"{AuthorizationEndpoint}?{queryString}"),
                CodeVerifier = codeVerifier,
                State = state
            };
        }

        /// <summary>
        /// Extracts the authorization code and state from a Trello OAuth 2.0 callback URL.
        /// </summary>
        /// <param name="callbackUrl">Complete callback URL returned by the authorization server</param>
        /// <returns>The authorization code and state</returns>
        public static (string Code, string State) ParseCallbackUrl(string callbackUrl)
        {
            if (string.IsNullOrWhiteSpace(callbackUrl) || !Uri.TryCreate(callbackUrl, UriKind.Absolute, out Uri callbackUri))
            {
                throw new ArgumentException("You need to specify an absolute OAuth 2.0 callback URL.", nameof(callbackUrl));
            }

            Dictionary<string, string> parameters = ParseQueryString(callbackUri.Query);
            if (parameters.TryGetValue("error", out string error))
            {
                parameters.TryGetValue("error_description", out string errorDescription);
                string description = string.IsNullOrWhiteSpace(errorDescription) ? error : $"{error}: {errorDescription}";
                throw new InvalidOperationException($"OAuth 2.0 authorization failed: {description}");
            }

            if (!parameters.TryGetValue("code", out string code) || string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("The OAuth 2.0 callback URL does not contain an authorization code.", nameof(callbackUrl));
            }

            if (!parameters.TryGetValue("state", out string state) || string.IsNullOrWhiteSpace(state))
            {
                throw new ArgumentException("The OAuth 2.0 callback URL does not contain state.", nameof(callbackUrl));
            }

            return (code, state);
        }

        /// <summary>
        /// Exchanges an OAuth 2.0 authorization code for access and refresh tokens.
        /// </summary>
        /// <param name="clientId">OAuth 2.0 client ID</param>
        /// <param name="code">Authorization code returned to the callback URL</param>
        /// <param name="codeVerifier">PKCE code verifier created with the authorization request</param>
        /// <param name="redirectUri">Callback URL used for the authorization request</param>
        /// <param name="clientSecret">OAuth 2.0 client secret for a confidential client, or null for a public client</param>
        /// <param name="httpClient">Optional HTTP Client to use for the token request</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The OAuth 2.0 token response</returns>
        public static async Task<TrelloOAuth2TokenResponse> ExchangeCodeAsync(
            string clientId,
            string code,
            string codeVerifier,
            string redirectUri,
            string clientSecret = null,
            HttpClient httpClient = null,
            CancellationToken cancellationToken = default)
        {
            ValidateClientId(clientId);
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("You need to specify an authorization code.", nameof(code));
            }

            if (string.IsNullOrWhiteSpace(codeVerifier))
            {
                throw new ArgumentException("You need to specify a PKCE code verifier.", nameof(codeVerifier));
            }

            ValidateRedirectUri(redirectUri);
            Dictionary<string, string> payload = CreateTokenPayload(clientId, clientSecret, "authorization_code");
            payload["redirect_uri"] = redirectUri;
            payload["code"] = code;
            payload["code_verifier"] = codeVerifier;
            return await RequestTokenAsync(payload, httpClient, cancellationToken);
        }

        /// <summary>
        /// Exchanges a rotating refresh token for a new access token and refresh token.
        /// </summary>
        /// <param name="clientId">OAuth 2.0 client ID</param>
        /// <param name="refreshToken">Current refresh token</param>
        /// <param name="clientSecret">OAuth 2.0 client secret for a confidential client, or null for a public client</param>
        /// <param name="httpClient">Optional HTTP Client to use for the token request</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The new OAuth 2.0 token response</returns>
        public static async Task<TrelloOAuth2TokenResponse> RefreshTokenAsync(
            string clientId,
            string refreshToken,
            string clientSecret = null,
            HttpClient httpClient = null,
            CancellationToken cancellationToken = default)
        {
            ValidateClientId(clientId);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentException("You need to specify a refresh token.", nameof(refreshToken));
            }

            Dictionary<string, string> payload = CreateTokenPayload(clientId, clientSecret, "refresh_token");
            payload["refresh_token"] = refreshToken;
            return await RequestTokenAsync(payload, httpClient, cancellationToken);
        }

        private static Dictionary<string, string> CreateTokenPayload(string clientId, string clientSecret, string grantType)
        {
            Dictionary<string, string> payload = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["grant_type"] = grantType
            };

            if (!string.IsNullOrWhiteSpace(clientSecret))
            {
                payload["client_secret"] = clientSecret;
            }

            return payload;
        }

        private static async Task<TrelloOAuth2TokenResponse> RequestTokenAsync(
            Dictionary<string, string> payload,
            HttpClient httpClient,
            CancellationToken cancellationToken)
        {
            string json = JsonSerializer.Serialize(payload);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = await (httpClient ?? StaticHttpClient).SendAsync(request, cancellationToken))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new TrelloApiException($"{responseContent} [{(int)response.StatusCode}: {response.StatusCode}]", TokenEndpoint, response.StatusCode);
                    }

                    TrelloOAuth2TokenResponse token = JsonSerializer.Deserialize<TrelloOAuth2TokenResponse>(responseContent);
                    if (token == null)
                    {
                        throw new TrelloApiException("The OAuth 2.0 token endpoint returned an empty response.", TokenEndpoint, response.StatusCode);
                    }

                    return token;
                }
            }
        }

        private static string CreateCodeVerifier()
        {
            byte[] bytes = new byte[32];
            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(bytes);
            }

            return Base64UrlEncode(bytes);
        }

        private static string CreateCodeChallenge(string codeVerifier)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return Base64UrlEncode(sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier)));
            }
        }

        private static string Base64UrlEncode(byte[] value)
        {
            return Convert.ToBase64String(value).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static void ValidateRedirectUri(string redirectUri)
        {
            if (string.IsNullOrWhiteSpace(redirectUri) || !Uri.TryCreate(redirectUri, UriKind.Absolute, out _))
            {
                throw new ArgumentException("You need to specify an absolute callback URL.", nameof(redirectUri));
            }
        }

        private static void ValidateClientId(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException("You need to specify an OAuth 2.0 client ID.", nameof(clientId));
            }
        }

        private static string[] ValidateScopes(IEnumerable<TrelloOAuth2Scope> scopes, bool generateRefreshToken)
        {
            if (scopes == null)
            {
                throw new ArgumentNullException(nameof(scopes));
            }

            TrelloOAuth2Scope[] requestedScopes = scopes.Distinct().ToArray();
            if (requestedScopes.Length == 0)
            {
                throw new ArgumentException("You need to specify at least one OAuth 2.0 scope.", nameof(scopes));
            }

            IEnumerable<string> scopeValues = requestedScopes.Select(x => x.GetJsonPropertyName());
            if (generateRefreshToken)
            {
                scopeValues = scopeValues.Concat(new[] { "offline_access" });
            }

            return scopeValues.ToArray();
        }

        private static Dictionary<string, string> ParseQueryString(string queryString)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (string part in queryString.TrimStart('?').Split('&'))
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    continue;
                }

                string[] nameAndValue = part.Split(new[] { '=' }, 2);
                string name = Uri.UnescapeDataString(nameAndValue[0].Replace("+", " "));
                string value = nameAndValue.Length == 2
                    ? Uri.UnescapeDataString(nameAndValue[1].Replace("+", " "))
                    : string.Empty;

                if (parameters.ContainsKey(name))
                {
                    throw new ArgumentException($"The OAuth 2.0 callback URL contains the '{name}' parameter more than once.", nameof(queryString));
                }

                parameters.Add(name, value);
            }

            return parameters;
        }
    }
}

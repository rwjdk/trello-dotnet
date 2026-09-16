using System.Text.Json.Serialization;

namespace TrelloDotNet.Model
{
    /// <summary>
    /// Tokens returned by the Trello OAuth 2.0 token endpoint.
    /// </summary>
    public class TrelloOAuth2TokenResponse
    {
        /// <summary>
        /// Access token used to authorize Trello API requests.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Rotating refresh token used to obtain a new token pair.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Number of seconds for which the access token is valid.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Space-separated scopes granted to the access token.
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// OpenID Connect ID token when one is returned by the authorization server.
        /// </summary>
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        /// <summary>
        /// OAuth 2.0 token type returned by the authorization server.
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}

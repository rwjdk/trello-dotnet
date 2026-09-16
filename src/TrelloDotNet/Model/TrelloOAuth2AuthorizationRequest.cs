using System;

namespace TrelloDotNet.Model
{
    /// <summary>
    /// Contains the values required to start a Trello OAuth 2.0 authorization flow.
    /// </summary>
    public class TrelloOAuth2AuthorizationRequest
    {
        /// <summary>
        /// URL to which the user should be redirected to authorize the application.
        /// </summary>
        public Uri AuthorizationUri { get; internal set; }

        /// <summary>
        /// PKCE code verifier that must be supplied when exchanging the authorization code.
        /// </summary>
        public string CodeVerifier { get; internal set; }

        /// <summary>
        /// State value supplied to the authorization endpoint.
        /// </summary>
        public string State { get; internal set; }
    }
}

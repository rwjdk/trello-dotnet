using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Model;

namespace TrelloDotNet.Control
{
    internal sealed class RefreshingTrelloOAuth2TokenProvider : ITrelloOAuth2TokenProvider
    {
        private static readonly TimeSpan RefreshBeforeExpiration = TimeSpan.FromMinutes(1);
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _httpClient;
        private readonly TrelloOAuth2RefreshTokenSaver _refreshTokenSaver;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private string _accessToken;
        private string _refreshToken;
        private DateTimeOffset _accessTokenExpiresAt;
        private bool _refreshTokenIsPersisted = true;

        internal RefreshingTrelloOAuth2TokenProvider(
            string clientId,
            string clientSecret,
            string refreshToken,
            TrelloOAuth2RefreshTokenSaver refreshTokenSaver,
            HttpClient httpClient)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException("You need to specify an OAuth 2.0 client ID.", nameof(clientId));
            }

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentException("You need to specify an OAuth 2.0 refresh token.", nameof(refreshToken));
            }

            _clientId = clientId;
            _clientSecret = clientSecret;
            _httpClient = httpClient;
            _refreshTokenSaver = refreshTokenSaver ?? throw new ArgumentNullException(nameof(refreshTokenSaver));
            _refreshToken = refreshToken;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (!_refreshTokenIsPersisted)
                {
                    await PersistRefreshTokenAsync();
                }

                if (!string.IsNullOrWhiteSpace(_accessToken) &&
                    DateTimeOffset.UtcNow < _accessTokenExpiresAt.Subtract(RefreshBeforeExpiration))
                {
                    return _accessToken;
                }

                TrelloOAuth2TokenResponse token = await TrelloOAuth2Flow.RefreshTokenAsync(
                    _clientId,
                    _refreshToken,
                    _clientSecret,
                    _httpClient,
                    cancellationToken);
                if (string.IsNullOrWhiteSpace(token.AccessToken) || string.IsNullOrWhiteSpace(token.RefreshToken))
                {
                    throw new InvalidOperationException("The OAuth 2.0 token endpoint did not return both an access token and a refresh token.");
                }

                _accessToken = token.AccessToken;
                _refreshToken = token.RefreshToken;
                _accessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);
                _refreshTokenIsPersisted = false;

                await PersistRefreshTokenAsync();
                return _accessToken;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task PersistRefreshTokenAsync()
        {
            await _refreshTokenSaver(_refreshToken);
            _refreshTokenIsPersisted = true;
        }
    }
}

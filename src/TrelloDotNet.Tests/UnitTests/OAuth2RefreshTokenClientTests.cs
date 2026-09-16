using System.Net;
using System.Text;
using System.Text.Json;

namespace TrelloDotNet.Tests.UnitTests;

public class OAuth2RefreshTokenClientTests
{
    [Fact]
    public async Task ConfidentialClient_RefreshesPersistsAndUsesAccessToken()
    {
        OAuthHandler handler = new OAuthHandler();
        using HttpClient httpClient = new HttpClient(handler);
        List<string> persistedRefreshTokens = new();
        TrelloClient client = TrelloClient.FromOAuth2RefreshToken(
            "client-id",
            "client-secret",
            "original-refresh-token",
            newRefreshToken =>
            {
                persistedRefreshTokens.Add(newRefreshToken);
                return Task.CompletedTask;
            },
            httpClient: httpClient);

        _ = await client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken);
        _ = await client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, handler.TokenRequestCount);
        Assert.Equal(2, handler.ApiRequestCount);
        Assert.Equal("client-id", handler.LastTokenPayload["client_id"]);
        Assert.Equal("client-secret", handler.LastTokenPayload["client_secret"]);
        Assert.Equal("refresh_token", handler.LastTokenPayload["grant_type"]);
        Assert.Equal("original-refresh-token", handler.LastTokenPayload["refresh_token"]);
        Assert.Equal(new[] { "replacement-refresh-token" }, persistedRefreshTokens);
        Assert.All(handler.ApiAuthorizationParameters, value => Assert.Equal("replacement-access-token", value));
    }

    [Fact]
    public async Task PublicClient_DoesNotSendClientSecret()
    {
        OAuthHandler handler = new OAuthHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = TrelloClient.FromOAuth2RefreshToken(
            "client-id",
            "original-refresh-token",
            _ => Task.CompletedTask,
            httpClient: httpClient);

        _ = await client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken);

        Assert.False(handler.LastTokenPayload.ContainsKey("client_secret"));
    }

    [Fact]
    public async Task ConcurrentRequests_OnlyRefreshOnce()
    {
        OAuthHandler handler = new OAuthHandler(delayTokenResponse: true);
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = TrelloClient.FromOAuth2RefreshToken(
            "client-id",
            "original-refresh-token",
            _ => Task.CompletedTask,
            httpClient: httpClient);

        Task<string>[] requests = Enumerable.Range(0, 10)
            .Select(_ => client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken))
            .ToArray();
        await Task.WhenAll(requests);

        Assert.Equal(1, handler.TokenRequestCount);
        Assert.Equal(10, handler.ApiRequestCount);
    }

    [Fact]
    public async Task FailedPersistence_IsRetriedWithoutUsingRefreshTokenAgain()
    {
        OAuthHandler handler = new OAuthHandler();
        using HttpClient httpClient = new HttpClient(handler);
        int persistenceAttempts = 0;
        TrelloClient client = TrelloClient.FromOAuth2RefreshToken(
            "client-id",
            "original-refresh-token",
            _ =>
            {
                persistenceAttempts++;
                return persistenceAttempts == 1
                    ? Task.FromException(new InvalidOperationException("Storage unavailable"))
                    : Task.CompletedTask;
            },
            httpClient: httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken));
        _ = await client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(2, persistenceAttempts);
        Assert.Equal(1, handler.TokenRequestCount);
        Assert.Equal(1, handler.ApiRequestCount);
    }

    private sealed class OAuthHandler : HttpMessageHandler
    {
        private readonly bool _delayTokenResponse;
        private int _tokenRequestCount;
        private int _apiRequestCount;

        public OAuthHandler(bool delayTokenResponse = false)
        {
            _delayTokenResponse = delayTokenResponse;
        }

        public int TokenRequestCount => _tokenRequestCount;

        public int ApiRequestCount => _apiRequestCount;

        public Dictionary<string, string> LastTokenPayload { get; private set; } = new();

        public List<string?> ApiAuthorizationParameters { get; } = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri?.Host == "auth.atlassian.com")
            {
                Interlocked.Increment(ref _tokenRequestCount);
                string payload = await request.Content!.ReadAsStringAsync(cancellationToken);
                LastTokenPayload = JsonSerializer.Deserialize<Dictionary<string, string>>(payload)!;
                if (_delayTokenResponse)
                {
                    await Task.Delay(50, cancellationToken);
                }

                const string tokenJson = "{\"access_token\":\"replacement-access-token\",\"refresh_token\":\"replacement-refresh-token\",\"expires_in\":3600}";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(tokenJson, Encoding.UTF8, "application/json")
                };
            }

            Interlocked.Increment(ref _apiRequestCount);
            lock (ApiAuthorizationParameters)
            {
                Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
                ApiAuthorizationParameters.Add(request.Headers.Authorization?.Parameter);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
        }
    }
}

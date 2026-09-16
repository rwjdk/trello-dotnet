using System.Net;
using System.Text;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class OAuth2CredentialsTests
{
    [Fact]
    public async Task GetAsync_SendsBearerTokenWithoutLegacyCredentials()
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = TrelloClient.FromOAuth2AccessToken("access-token", httpClient: httpClient);

        _ = await client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken);

        RecordedRequest request = Assert.Single(handler.Requests);
        Assert.Equal(string.Empty, request.Query);
        Assert.Equal("Bearer", request.AuthorizationScheme);
        Assert.Equal("access-token", request.AuthorizationParameter);
    }

    [Fact]
    public async Task GetAsync_AsksTokenProviderForEveryRequest()
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        RotatingTokenProvider tokenProvider = new RotatingTokenProvider();
        TrelloClient client = new TrelloClient(tokenProvider, httpClient: httpClient);

        _ = await client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken);
        _ = await client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(2, tokenProvider.RequestCount);
        Assert.Equal("access-token-1", handler.Requests[0].AuthorizationParameter);
        Assert.Equal("access-token-2", handler.Requests[1].AuthorizationParameter);
    }

    [Fact]
    public async Task GetTokenMemberAsync_UsesMembersMeWithOAuth2()
    {
        RecordingHandler handler = new RecordingHandler("{}");
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = TrelloClient.FromOAuth2AccessToken("access-token", httpClient: httpClient);

        _ = await client.GetTokenMemberAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("/1/members/me", Assert.Single(handler.Requests).Path);
    }

    [Fact]
    public async Task TokenOnlyOperations_ThrowHelpfulExceptionsWithOAuth2()
    {
        TrelloClient client = TrelloClient.FromOAuth2AccessToken("access-token");

        await Assert.ThrowsAsync<NotSupportedException>(() => client.GetTokenInformationAsync(TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<NotSupportedException>(() => client.GetWebhooksForCurrentTokenAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DownloadAttachmentAsync_SendsBearerTokenToTrello()
    {
        RecordingHandler handler = new RecordingHandler("attachment");
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = TrelloClient.FromOAuth2AccessToken("access-token", httpClient: httpClient);

        using Stream stream = await client.DownloadAttachmentAsync("https://api.trello.com/download/file", TestContext.Current.CancellationToken);

        RecordedRequest request = Assert.Single(handler.Requests);
        Assert.Equal("Bearer", request.AuthorizationScheme);
        Assert.Equal("access-token", request.AuthorizationParameter);
    }

    private sealed class RotatingTokenProvider : ITrelloOAuth2TokenProvider
    {
        public int RequestCount { get; private set; }

        public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            RequestCount++;
            return Task.FromResult($"access-token-{RequestCount}");
        }
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly string _responseContent;

        public RecordingHandler(string responseContent = "{}")
        {
            _responseContent = responseContent;
        }

        public List<RecordedRequest> Requests { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(new RecordedRequest(
                request.RequestUri?.AbsolutePath ?? string.Empty,
                request.RequestUri?.Query ?? string.Empty,
                request.Headers.Authorization?.Scheme,
                request.Headers.Authorization?.Parameter));

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }

    private sealed record RecordedRequest(string Path, string Query, string? AuthorizationScheme, string? AuthorizationParameter);
}

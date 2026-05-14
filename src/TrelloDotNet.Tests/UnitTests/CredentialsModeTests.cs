using System.Net;
using System.Text;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class CredentialsModeTests
{
    [Fact]
    public async Task GetAsync_SendsCredentialsInQueryString_ByDefault()
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", httpClient: httpClient);

        _ = await client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken);

        RecordedRequest request = Assert.Single(handler.Requests);
        Assert.Equal("?key=key&token=token", request.Query);
        Assert.Null(request.AuthorizationScheme);
        Assert.Null(request.AuthorizationParameter);
    }

    [Fact]
    public async Task GetAsync_AppendsParametersAfterQueryStringCredentials()
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", httpClient: httpClient);

        _ = await client.GetAsync("members/me", TestContext.Current.CancellationToken, new QueryParameter("name", "Hello World"));

        RecordedRequest request = Assert.Single(handler.Requests);
        Assert.Equal("?key=key&token=token&name=Hello+World", request.Query);
        Assert.Null(request.AuthorizationScheme);
        Assert.Null(request.AuthorizationParameter);
    }

    [Fact]
    public async Task GetAsync_AppendsCredentialsWithAmpersand_WhenSuffixAlreadyHasQueryString()
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", httpClient: httpClient);

        _ = await client.GetAsync("members/me?fields=username", cancellationToken: TestContext.Current.CancellationToken);

        RecordedRequest request = Assert.Single(handler.Requests);
        Assert.Equal("?fields=username&key=key&token=token", request.Query);
        Assert.Null(request.AuthorizationScheme);
        Assert.Null(request.AuthorizationParameter);
    }

    [Fact]
    public async Task GetAsync_SendsCredentialsInAuthorizationHeader_WhenHeaderModeIsSelected()
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", new TrelloClientOptions
        {
            SendCredentialsMode = SendCredentialsMode.Header
        }, httpClient);

        _ = await client.GetAsync("members/me", cancellationToken: TestContext.Current.CancellationToken);

        RecordedRequest request = Assert.Single(handler.Requests);
        Assert.Equal(string.Empty, request.Query);
        Assert.Equal("OAuth", request.AuthorizationScheme);
        Assert.Contains("oauth_consumer_key=\"key\"", request.AuthorizationParameter, StringComparison.Ordinal);
        Assert.Contains("oauth_token=\"token\"", request.AuthorizationParameter, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_SendsOnlyAdditionalParametersInQueryString_WhenHeaderModeIsSelected()
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", new TrelloClientOptions
        {
            SendCredentialsMode = SendCredentialsMode.Header
        }, httpClient);

        _ = await client.GetAsync("members/me", TestContext.Current.CancellationToken, new QueryParameter("name", "Hello World"));

        RecordedRequest request = Assert.Single(handler.Requests);
        Assert.Equal("?name=Hello+World", request.Query);
        Assert.Equal("OAuth", request.AuthorizationScheme);
        Assert.Contains("oauth_consumer_key=\"key\"", request.AuthorizationParameter, StringComparison.Ordinal);
        Assert.Contains("oauth_token=\"token\"", request.AuthorizationParameter, StringComparison.Ordinal);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<RecordedRequest> Requests { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(new RecordedRequest(
                request.RequestUri?.Query ?? string.Empty,
                request.Headers.Authorization?.Scheme,
                request.Headers.Authorization?.Parameter));

            const string json = "{}";
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }

    private sealed record RecordedRequest(string Query, string? AuthorizationScheme, string? AuthorizationParameter);
}

using System.Net;
using System.Net.Http;
using System.Text;

namespace TrelloDotNet.Tests.UnitTests;

public class DownloadAttachmentSecurityTests
{
    [Theory]
    [InlineData("https://api.trello.com/1/cards/card/attachments/attachment/download/file.txt")]
    [InlineData("https://trello.com/1/cards/card/attachments/attachment/download/file.txt")]
    public async Task DownloadAttachmentAsync_SendsCredentialsToTrustedTrelloHttpsHosts(string url)
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", httpClient: httpClient);

        using Stream stream = await client.DownloadAttachmentAsync(url, CancellationToken.None);

        RecordedRequest request = Assert.Single(handler.Requests);
        Assert.Equal("OAuth", request.AuthorizationScheme);
        Assert.Contains("oauth_consumer_key=\"key\"", request.AuthorizationParameter);
        Assert.Contains("oauth_token=\"token\"", request.AuthorizationParameter);
        Assert.Null(httpClient.DefaultRequestHeaders.Authorization);
    }

    [Theory]
    [InlineData("https://example.com/attachment.txt")]
    [InlineData("https://api.trello.com.attacker.example/attachment.txt")]
    [InlineData("http://api.trello.com/attachment.txt")]
    public async Task DownloadAttachmentAsync_DoesNotSendCredentialsToUntrustedUrl(string url)
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", httpClient: httpClient);

        using Stream stream = await client.DownloadAttachmentAsync(url, CancellationToken.None);

        RecordedRequest request = Assert.Single(handler.Requests);
        Assert.Null(request.AuthorizationScheme);
        Assert.Null(request.AuthorizationParameter);
    }

    [Theory]
    [InlineData("not a URL")]
    [InlineData("file:///temporary/attachment.txt")]
    [InlineData("ftp://api.trello.com/attachment.txt")]
    public async Task DownloadAttachmentAsync_RejectsInvalidOrUnsupportedUrl(string url)
    {
        RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", httpClient: httpClient);

        await Assert.ThrowsAsync<ArgumentException>(() => client.DownloadAttachmentAsync(url, CancellationToken.None));

        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task DownloadAttachmentAsync_ByAttachmentId_DoesNotSendCredentialsToLinkAttachmentUrl()
    {
        RecordingHandler handler = new RecordingHandler("https://attacker.example/attachment.txt");
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", httpClient: httpClient);

        using Stream stream = await client.DownloadAttachmentAsync("card", "attachment", CancellationToken.None);

        Assert.Equal(2, handler.Requests.Count);
        RecordedRequest downloadRequest = handler.Requests[1];
        Assert.Equal("attacker.example", downloadRequest.Uri.Host);
        Assert.Null(downloadRequest.AuthorizationScheme);
        Assert.Null(downloadRequest.AuthorizationParameter);
    }

    private sealed class RecordingHandler(string? attachmentUrl = null) : HttpMessageHandler
    {
        public List<RecordedRequest> Requests { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(new RecordedRequest(
                request.RequestUri!,
                request.Headers.Authorization?.Scheme,
                request.Headers.Authorization?.Parameter));

            string content = attachmentUrl != null && Requests.Count == 1
                ? $"{{\"id\":\"attachment\",\"url\":\"{attachmentUrl}\",\"isUpload\":false}}"
                : "attachment content";

            string mediaType = attachmentUrl != null && Requests.Count == 1 ? "application/json" : "text/plain";
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, Encoding.UTF8, mediaType)
            });
        }
    }

    private sealed record RecordedRequest(Uri Uri, string? AuthorizationScheme, string? AuthorizationParameter);
}

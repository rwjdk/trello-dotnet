using System.Net;
using System.Text;
using System.Text.Json;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Actions;

namespace TrelloDotNet.Tests.UnitTests;

/// <summary>
/// Tests comment request transport without calling Trello.
/// </summary>
public class CommentRequestBehaviorTests
{
    /// <summary>
    /// Verifies comment text is preserved in JSON and kept out of the URL for each authentication mode.
    /// </summary>
    [Theory]
    [InlineData(false, false, 0)]
    [InlineData(false, true, 0)]
    [InlineData(true, false, 0)]
    [InlineData(true, true, 0)]
    [InlineData(false, true, 1)]
    [InlineData(true, true, 1)]
    [InlineData(false, true, 2)]
    [InlineData(true, true, 2)]
    public async Task CommentTextUsesJsonBodyAsync(bool update, bool longText, int authenticationMode)
    {
        using RecordingHandler handler = new RecordingHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = authenticationMode == 2
            ? TrelloClient.FromOAuth2AccessToken("access-token", httpClient: httpClient)
            : new TrelloClient("key", "token", new TrelloClientOptions
            {
                SendCredentialsMode = authenticationMode == 1 ? SendCredentialsMode.Header : SendCredentialsMode.QueryString
            }, httpClient);
        const string specialText = "@member æøå 😀 & + % ? # \"quoted\"\r\n";
        string text = longText ? string.Concat(Enumerable.Repeat(specialText, 400)) : specialText;

        TrelloAction result = await SendCommentAsync(client, update, text, TestContext.Current.CancellationToken);

        Assert.Equal("commentId", result.Id);
        Assert.Equal(1, handler.Calls);
        Assert.Equal(update ? HttpMethod.Put : HttpMethod.Post, handler.Method);
        Assert.Equal(update ? "/1/actions/commentId" : "/1/cards/cardId/actions/comments", handler.Uri!.AbsolutePath);
        Assert.Equal(authenticationMode == 0 ? "?key=key&token=token" : string.Empty, handler.Uri.Query);
        Assert.True(handler.Uri.AbsoluteUri.Length < 200);
        Assert.Equal(authenticationMode == 2 ? "Bearer" : authenticationMode == 1 ? "OAuth" : null, handler.AuthorizationScheme);
        if (authenticationMode == 2)
        {
            Assert.Equal("access-token", handler.AuthorizationParameter);
        }
        else if (authenticationMode == 1)
        {
            Assert.Contains("oauth_consumer_key=\"key\"", handler.AuthorizationParameter);
            Assert.Contains("oauth_token=\"token\"", handler.AuthorizationParameter);
        }
        Assert.Equal("application/json", handler.MediaType);
        using JsonDocument body = JsonDocument.Parse(handler.Body!);
        Assert.Equal(text, body.RootElement.GetProperty("text").GetString());
        Assert.Single(body.RootElement.EnumerateObject());
    }

    /// <summary>
    /// Verifies rejected comments propagate the Trello error without retrying.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CommentFailurePropagatesAsync(bool update)
    {
        using RecordingHandler handler = new RecordingHandler { Status = HttpStatusCode.BadRequest };
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", httpClient: httpClient);

        TrelloApiException exception = await Assert.ThrowsAsync<TrelloApiException>(() =>
            SendCommentAsync(client, update, "text", TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.ErrorCode);
        Assert.Equal(1, handler.Calls);
    }

    /// <summary>
    /// Verifies cancellation reaches comment transport.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CommentCancellationReachesTransportAsync(bool update)
    {
        using CancellationTokenSource source = new CancellationTokenSource();
        using RecordingHandler handler = new RecordingHandler { OnSend = source.Cancel };
        using HttpClient httpClient = new HttpClient(handler);
        TrelloClient client = new TrelloClient("key", "token", httpClient: httpClient);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => SendCommentAsync(client, update, "text", source.Token));
        Assert.Equal(1, handler.Calls);
    }

    private static Task<TrelloAction> SendCommentAsync(TrelloClient client, bool update, string text, CancellationToken cancellationToken)
    {
        if (!update)
        {
            return client.AddCommentAsync("cardId", new Comment(text), cancellationToken);
        }

        TrelloAction action = JsonSerializer.Deserialize<TrelloAction>("{\"id\":\"commentId\",\"data\":{\"text\":\"\"}}")!;
        action.Data.Text = text;
        return client.UpdateCommentActionAsync(action, cancellationToken);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public int Calls { get; private set; }
        public HttpMethod? Method { get; private set; }
        public Uri? Uri { get; private set; }
        public string? Body { get; private set; }
        public string? MediaType { get; private set; }
        public string? AuthorizationScheme { get; private set; }
        public string? AuthorizationParameter { get; private set; }
        public HttpStatusCode Status { get; init; } = HttpStatusCode.OK;
        public Action? OnSend { get; init; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            Method = request.Method;
            Uri = request.RequestUri;
            Body = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            MediaType = request.Content?.Headers.ContentType?.MediaType;
            AuthorizationScheme = request.Headers.Authorization?.Scheme;
            AuthorizationParameter = request.Headers.Authorization?.Parameter;
            OnSend?.Invoke();
            cancellationToken.ThrowIfCancellationRequested();
            return new HttpResponseMessage(Status)
            {
                Content = new StringContent("{\"id\":\"commentId\"}", Encoding.UTF8, "application/json")
            };
        }
    }
}

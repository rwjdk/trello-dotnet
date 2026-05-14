using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class GenericRawCallOverloadTests
{
    [Fact]
    public async Task PostAsyncTypedWithoutCancellationTokenCallsEndpoint()
    {
        RecordingHandler handler = new RecordingHandler("""{"name":"Posted"}""");
        TrelloClient client = CreateClient(handler);

        RawResponse response = await client.PostAsync<RawResponse>("cards", new QueryParameter("name", "Card"));

        Assert.Equal("Posted", response.Name);
        Assert.Equal(HttpMethod.Post, handler.Requests.Single().Method);
        Assert.EndsWith("/1/cards?key=key&token=token&name=Card", handler.Requests.Single().RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task PostAsyncStringWithoutCancellationTokenCallsEndpoint()
    {
        RecordingHandler handler = new RecordingHandler("""{"ok":true}""");
        TrelloClient client = CreateClient(handler);

        string response = await client.PostAsync("cards", new QueryParameter("name", "Card"));

        Assert.Equal("""{"ok":true}""", response);
        Assert.Equal(HttpMethod.Post, handler.Requests.Single().Method);
        Assert.EndsWith("/1/cards?key=key&token=token&name=Card", handler.Requests.Single().RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task PutAsyncTypedWithoutCancellationTokenCallsEndpoint()
    {
        RecordingHandler handler = new RecordingHandler("""{"name":"Updated"}""");
        TrelloClient client = CreateClient(handler);

        RawResponse response = await client.PutAsync<RawResponse>("cards/card1", new QueryParameter("name", "Card"));

        Assert.Equal("Updated", response.Name);
        Assert.Equal(HttpMethod.Put, handler.Requests.Single().Method);
        Assert.EndsWith("/1/cards/card1?key=key&token=token&name=Card", handler.Requests.Single().RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task PutAsyncStringWithoutCancellationTokenCallsEndpoint()
    {
        RecordingHandler handler = new RecordingHandler("""{"ok":true}""");
        TrelloClient client = CreateClient(handler);

        string response = await client.PutAsync("cards/card1", new QueryParameter("name", "Card"));

        Assert.Equal("""{"ok":true}""", response);
        Assert.Equal(HttpMethod.Put, handler.Requests.Single().Method);
        Assert.EndsWith("/1/cards/card1?key=key&token=token&name=Card", handler.Requests.Single().RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task PutAsyncWithPayloadWithoutCancellationTokenSendsJsonBody()
    {
        RecordingHandler handler = new RecordingHandler("""{"ok":true}""");
        TrelloClient client = CreateClient(handler);

        string response = await client.PutAsync("cards/card1", """{"desc":"New"}""", new QueryParameter("name", "Card"));

        Assert.Equal("""{"ok":true}""", response);
        Assert.Equal(HttpMethod.Put, handler.Requests.Single().Method);
        Assert.Equal("""{"desc":"New"}""", handler.Bodies.Single());
        Assert.EndsWith("/1/cards/card1?key=key&token=token&name=Card", handler.Requests.Single().RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task GetAsyncTypedWithoutCancellationTokenCallsEndpoint()
    {
        RecordingHandler handler = new RecordingHandler("""{"name":"Fetched"}""");
        TrelloClient client = CreateClient(handler);

        RawResponse response = await client.GetAsync<RawResponse>("cards/card1", new QueryParameter("fields", "name"));

        Assert.Equal("Fetched", response.Name);
        Assert.Equal(HttpMethod.Get, handler.Requests.Single().Method);
        Assert.EndsWith("/1/cards/card1?key=key&token=token&fields=name", handler.Requests.Single().RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task GetAsyncStringWithoutCancellationTokenCallsEndpoint()
    {
        RecordingHandler handler = new RecordingHandler("""{"ok":true}""");
        TrelloClient client = CreateClient(handler);

        string response = await client.GetAsync("cards/card1", new QueryParameter("fields", "name"));

        Assert.Equal("""{"ok":true}""", response);
        Assert.Equal(HttpMethod.Get, handler.Requests.Single().Method);
        Assert.EndsWith("/1/cards/card1?key=key&token=token&fields=name", handler.Requests.Single().RequestUri!.AbsoluteUri);
    }

    private static TrelloClient CreateClient(HttpMessageHandler handler)
    {
        return new TrelloClient("key", "token", httpClient: new HttpClient(handler));
    }

    private sealed class RecordingHandler(string responseJson) : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = new();
        public List<string> Bodies { get; } = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            if (request.Content != null)
            {
                Bodies.Add(await request.Content.ReadAsStringAsync(cancellationToken));
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        }
    }

    private sealed class RawResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}

using System.Net;
using System.Text;

namespace TrelloDotNet.Tests.UnitTests;

public class EmojiTests
{
    [Fact]
    public async Task GetAvailableEmojiAsyncReturnsDeserializedEmoji()
    {
        const string json = """
                            {
                              "trello": [{
                                "unified": "1F600",
                                "name": "GRINNING FACE",
                                "native": "😀",
                                "shortName": "grinning",
                                "shortNames": ["grinning"],
                                "text": ":D",
                                "texts": [":-D"],
                                "category": "Smileys & People",
                                "sheetX": 30,
                                "sheetY": 24,
                                "skinVariation": null,
                                "tts": "grinning face",
                                "keywords": ["face", "grin"]
                              }]
                            }
                            """;
        RecordingHandler handler = new(json);
        TrelloClient client = new("key", "token", httpClient: new HttpClient(handler));

        List<Model.Emoji> response = await client.GetAvailableEmojiAsync(TestContext.Current.CancellationToken);

        HttpRequestMessage request = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.EndsWith("/1/emoji?key=key&token=token", request.RequestUri!.AbsoluteUri);
        Model.Emoji emoji = Assert.Single(response);
        Assert.Equal("1F600", emoji.UnifiedId);
        Assert.Equal("😀", emoji.Native);
        Assert.Equal(new[] { "face", "grin" }, emoji.Keywords);
    }

    private sealed class RecordingHandler(string responseJson) : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });
        }
    }
}

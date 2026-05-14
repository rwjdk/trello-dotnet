using System.Text.Json;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.IntegrationTests;

public class RawCallTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;

    [Fact]
    public async Task RawExceptionThrowsIncludeUrlButMaskCredentials()
    {
        try
        {
            TrelloClient.Options.ApiCallExceptionOption = ApiCallExceptionOption.IncludeUrlButMaskCredentials;
            await TrelloClient.GetAsync("xyz", cancellationToken: TestCancellationToken);
        }
        catch (TrelloApiException e)
        {
            //Empty
            Assert.Contains("XXXXXXXXXX", e.DataSentToTrello);
        }
    }

    [Fact]
    public async Task RawExceptionThrowsErrorWithoutUrl()
    {
        try
        {
            TrelloClient.Options.ApiCallExceptionOption = ApiCallExceptionOption.DoNotIncludeTheUrl;
            await TrelloClient.GetAsync("xyz", cancellationToken: TestCancellationToken);
        }
        catch (TrelloApiException e)
        {
            //Empty
            Assert.Equal("XXXXX", e.DataSentToTrello);
        }
    }

    [Fact]
    public async Task RawExceptionThrowsErrorWithUrlAndCredentials()
    {
        try
        {
            TrelloClient.Options.ApiCallExceptionOption = ApiCallExceptionOption.IncludeUrlAndCredentials;
            await TrelloClient.GetAsync("xyz", cancellationToken: TestCancellationToken);
        }
        catch (TrelloApiException e)
        {
            //Empty
            Assert.DoesNotContain("XXXXXXXXXX", e.DataSentToTrello);
        }
    }

    [Fact]
    public async Task RawExceptionsThrowCorrectException()
    {
        await Assert.ThrowsAsync<TrelloApiException>(async () => await TrelloClient.GetAsync("xyz", cancellationToken: TestCancellationToken));
        await Assert.ThrowsAsync<TrelloApiException>(async () => await TrelloClient.PostAsync("xyz", cancellationToken: TestCancellationToken));
        await Assert.ThrowsAsync<TrelloApiException>(async () => await TrelloClient.PutAsync("xyz", cancellationToken: TestCancellationToken));
        await Assert.ThrowsAsync<TrelloApiException>(async () => await TrelloClient.DeleteAsync("xyz", cancellationToken: TestCancellationToken));
    }

    [Fact]
    public async Task RawGet()
    {
        //Raw JSON
        string? rawGet = await TrelloClient.GetAsync($"boards/{_board.Id}", cancellationToken: TestCancellationToken);
        Assert.NotNull(rawGet);

        //Raw
        Board? rawGetBoard = await TrelloClient.GetAsync<Board>($"boards/{_board.Id}", cancellationToken: TestCancellationToken);
        Assert.Equal(_board.Id, rawGetBoard.Id);
    }

    [Fact]
    public async Task RawGetWithoutCancellationTokenOverload()
    {
        string? rawGet = await TrelloClient.GetAsync($"boards/{_board.Id}");
        Assert.NotNull(rawGet);

        Board? rawGetBoard = await TrelloClient.GetAsync<Board>($"boards/{_board.Id}");
        Assert.Equal(_board.Id, rawGetBoard.Id);
    }

    [Fact]
    public async Task RawPost()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        string? rawPost = await TrelloClient.PostAsync("cards", TestCancellationToken, new QueryParameter("name", "Card"), new QueryParameter("idList", list.Id));
        Assert.NotNull(rawPost);

        Card? rawPostCard = await TrelloClient.PostAsync<Card>("cards", TestCancellationToken, new QueryParameter("name", "Card"), new QueryParameter("idList", list.Id));
        Assert.NotNull(rawPostCard.Id);
    }

    [Fact]
    public async Task RawPostWithoutCancellationTokenOverload()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);

        string? rawPost = await TrelloClient.PostAsync("cards", new QueryParameter("name", "Card"), new QueryParameter("idList", list.Id));
        Assert.NotNull(rawPost);

        Card? rawPostCard = await TrelloClient.PostAsync<Card>("cards", new QueryParameter("name", "Card"), new QueryParameter("idList", list.Id));
        Assert.NotNull(rawPostCard.Id);
    }

    [Fact]
    public async Task RawPut()
    {
        Card card = await AddDummyCard(_board.Id, "RawPut");

        string? rawUpdate = await TrelloClient.PutAsync($"cards/{card.Id}", TestCancellationToken, new QueryParameter("desc", "New Description"));
        Assert.NotNull(rawUpdate);

        Card? rawUpdateCard = await TrelloClient.PutAsync<Card>($"cards/{card.Id}", TestCancellationToken, new QueryParameter("desc", "New Description2"));
        Assert.Equal("New Description2", rawUpdateCard.Description);
    }

    [Fact]
    public async Task RawPutWithoutCancellationTokenOverload()
    {
        Card card = await AddDummyCard(_board.Id, "RawPut");

        string? rawUpdate = await TrelloClient.PutAsync($"cards/{card.Id}", new QueryParameter("desc", "New Description"));
        Assert.NotNull(rawUpdate);

        Card? rawUpdateCard = await TrelloClient.PutAsync<Card>($"cards/{card.Id}", new QueryParameter("desc", "New Description2"));
        Assert.Equal("New Description2", rawUpdateCard.Description);
    }

    [Fact]
    public async Task RawPutWithPayload()
    {
        Card card = await AddDummyCard(_board.Id, "RawPut");

        CardCover coverToAdd = new(CardCoverColor.Black, CardCoverSize.Full);
        coverToAdd.PrepareForAddUpdate();
        string payload = $"{{\"cover\":{JsonSerializer.Serialize(coverToAdd)}}}";

        string? rawUpdate = await TrelloClient.PutAsync($"{UrlPaths.Cards}/{card.Id}", payload, cancellationToken: TestCancellationToken);
        Assert.NotNull(rawUpdate);
    }

    [Fact]
    public async Task RawPutWithPayloadWithoutCancellationTokenOverload()
    {
        Card card = await AddDummyCard(_board.Id, "RawPut");

        CardCover coverToAdd = new(CardCoverColor.Black, CardCoverSize.Full);
        coverToAdd.PrepareForAddUpdate();
        string payload = $"{{\"cover\":{JsonSerializer.Serialize(coverToAdd)}}}";

        string? rawUpdate = await TrelloClient.PutAsync($"{UrlPaths.Cards}/{card.Id}", payload);
        Assert.NotNull(rawUpdate);
    }
}

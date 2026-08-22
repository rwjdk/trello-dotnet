using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.IntegrationTests;

public class PluginDataTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;

    [Fact]
    public async Task GetPluginDataOnCard()
    {
        List list = await AddDummyList(_board.Id);
        Card card = await AddDummyCardToList(list);

        List<PluginData>? pluginData = await TrelloClient.GetPluginDataOnCardAsync(card.Id, cancellationToken: TestCancellationToken);

        Assert.NotNull(pluginData);
        Assert.Empty(pluginData);
    }

    [Fact]
    public async Task GetPluginDataOnCardWithPluginId()
    {
        List list = await AddDummyList(_board.Id);
        Card card = await AddDummyCardToList(list);
        const string pluginId = "com.example.plugin";

        PluginData pluginData = await TrelloClient.GetPluginDataOnCardAsync(card.Id, pluginId, cancellationToken: TestCancellationToken);

        Assert.Null(pluginData);
    }

    [Fact]
    public async Task GetPluginDataOnCardWithGenericType()
    {
        List list = await AddDummyList(_board.Id);
        Card card = await AddDummyCardToList(list);
        const string pluginId = "com.example.plugin";

        DummyPlugin? pluginData = await TrelloClient.GetPluginDataOnCardAsync<DummyPlugin>(card.Id, pluginId, cancellationToken: TestCancellationToken);

        Assert.Null(pluginData);
    }

    [Fact]
    public async Task GetPluginDataOnBoard()
    {
        List<PluginData>? pluginData = await TrelloClient.GetPluginDataOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);

        Assert.NotNull(pluginData);
        Assert.Empty(pluginData);
    }

    [Fact]
    public async Task GetPluginDataOnBoardWithPluginId()
    {
        const string pluginId = "com.example.plugin";

        PluginData? pluginData = await TrelloClient.GetPluginDataOfBoardAsync(_board.Id, pluginId, cancellationToken: TestCancellationToken);

        Assert.Null(pluginData);
    }

    [Fact]
    public async Task GetPluginDataOnBoardWithGenericType()
    {
        const string pluginId = "com.example.plugin";

        DummyPlugin pluginData = await TrelloClient.GetPluginDataOfBoardAsync<DummyPlugin>(_board.Id, pluginId, cancellationToken: TestCancellationToken);

        Assert.Null(pluginData);
    }
}

public class DummyPlugin
{
    //Empty
}

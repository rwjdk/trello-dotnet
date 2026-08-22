using TrelloDotNet.Model;
using TrelloDotNet.Model.Actions;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Options.GetActionsOptions;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.IntegrationTests;

public class ActionTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly string? _boardId = fixture.BoardId;
    private readonly Board _board = fixture.Board!;
    private readonly Organization _organization = fixture.Organization!;

    [Fact]
    public async Task GetActionsOfBoard()
    {
        string nameBefore = _board.Name;
        string newName = _board.Name + "GetActionsOfBoard";
        _board.Name = newName;
        await TrelloClient.UpdateBoardAsync(_board);
        List<TrelloAction> actions = await TrelloClient.GetActionsOfBoardAsync(_boardId, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateBoard]
        });
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateBoard && x.Data.Board.Name == newName && x.Data.Old.Name == nameBefore);
    }

    [Fact]
    public async Task GetActionsForOrganizations()
    {
        string nameBefore = _organization.DisplayName;
        string newName = _organization.DisplayName + "GetActionsForOrganizations";
        _organization.DisplayName = newName;
        await TrelloClient.UpdateOrganizationAsync(_organization);
        List<TrelloAction> actions = await TrelloClient.GetActionsForOrganizationsAsync(_organization.Id, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateOrganization]
        });
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateOrganization && x.Data.Organization.Name == newName && x.Data.Old.DisplayName == nameBefore);
    }

    [Fact]
    public async Task GetActionsOfCard()
    {
        const string testName = "GetActionsOfCard";
        List list = await TrelloClient.AddListAsync(new List(testName, _boardId));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, testName));
        const string newName = testName + "X";
        await TrelloClient.UpdateCardAsync(card.Id, [
            CardUpdate.Name(newName),
        ]);
        List<TrelloAction> actions = await TrelloClient.GetActionsOnCardAsync(card.Id, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateCard]
        });
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateCard && x.Data.Card.Id == card.Id && x.Data.Card.Name == newName && x.Data.Old.Name == testName);
    }

    [Fact]
    public async Task GetActionsForList()
    {
        const string testName = "GetActionsOfList";
        List list = await TrelloClient.AddListAsync(new List(testName, _boardId));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, testName));
        const string newName = testName + "X";
        await TrelloClient.UpdateCardAsync(card.Id, [CardUpdate.Name(newName)]);
        List<TrelloAction> actions = await TrelloClient.GetActionsForListAsync(list.Id, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateCard]
        });
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateCard && x.Data.Card.Id == card.Id && x.Data.Card.Name == newName && x.Data.Old.Name == testName);
    }

    [Fact]
    public async Task GetActionsForMember()
    {
        const string testName = "GetActionsForMember";
        List list = await TrelloClient.AddListAsync(new List(testName, _boardId));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, testName));
        const string newName = testName + "X";
        await TrelloClient.UpdateCardAsync(card.Id, [CardUpdate.Name(newName)]);
        Member member = await TrelloClient.GetTokenMemberAsync();
        List<TrelloAction> actions = await TrelloClient.GetActionsForMemberAsync(member.Id, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateCard]
        });
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateCard && x.Data.Card.Id == card.Id && x.Data.Card.Name == newName && x.Data.Old.Name == testName);
    }
}
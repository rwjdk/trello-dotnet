using TrelloDotNet.Model;
using TrelloDotNet.Model.Actions;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Options.GetActionsOptions;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.IntegrationTests;

public class ActionTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly string? _boardId = fixture.BoardId;
    private readonly Board _board = fixture.Board!;
    private readonly Organization _organization = fixture.Organization!;

    [Fact]
    public async Task GetActionsOfBoard()
    {
        string? nameBefore = _board.Name;
        string newName = _board.Name + "GetActionsOfBoard";
        await TrelloClient.UpdateBoardAsync(_board.Id, [BoardUpdate.Name(newName)], cancellationToken: TestCancellationToken);
        List<TrelloAction>? actions = await TrelloClient.GetActionsOfBoardAsync(_boardId, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateBoard]
        }, cancellationToken: TestCancellationToken);
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateBoard && x.Data.Board.Name == newName && x.Data.Old.Name == nameBefore);
    }

    [Fact]
    public async Task GetActionsForOrganizations()
    {
        string? nameBefore = _organization.DisplayName;
        string newName = _organization.DisplayName + "GetActionsForOrganizations";
        _organization.DisplayName = newName;
        await TrelloClient.UpdateOrganizationAsync(_organization, cancellationToken: TestCancellationToken);
        List<TrelloAction>? actions = await TrelloClient.GetActionsForOrganizationsAsync(_organization.Id, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateOrganization]
        }, cancellationToken: TestCancellationToken);
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateOrganization && x.Data.Organization.Name == newName && x.Data.Old.DisplayName == nameBefore);
    }

    [Fact]
    public async Task GetActionsOfCard()
    {
        const string testName = "GetActionsOfCard";
        List list = await TrelloClient.AddListAsync(new List(testName, _boardId), cancellationToken: TestCancellationToken);
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, testName), cancellationToken: TestCancellationToken);
        const string newName = testName + "X";
        await TrelloClient.UpdateCardAsync(card.Id, [
            CardUpdate.Name(newName),
        ], cancellationToken: TestCancellationToken);
        List<TrelloAction>? actions = await TrelloClient.GetActionsOnCardAsync(card.Id, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateCard]
        }, cancellationToken: TestCancellationToken);
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateCard && x.Data.Card.Id == card.Id && x.Data.Card.Name == newName && x.Data.Old.Name == testName);
    }

    [Fact]
    public async Task GetActionsForList()
    {
        const string testName = "GetActionsOfList";
        List list = await TrelloClient.AddListAsync(new List(testName, _boardId), cancellationToken: TestCancellationToken);
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, testName), cancellationToken: TestCancellationToken);
        const string newName = testName + "X";
        await TrelloClient.UpdateCardAsync(card.Id, [CardUpdate.Name(newName)], cancellationToken: TestCancellationToken);
        List<TrelloAction>? actions = await TrelloClient.GetActionsForListAsync(list.Id, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateCard]
        }, cancellationToken: TestCancellationToken);
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateCard && x.Data.Card.Id == card.Id && x.Data.Card.Name == newName && x.Data.Old.Name == testName);
    }

    [Fact]
    public async Task GetActionsForMember()
    {
        const string testName = "GetActionsForMember";
        List list = await TrelloClient.AddListAsync(new List(testName, _boardId), cancellationToken: TestCancellationToken);
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, testName), cancellationToken: TestCancellationToken);
        const string newName = testName + "X";
        await TrelloClient.UpdateCardAsync(card.Id, [CardUpdate.Name(newName)], cancellationToken: TestCancellationToken);
        Member member = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);
        List<TrelloAction>? actions = await TrelloClient.GetActionsForMemberAsync(member.Id, new GetActionsOptions
        {
            Filter = [WebhookActionTypes.UpdateCard]
        }, cancellationToken: TestCancellationToken);
        Assert.Contains(actions, x => x.Type == WebhookActionTypes.UpdateCard && x.Data.Card.Id == card.Id && x.Data.Card.Name == newName && x.Data.Old.Name == testName);
    }
}

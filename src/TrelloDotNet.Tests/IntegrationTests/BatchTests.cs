using TrelloDotNet.Model;
using TrelloDotNet.Model.Batch;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetBoardOptions;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Options.GetListOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class BatchTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly string _boardId = fixture.BoardId!;

    [Fact]
    public async Task ExecuteBatchedRequestAsync()
    {
        Board? b = null;
        (List list, Card card) = await AddDummyCardAndList(_boardId);
        Member member = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);

        await TrelloClient.ExecuteBatchedRequestAsync(
        [
            BatchRequest.GetBoard(_boardId, result => b = result.GetData<Board>()),
            BatchRequest.GetBoard(_boardId, new GetBoardOptions(), result => b = result.GetData<Board>()),
            BatchRequest.GetActionsForList(list.Id, _ => Console.Write("")),
            BatchRequest.GetActionsForMember(member.Id, _ => Console.Write("")),
            BatchRequest.GetActionsForOrganization(fixture.OrganizationId, _ => Console.Write("")),
            BatchRequest.GetActionsOnBoard(_boardId, _ => Console.Write("")),
            BatchRequest.GetActionsOnCard(card.Id, _ => Console.Write("")),
            BatchRequest.GetAttachmentsOnCard(card.Id, _ => Console.Write("")),
            BatchRequest.GetBoardsForMember(member.Id, _ => Console.Write("")),
            BatchRequest.GetBoardsForMember(member.Id, new GetBoardOptions(), _ => Console.Write("")),
            BatchRequest.GetBoardsInOrganization(fixture.OrganizationId, _ => Console.Write("")),
            BatchRequest.GetBoardsInOrganization(fixture.OrganizationId, new GetBoardOptions(), _ => Console.Write("")),
            BatchRequest.GetCard(card.Id, _ => Console.Write("")),
            BatchRequest.GetCard(card.Id, new GetCardOptions(), _ => Console.Write("")),
            BatchRequest.GetCardsOnBoard(_boardId, _ => Console.Write("")),
            BatchRequest.GetCardsOnBoard(_boardId, new GetCardOptions(), _ => Console.Write("")),
            BatchRequest.GetActionsForList(list.Id, _ => Console.Write("")),
            BatchRequest.GetCardsInList(list.Id, _ => Console.Write("")),
            BatchRequest.GetCardsInList(list.Id, new GetCardOptions(), _ => Console.Write("")),
            BatchRequest.GetCardsForMember(member.Id, _ => Console.Write("")),
            BatchRequest.GetCardsForMember(member.Id, new GetCardOptions(), _ => Console.Write("")),
            BatchRequest.GetChecklistsOnBoard(_boardId, _ => Console.Write("")),
            BatchRequest.GetChecklistsOnCard(card.Id, _ => Console.Write("")),
            BatchRequest.GetLabelsOfBoard(_boardId, _ => Console.Write("")),
            BatchRequest.GetCustomFieldItemsForCard(card.Id, _ => Console.Write("")),
            BatchRequest.GetCustomFieldsOnBoard(_boardId, _ => Console.Write("")),
            BatchRequest.GetList(list.Id, _ => Console.Write("")),
            BatchRequest.GetListsOnBoard(_boardId, _ => Console.Write("")),
            BatchRequest.GetMember(member.Id, _ => Console.Write("")),
            BatchRequest.GetMembersOfBoard(_boardId, _ => Console.Write("")),
            BatchRequest.GetMembersOfCard(card.Id, _ => Console.Write("")),
            BatchRequest.GetMembersOfOrganization(fixture.OrganizationId, _ => Console.Write("")),
            BatchRequest.GetMembershipsOfBoard(_boardId, _ => Console.Write("")),
            BatchRequest.GetOrganization(fixture.OrganizationId, _ => Console.Write("")),
            BatchRequest.GetOrganizationsForMember(member.Id, _ => Console.Write("")),
            BatchRequest.GetStickersOnCard(card.Id, _ => Console.Write("")),
        ], cancellationToken: TestCancellationToken);

        Assert.NotNull(b);
    }

    [Fact]
    public async Task GetOrganizations()
    {
        List<Organization>? data = await TrelloClient.GetOrganizationsAsync([fixture.OrganizationId!], cancellationToken: TestCancellationToken);
        Assert.NotEmpty(data);
    }

    [Fact]
    public async Task GetNoneExistOrganizations()
    {
        await Assert.ThrowsAsync<TrelloApiException>(async () =>
        {
            List<Organization>? data = await TrelloClient.GetOrganizationsAsync(["non-Exist"], cancellationToken: TestCancellationToken);
            Assert.NotEmpty(data);
        });
    }

    [Fact]
    public async Task GetListsAndCards()
    {
        (List list, Card card) = await AddDummyCardAndList(_boardId);
        // ReSharper disable once RedundantAssignment
        List<List>? listData = await TrelloClient.GetListsAsync([list.Id], cancellationToken: TestCancellationToken);
        listData = await TrelloClient.GetListsAsync([list.Id], new GetListOptions
        {
            IncludeBoard = true,
            IncludeCards = GetListOptionsIncludeCards.All
        }, cancellationToken: TestCancellationToken);
        Assert.NotEmpty(listData);
        Assert.Equal(list.Id, listData[0].Id);
        Assert.Equal(list.Name, listData[0].Name);
        Assert.Equal(list.BoardId, listData[0].BoardId);
        Assert.Equal(list.Closed, listData[0].Closed);
        Assert.Equal(list.Position, listData[0].Position);

        List<Card>? data = await TrelloClient.GetCardsAsync([card.Id], cancellationToken: TestCancellationToken);
        Assert.NotEmpty(data);
        Assert.Equal(card.Id, data[0].Id);
        Assert.Equal(card.Name, data[0].Name);
        Assert.Equal(card.ListId, data[0].ListId);
        Assert.Equal(card.BoardId, data[0].BoardId);
        Assert.Equal(card.Description, data[0].Description);
        Assert.Equal(card.Closed, data[0].Closed);
        Assert.Equal(card.Position, data[0].Position);
        Assert.Equal(card.Due, data[0].Due);
        Assert.Equal(card.DueComplete, data[0].DueComplete);
        Assert.Equal(card.Start, data[0].Start);
        Assert.Equal(card.MemberIds, data[0].MemberIds);
        Assert.Equal(card.LabelIds, data[0].LabelIds);

        data = await TrelloClient.GetCardsAsync([card.Id], new GetCardOptions
        {
            CardFields = new CardFields(CardFieldsType.Name)
        }, cancellationToken: TestCancellationToken);
        Assert.NotEmpty(data);
        Assert.Equal(card.Id, data[0].Id);
        Assert.Equal(card.Name, data[0].Name);
        Assert.Null(data[0].Description);
        Assert.Null(data[0].Due);
        Assert.Null(data[0].Start);
    }

    [Fact]
    public async Task GetCardsWithNullOptionsUsesSimpleBatchPath()
    {
        (List list, Card card1) = await AddDummyCardAndList(_boardId);
        Card card2 = await AddDummyCardToList(list);

        List<Card>? data = await TrelloClient.GetCardsAsync([card1.Id, card2.Id], (GetCardOptions)null!, cancellationToken: TestCancellationToken);

        Assert.Equal(2, data.Count);
        Assert.Contains(data, x => x.Id == card1.Id);
        Assert.Contains(data, x => x.Id == card2.Id);
    }

    [Fact]
    public async Task GetBoardsWithNullOptionsUsesSimpleBatchPath()
    {
        List<Board>? data = await TrelloClient.GetBoardsAsync([fixture.BoardId!], (GetBoardOptions)null!, cancellationToken: TestCancellationToken);

        Assert.Single(data);
        Assert.Equal(fixture.BoardId, data[0].Id);
    }

    [Fact]
    public async Task ExecuteBatchedRequestAsyncHandlesMoreThanTenRequests()
    {
        int actionCount = 0;
        List<BatchRequest> requests = Enumerable.Range(0, 11)
            .Select(_ => BatchRequest.GetBoard(_boardId, result =>
            {
                Assert.Equal(_boardId, result.GetData<Board>().Id);
                actionCount++;
            }))
            .ToList();

        await TrelloClient.ExecuteBatchedRequestAsync(requests, TestCancellationToken);

        Assert.Equal(11, actionCount);
    }


    [Fact]
    public async Task GetMembers()
    {
        Member member = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);
        List<Member>? data = await TrelloClient.GetMembersAsync([member.Id], cancellationToken: TestCancellationToken);
        Assert.NotEmpty(data);
    }

    [Fact]
    public async Task GetBoards()
    {
        List<Board>? data = await TrelloClient.GetBoardsAsync([fixture.BoardId!], cancellationToken: TestCancellationToken);
        Assert.NotEmpty(data);

        data = await TrelloClient.GetBoardsAsync([fixture.BoardId!], new GetBoardOptions
        {
            IncludeLabels = true
        }, cancellationToken: TestCancellationToken);
        Assert.NotEmpty(data);
    }
}

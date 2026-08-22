using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Options.GetListOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class ListTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly string _boardId = fixture.BoardId!;

    [Fact]
    public async Task AddList()
    {
        string name = Guid.NewGuid().ToString();
        List? addList = await TrelloClient.AddListAsync(new List(name, _boardId), cancellationToken: TestCancellationToken);
        AssertTimeIsNow(addList.Created);
        Assert.False(addList.Closed);
        Assert.Equal(name, addList.Name);
        Assert.False(addList.Subscribed);
        Assert.Null(addList.SoftLimit);
        List<List>? listsAfter = await TrelloClient.GetListsOnBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        List? foundList = listsAfter.FirstOrDefault(x => x.Id == addList.Id);
        Assert.NotNull(foundList);
        Assert.Equal(name, foundList.Name);
    }

    [Fact]
    public async Task UpdateList()
    {
        string name = Guid.NewGuid().ToString();
        List? addList = await TrelloClient.AddListAsync(new List(name, _boardId), cancellationToken: TestCancellationToken);
        string updatedName = Guid.NewGuid().ToString();
        List? updateList = await TrelloClient.UpdateListAsync(addList.Id, [
            ListUpdate.Name(updatedName)
        ], cancellationToken: TestCancellationToken);
        List? getList = await TrelloClient.GetListAsync(addList.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(updatedName, getList.Name);
        Assert.Equal(updateList.Name, getList.Name);
    }

    [Fact]
    public async Task ArchiveAndReopenList()
    {
        string name = Guid.NewGuid().ToString();
        List? addList = await TrelloClient.AddListAsync(new List(name, _boardId), cancellationToken: TestCancellationToken);

        //Archive
        List? archivedList = await TrelloClient.ArchiveListAsync(addList.Id, cancellationToken: TestCancellationToken);
        Assert.True(archivedList.Closed);
        List<List>? listsAfter = await TrelloClient.GetListsOnBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        Assert.True(listsAfter.TrueForAll(x => x.Id != addList.Id));
        Assert.True(listsAfter.TrueForAll(x => x.Name != name));

        //Check that there are a closed list
        List<List>? closedLists = await TrelloClient.GetListsOnBoardAsync(_boardId, new GetListOptions
        {
            Filter = ListFilter.Closed,
            BoardFields = new BoardFields(BoardFieldsType.Closed),
            AdditionalParameters = [new QueryParameter("x", "y")],
            CardFields = new CardFields(CardFieldsType.Name),
            CardsFilterConditions = [CardsFilterCondition.Name(CardsConditionString.NotEqual, Guid.NewGuid().ToString()),],
            CardsOrderBy = CardsOrderBy.CreateDateDesc,
            IncludeBoard = true,
            IncludeCards = GetListOptionsIncludeCards.All
        }, cancellationToken: TestCancellationToken);
        List foundList = closedLists.Single(x => x.Id == addList.Id);
        Assert.Equal(addList.Name, foundList.Name);

        //Re-open
        List? reopenedList = await TrelloClient.ReOpenListAsync(foundList.Id, cancellationToken: TestCancellationToken);
        Assert.False(reopenedList.Closed);
        Assert.Equal(addList.Id, reopenedList.Id);
        Assert.Equal(name, reopenedList.Name);

        List<List>? listsAfterReopen = await TrelloClient.GetListsOnBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        Assert.Contains(listsAfterReopen, x => x.Id == reopenedList.Id);
        Assert.Contains(listsAfterReopen, x => x.Name == name);
    }

    [Fact]
    public async Task DeleteList()
    {
        List<List>? listsBefore = await TrelloClient.GetListsOnBoardAsync(_boardId, new GetListOptions
        {
            Filter = ListFilter.All
        }, cancellationToken: TestCancellationToken);
        string name = Guid.NewGuid().ToString();
        List? addList = await TrelloClient.AddListAsync(new List(name, _boardId), cancellationToken: TestCancellationToken);

        //Delete
        await TrelloClient.DeleteListAsync(addList.Id, cancellationToken: TestCancellationToken);
        List<List>? listsAfter = await TrelloClient.GetListsOnBoardAsync(_boardId, new GetListOptions
        {
            Filter = ListFilter.All
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(listsAfter.Count, listsBefore.Count);
        Assert.Contains(listsAfter, x => x.Name != name);
    }

    [Fact]
    public async Task ArchiveAllCardsInList()
    {
        string name = Guid.NewGuid().ToString();
        List? addList = await TrelloClient.AddListAsync(new List(name, _boardId), cancellationToken: TestCancellationToken);
        //Add some cards so we can test Archive All Cards In List
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C1"), cancellationToken: TestCancellationToken);
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C2"), cancellationToken: TestCancellationToken);
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C3"), cancellationToken: TestCancellationToken);
        List<Card>? cardsOnListAfterAdd = await TrelloClient.GetCardsInListAsync(addList.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cardsOnListAfterAdd.Count);
        await TrelloClient.ArchiveAllCardsInListAsync(addList.Id, cancellationToken: TestCancellationToken);
        List<Card>? cardsOnListAfterArchive = await TrelloClient.GetCardsInListAsync(addList.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(cardsOnListAfterArchive);
    }

    [Fact]
    public async Task MoveCardToList()
    {
        List? sourceList = await TrelloClient.AddListAsync(new List("Source", _boardId), cancellationToken: TestCancellationToken);
        List? targetList = await TrelloClient.AddListAsync(new List("Target", _boardId), cancellationToken: TestCancellationToken);

        Card card1 = await TrelloClient.AddCardAsync(new AddCardOptions(sourceList.Id, "C1"), cancellationToken: TestCancellationToken);
        Card card2 = await TrelloClient.AddCardAsync(new AddCardOptions(sourceList.Id, "C2"), cancellationToken: TestCancellationToken);
        Card card3 = await TrelloClient.AddCardAsync(new AddCardOptions(sourceList.Id, "C3"), cancellationToken: TestCancellationToken);

        await TrelloClient.MoveCardToListAsync(card2.Id, targetList.Id, cancellationToken: TestCancellationToken);

        List<Card>? sourceAfter = await TrelloClient.GetCardsInListAsync(sourceList.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(2, sourceAfter.Count);
        Assert.Contains(sourceAfter, x => x.Id == card1.Id);
        Assert.Contains(sourceAfter, x => x.Id == card3.Id);


        List<Card>? targetAfter = await TrelloClient.GetCardsInListAsync(targetList.Id, cancellationToken: TestCancellationToken);
        Assert.Single(targetAfter);
        Assert.Contains(targetAfter, x => x.Id == card2.Id);
    }

    [Fact]
    public async Task MoveAllCardsInList()
    {
        string name = Guid.NewGuid().ToString();
        List? addList = await TrelloClient.AddListAsync(new List(name, _boardId), cancellationToken: TestCancellationToken);
        //Add some cards so we can test Move All Cards In List
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C1"), cancellationToken: TestCancellationToken);
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C2"), cancellationToken: TestCancellationToken);
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C3"), cancellationToken: TestCancellationToken);

        //Add new list to move cards to
        List? listToMoveTo = await TrelloClient.AddListAsync(new List("List to move to", _boardId), cancellationToken: TestCancellationToken);
        await TrelloClient.MoveAllCardsInListAsync(addList.Id, listToMoveTo.Id, cancellationToken: TestCancellationToken);
        List<Card>? cardsOnListAfterMove = await TrelloClient.GetCardsInListAsync(listToMoveTo.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cardsOnListAfterMove.Count);
    }

    [Fact]
    public async Task GetCardsInList()
    {
        List list = await AddDummyList(_boardId);

        await AddDummyCardToList(list, "Card 1");
        await AddDummyCardToList(list, "Card 2");
        await AddDummyCardToList(list, "Card 3");

        List<Card>? cards = await TrelloClient.GetCardsInListAsync(list.Id, new GetCardOptions
        {
            IncludeBoard = true,
            IncludeList = true,
            ActionsTypes = ActionTypesToInclude.All,
            Limit = 10,
            CardFields = new CardFields(CardFieldsType.Name)
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(3, cards.Count);
    }

    [Fact]
    public async Task GetListWithOptions()
    {
        List list = await AddDummyList(_boardId);
        Card card = await AddDummyCardToList(list);

        List? listWithOptions = await TrelloClient.GetListAsync(list.Id, new GetListOptions
        {
            IncludeBoard = true,
            IncludeCards = GetListOptionsIncludeCards.All
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(list.Id, listWithOptions.Id);
        Assert.Equal(list.Name, listWithOptions.Name);
        Assert.False(listWithOptions.Closed);
        Assert.NotEmpty(listWithOptions.Cards);
        Assert.Contains(listWithOptions.Cards, c => c.Id == card.Id);
    }
}

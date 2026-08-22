using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Options.GetListOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class ListTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly string? _boardId = fixture.BoardId;

    [Fact]
    public async Task AddList()
    {
        string name = Guid.NewGuid().ToString();
        List addList = await TrelloClient.AddListAsync(new List(name, _boardId));
        AssertTimeIsNow(addList.Created);
        Assert.False(addList.Closed);
        Assert.Equal(name, addList.Name);
        Assert.False(addList.Subscribed);
        Assert.Null(addList.SoftLimit);
        List<List> listsAfter = await TrelloClient.GetListsOnBoardAsync(_boardId);
        List? foundList = listsAfter.FirstOrDefault(x => x.Id == addList.Id);
        Assert.NotNull(foundList);
        Assert.Equal(name, foundList.Name);
    }

    [Fact]
    public async Task UpdateList()
    {
        string name = Guid.NewGuid().ToString();
        List addList = await TrelloClient.AddListAsync(new List(name, _boardId));
        string updatedName = Guid.NewGuid().ToString();
        addList.Name = updatedName;
        List updateList = await TrelloClient.UpdateListAsync(addList);
        List getList = await TrelloClient.GetListAsync(addList.Id);
        Assert.Equal(updatedName, getList.Name);
        Assert.Equal(updateList.Name, getList.Name);
    }

    [Fact]
    public async Task ArchiveAndReopenList()
    {
        string name = Guid.NewGuid().ToString();
        List addList = await TrelloClient.AddListAsync(new List(name, _boardId));

        //Archive
        List archivedList = await TrelloClient.ArchiveListAsync(addList.Id);
        Assert.True(archivedList.Closed);
        List<List> listsAfter = await TrelloClient.GetListsOnBoardAsync(_boardId);
        Assert.True(listsAfter.TrueForAll(x => x.Id != addList.Id));
        Assert.True(listsAfter.TrueForAll(x => x.Name != name));

        //Check that there are a closed list
        List<List> closedLists = await TrelloClient.GetListsOnBoardAsync(_boardId, new GetListOptions
        {
            Filter = ListFilter.Closed
        });
        List foundList = closedLists.Single(x => x.Id == addList.Id);
        Assert.Equal(addList.Name, foundList.Name);

        //Re-open
        List reopenedList = await TrelloClient.ReOpenListAsync(foundList.Id);
        Assert.False(reopenedList.Closed);
        Assert.Equal(addList.Id, reopenedList.Id);
        Assert.Equal(name, reopenedList.Name);

        List<List> listsAfterReopen = await TrelloClient.GetListsOnBoardAsync(_boardId);
        Assert.Contains(listsAfterReopen, x => x.Id == reopenedList.Id);
        Assert.Contains(listsAfterReopen, x => x.Name == name);
    }

    [Fact]
    public async Task ArchiveAllCardsInList()
    {
        string name = Guid.NewGuid().ToString();
        List addList = await TrelloClient.AddListAsync(new List(name, _boardId));
        //Add some cards so we can test Archive All Cards In List
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C1"));
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C2"));
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C3"));
        List<Card> cardsOnListAfterAdd = await TrelloClient.GetCardsInListAsync(addList.Id);
        Assert.Equal(3, cardsOnListAfterAdd.Count);
        await TrelloClient.ArchiveAllCardsInListAsync(addList.Id);
        List<Card> cardsOnListAfterArchive = await TrelloClient.GetCardsInListAsync(addList.Id);
        Assert.Empty(cardsOnListAfterArchive);
    }

    [Fact]
    public async Task MoveCardToList()
    {
        List sourceList = await TrelloClient.AddListAsync(new List("Source", _boardId));
        List targetList = await TrelloClient.AddListAsync(new List("Target", _boardId));

        Card card1 = await TrelloClient.AddCardAsync(new AddCardOptions(sourceList.Id, "C1"));
        Card card2 = await TrelloClient.AddCardAsync(new AddCardOptions(sourceList.Id, "C2"));
        Card card3 = await TrelloClient.AddCardAsync(new AddCardOptions(sourceList.Id, "C3"));

        await TrelloClient.MoveCardToListAsync(card2.Id, targetList.Id);

        List<Card> sourceAfter = await TrelloClient.GetCardsInListAsync(sourceList.Id);
        Assert.Equal(2, sourceAfter.Count);
        Assert.Contains(sourceAfter, x => x.Id == card1.Id);
        Assert.Contains(sourceAfter, x => x.Id == card3.Id);


        List<Card> targetAfter = await TrelloClient.GetCardsInListAsync(targetList.Id);
        Assert.Single(targetAfter);
        Assert.Contains(targetAfter, x => x.Id == card2.Id);
    }

    [Fact]
    public async Task MoveAllCardsInList()
    {
        string name = Guid.NewGuid().ToString();
        List addList = await TrelloClient.AddListAsync(new List(name, _boardId));
        //Add some cards so we can test Move All Cards In List
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C1"));
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C2"));
        await TrelloClient.AddCardAsync(new AddCardOptions(addList.Id, "C3"));

        //Add new list to move cards to
        List listToMoveTo = await TrelloClient.AddListAsync(new List("List to move to", _boardId));
        await TrelloClient.MoveAllCardsInListAsync(addList.Id, listToMoveTo.Id);
        List<Card> cardsOnListAfterMove = await TrelloClient.GetCardsInListAsync(listToMoveTo.Id);
        Assert.Equal(3, cardsOnListAfterMove.Count);
    }
}
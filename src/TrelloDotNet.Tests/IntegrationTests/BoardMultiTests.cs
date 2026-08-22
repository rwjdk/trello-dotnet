using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.AddCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class BoardMultiTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly string? _boardId = fixture.BoardId;
    private readonly string? _organizationId = fixture.OrganizationId;

    [Fact]
    public async Task ListCanBeMovedToAnotherBoard()
    {
        string? secondBoardId = null;
        try
        {
            Board board = new Board("UnitTestBoard - Second Board")
            {
                OrganizationId = _organizationId
            };
            Board secondBoard = await TrelloClient.AddBoardAsync(board);
            secondBoardId = secondBoard.Id;

            List addedList = await TrelloClient.AddListAsync(new List("List on first board", _boardId));
            await TrelloClient.AddCardAsync(new AddCardOptions(addedList.Id, "card to move between boards"));
            List<List> listOnPrimaryBoard = await TrelloClient.GetListsOnBoardAsync(_boardId);
            List<List> listOnSecondaryBoard = await TrelloClient.GetListsOnBoardAsync(secondBoardId);
            Assert.Equal(4, listOnPrimaryBoard.Count);
            Assert.Equal(3, listOnSecondaryBoard.Count);
            List<Card> cardsOnPrimaryBoard = await TrelloClient.GetCardsOnBoardAsync(_boardId);
            List<Card> cardsOnSecondaryBoard = await TrelloClient.GetCardsOnBoardAsync(secondBoardId);
            Assert.Single(cardsOnPrimaryBoard);
            Assert.Empty(cardsOnSecondaryBoard);

            await TrelloClient.MoveListToBoardAsync(addedList.Id, secondBoardId);

            List<List> listOnPrimaryBoardAfterMove = await TrelloClient.GetListsOnBoardAsync(_boardId);
            List<List> listOnSecondaryBoardAfterMove = await TrelloClient.GetListsOnBoardAsync(secondBoardId);
            Assert.Equal(3, listOnPrimaryBoardAfterMove.Count);
            Assert.Equal(4, listOnSecondaryBoardAfterMove.Count);

            List<Card> cardsOnPrimaryBoardAfterMove = await TrelloClient.GetCardsOnBoardAsync(_boardId);
            List<Card> cardsOnSecondaryBoardAfterMove = await TrelloClient.GetCardsOnBoardAsync(secondBoardId);
            Assert.Empty(cardsOnPrimaryBoardAfterMove);
            Assert.Single(cardsOnSecondaryBoardAfterMove);
        }
        finally
        {
            if (secondBoardId != null)
            {
                TrelloClient.Options.AllowDeleteOfBoards = true;
                await TrelloClient.DeleteBoardAsync(secondBoardId);
                TrelloClient.Options.AllowDeleteOfBoards = false;
            }
        }
    }
}
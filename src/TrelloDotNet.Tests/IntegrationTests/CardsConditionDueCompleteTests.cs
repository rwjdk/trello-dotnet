using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class CardsConditionDueCompleteTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
    [Fact]
    public async Task DueCompleteFilter()
    {
        List list1 = await AddDummyList(_board.Id, "List 1");
        List list2 = await AddDummyList(_board.Id, "List 2");
        List list3 = await AddDummyList(_board.Id, "List 3");

        await AddDummyCardToList(list1, "Card 1", due: DateTimeOffset.UtcNow.AddDays(-1), dueComplete: true);
        await AddDummyCardToList(list2, "Card 2", due: DateTimeOffset.UtcNow, dueComplete: true);
        await AddDummyCardToList(list3, "Card 3", due: DateTimeOffset.UtcNow.AddDays(1), dueComplete: false);

        // ReSharper disable once JoinDeclarationAndInitializer
        List<Card> cards;
        CardFields cardFields = new CardFields(CardFieldsType.Name, CardFieldsType.Due, CardFieldsType.DueComplete);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.IsComplete()]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.IsNotComplete()]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
    }
}

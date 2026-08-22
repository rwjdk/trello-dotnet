using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class CardsConditionStartedTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
    [Fact]
    public async Task StartedFilter()
    {
        List list1 = await AddDummyList(_board.Id, "List 1");
        List list2 = await AddDummyList(_board.Id, "List 2");
        List list3 = await AddDummyList(_board.Id, "List 3");

        Card card1 = await AddDummyCardToList(list1, "Card 1", start: DateTimeOffset.UtcNow.AddDays(-1));
        Card card2 = await AddDummyCardToList(list2, "Card 2", start: DateTimeOffset.UtcNow);
        Card card3 = await AddDummyCardToList(list3, "Card 3", start: DateTimeOffset.UtcNow.AddDays(1));

        // ReSharper disable once JoinDeclarationAndInitializer
        List<Card> cards;
        CardFields cardFields = new CardFields(CardFieldsType.Name, CardFieldsType.Start);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.AnyOfThese, card1.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.AnyOfThese, card1.Start!.Value, card2.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.Equal, card1.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.Equal, card1.Start!.Value, card2.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.NotEqual, card1.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.NotEqual, card1.Start!.Value, card2.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.Between, card1.Start!.Value, card3.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.NotBetween, card1.Start!.Value, card2.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.DoNotHaveAnyValue)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.HasAnyValue)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.GreaterThan, card1.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.GreaterThanOrEqual, card1.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.LessThan, card1.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.LessThanOrEqual, card1.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.NoneOfThese, card1.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Start(CardsConditionDate.NoneOfThese, card1.Start!.Value, card2.Start!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
    }
}

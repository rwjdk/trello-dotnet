using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class CardsConditionDueDateTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DueFilter(bool includeCardsThatAreMarkedAsComplete)
    {
        List<Card>? existingCards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        foreach (Card existingCard in existingCards)
        {
            await TrelloClient.DeleteCardAsync(existingCard.Id, cancellationToken: TestCancellationToken);
        }

        List list1 = await AddDummyList(_board.Id, "List 1");
        List list2 = await AddDummyList(_board.Id, "List 2");
        List list3 = await AddDummyList(_board.Id, "List 3");

        Card card1 = await AddDummyCardToList(list1, "Card 1", due: DateTimeOffset.UtcNow.AddDays(-1));
        Card card2 = await AddDummyCardToList(list2, "Card 2", due: DateTimeOffset.UtcNow);
        Card card3 = await AddDummyCardToList(list3, "Card 3", due: DateTimeOffset.UtcNow.AddDays(1));

        // ReSharper disable once JoinDeclarationAndInitializer
        List<Card> cards;
        CardFields cardFields = new CardFields(CardFieldsType.Name, CardFieldsType.Due, CardFieldsType.DueComplete);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.AnyOfThese, includeCardsThatAreMarkedAsComplete, card1.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.AnyOfThese, includeCardsThatAreMarkedAsComplete, card1.Due!.Value, card2.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.Equal, includeCardsThatAreMarkedAsComplete, card1.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.Equal, includeCardsThatAreMarkedAsComplete, card1.Due!.Value, card2.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.NotEqual, includeCardsThatAreMarkedAsComplete, card1.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.NotEqual, includeCardsThatAreMarkedAsComplete, card1.Due!.Value, card2.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.Between, includeCardsThatAreMarkedAsComplete, card1.Due!.Value, card3.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.NotBetween, includeCardsThatAreMarkedAsComplete, card1.Due!.Value, card2.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.DoNotHaveAnyValue, includeCardsThatAreMarkedAsComplete)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.HasAnyValue, includeCardsThatAreMarkedAsComplete)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.GreaterThan, includeCardsThatAreMarkedAsComplete, card1.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.GreaterThanOrEqual, includeCardsThatAreMarkedAsComplete, card1.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.LessThan, includeCardsThatAreMarkedAsComplete, card1.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.LessThanOrEqual, includeCardsThatAreMarkedAsComplete, card1.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.NoneOfThese, includeCardsThatAreMarkedAsComplete, card1.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Due(CardsConditionDate.NoneOfThese, includeCardsThatAreMarkedAsComplete, card1.Due!.Value, card2.Due!.Value)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
    }
}

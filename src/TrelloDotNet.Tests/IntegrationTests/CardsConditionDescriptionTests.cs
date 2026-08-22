using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class CardsConditionDescriptionTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
    [Fact]
    public async Task DescriptionFilter()
    {
        List list1 = await AddDummyList(_board.Id, "List 1");
        List list2 = await AddDummyList(_board.Id, "List 2");
        List list3 = await AddDummyList(_board.Id, "List 3");

        Card card1 = await AddDummyCardToList(list1, "Name1", "Card 1");
        Card card2 = await AddDummyCardToList(list2, "Name2", "Card 2");
        Card card3 = await AddDummyCardToList(list3, "Name3", "Card 3");

        // ReSharper disable once JoinDeclarationAndInitializer
        List<Card> cards;
        CardFields cardFields = new CardFields(CardFieldsType.Name, CardFieldsType.Description);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.HasDescription()]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.HasNoDescription()]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.AnyOfThese, "Card 1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.AnyOfThese, "Card 1", "Card 2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card2.Name);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.Contains, "ard 1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.Contains, "ard 1", "ard 2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card2.Name);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.DoNotContains, "ard 1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.DoNotContains, "ard 1", "ard 2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.DoNotEndWith, "ard 1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.DoNotEndWith, "ard 1", "ard 2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card3.Name);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.DoNotStartWith, "Card")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.DoNotStartWith, "Card", "Car")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.EndsWith, "1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.EndsWith, "2", "3")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.RegEx, "ard")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.RegEx, "ard", "rd")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.StartsWith, "C")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.RegEx, "Card 1", "Card 2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.AnyOfThese, "Card 1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.AnyOfThese, "Card 1", "Card 2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.Equal, "Card 1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.Equal, "Card 1", "Card 2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.NotEqual, "Card 1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.NotEqual, "Card 1", "Card 2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        //*********************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.NoneOfThese, "Card 1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.Description(CardsConditionString.NoneOfThese, "Card 1", "Card 2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        //*********************************************************
    }
}

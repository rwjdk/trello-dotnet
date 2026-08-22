using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class CardsConditionLabelTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
    [Fact]
    public async Task LabelsFilter()
    {
        List list1 = await AddDummyList(_board.Id, "List 1");
        List list2 = await AddDummyList(_board.Id, "List 2");
        List list3 = await AddDummyList(_board.Id, "List 3");

        Card card1 = await AddDummyCardToList(list1);
        Card card2 = await AddDummyCardToList(list2);
        Card card3 = await AddDummyCardToList(list3);

        Label redLabel = await TrelloClient.AddLabelAsync(new Label(_board.Id, "red", "red"), cancellationToken: TestCancellationToken);
        Label greenLabel = await TrelloClient.AddLabelAsync(new Label(_board.Id, "green", "green"), cancellationToken: TestCancellationToken);
        Label skyLabel = await TrelloClient.AddLabelAsync(new Label(_board.Id, "sky", "sky"), cancellationToken: TestCancellationToken);

        await TrelloClient.AddLabelsToCardAsync(card1.Id, TestCancellationToken, redLabel.Id, greenLabel.Id);
        await TrelloClient.AddLabelsToCardAsync(card2.Id, TestCancellationToken, skyLabel.Id, skyLabel.Id);

        // ReSharper disable once JoinDeclarationAndInitializer
        List<Card> cards;

        //Special *********************************************************************************************************
        CardFields cardFields = new CardFields(CardFieldsType.Name);
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.HasNoLabels()]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.HasAnyLabel()]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        //Between/Not Between *********************************************************************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCountBetween(1, 2)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCountNotBetween(1, 2)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        //Counts *********************************************************************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCount(CardsConditionCount.Equal, 2)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCount(CardsConditionCount.Equal, 1)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCount(CardsConditionCount.NotEqual, 99)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCount(CardsConditionCount.NotEqual, 2)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCount(CardsConditionCount.GreaterThan, 2)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCount(CardsConditionCount.GreaterThanOrEqual, 2)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCount(CardsConditionCount.LessThan, 2)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelCount(CardsConditionCount.LessThanOrEqual, 2)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        //Equal *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.Equal, skyLabel.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.Equal, redLabel.Id, greenLabel.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.Equal, skyLabel.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.Equal, redLabel.Name, greenLabel.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);


        //NotEqual *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.NotEqual, redLabel.Id)]
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.NotEqual, redLabel.Id, skyLabel.Id)]
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.NotEqual, skyLabel.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.NotEqual, skyLabel.Name, redLabel.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        //Any of These *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.AnyOfThese, redLabel.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.AnyOfThese, redLabel.Id, skyLabel.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.AnyOfThese, redLabel.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.AnyOfThese, redLabel.Name, skyLabel.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card2.Name);

        //None of These *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.NoneOfThese, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.NoneOfThese, redLabel.Id, greenLabel.Id, skyLabel.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card3.Name);


        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.NoneOfThese, redLabel.Name, greenLabel.Name, skyLabel.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.NoneOfThese, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        //All of These *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.AllOfThese, redLabel.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelId(CardsConditionIds.AllOfThese, redLabel.Id, greenLabel.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.AllOfThese, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.AllOfThese, redLabel.Name, skyLabel.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        //Other *****************************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.Contains, "Hello")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.Contains, "Hello", "World")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.DoNotContains, "Hello")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.DoNotContains, "Hello World")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.StartsWith, "Re")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.StartsWith, "Re", Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);


        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.DoNotStartWith, "Re")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.DoNotStartWith, "Re", Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.EndsWith, "d")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.EndsWith, "d", "y")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.DoNotEndWith, "x")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.DoNotEndWith, "x", "z")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.RegEx, "ed")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.LabelName(CardsConditionString.RegEx, "ed", Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
    }
}

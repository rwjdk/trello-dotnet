using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class CardsConditionListTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
    [Fact]
    public async Task ListFilter()
    {
        List list1 = await AddDummyList(_board.Id, "List 1");
        List list2 = await AddDummyList(_board.Id, "List 2");
        List list3 = await AddDummyList(_board.Id, "List 3");

        Card card1 = await AddDummyCardToList(list1);
        Card card2 = await AddDummyCardToList(list2);
        Card card3 = await AddDummyCardToList(list3);

        // ReSharper disable once JoinDeclarationAndInitializer
        List<Card> cards;
        //Equal *********************************************************************************************************

        CardFields cardFields = new CardFields(CardFieldsType.Name);
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.Equal, list1.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.Equal, list1.Id, list1.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.Equal, list2.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.Equal, list2.Name, list2.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);


        //NotEqual *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.NotEqual, list1.Id)]
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.NotEqual, list1.Id, list1.Id)]
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.NotEqual, list2.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.NotEqual, list2.Name, list2.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        //Any of These *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.AnyOfThese, list1.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.AnyOfThese, list1.Id, list2.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.AnyOfThese, list1.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.AnyOfThese, list1.Name, list2.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card2.Name);

        //None of These *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.NoneOfThese, list1.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.NoneOfThese, list1.Id, list2.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.NoneOfThese, list1.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.NoneOfThese, list1.Name, list2.Name)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card3.Name);

        //All of These *********************************************************************************************************

        await Assert.ThrowsAsync<TrelloApiException>(async () =>
        {
            cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
            {
                CardFields = cardFields,
                FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.AllOfThese, list1.Id)]
            }, cancellationToken: TestCancellationToken);
        });

        await Assert.ThrowsAsync<TrelloApiException>(async () =>
        {
            cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
            {
                CardFields = cardFields,
                FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.AllOfThese, list1.Id, list2.Id)]
            }, cancellationToken: TestCancellationToken);
        });


        await Assert.ThrowsAsync<TrelloApiException>(async () =>
        {
            cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
            {
                CardFields = cardFields,
                FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.AllOfThese, list1.Name)]
            }, cancellationToken: TestCancellationToken);
        });

        await Assert.ThrowsAsync<TrelloApiException>(async () =>
        {
            cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
            {
                CardFields = cardFields,
                FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.AllOfThese, list1.Name, list2.Name)]
            }, cancellationToken: TestCancellationToken);
        });

        //Other *****************************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.Contains, "Hello")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.Contains, "Hello", "World")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.DoNotContains, "Hello")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.DoNotContains, "Hello", "World")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.StartsWith, "Li")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.StartsWith, "Li", Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.DoNotStartWith, "Li")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.DoNotStartWith, "Li", Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.EndsWith, "1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.EndsWith, "1", "2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.DoNotEndWith, "1")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.DoNotEndWith, "1", "2")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.RegEx, "is")]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.ListName(CardsConditionString.RegEx, "is", Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);
    }
}

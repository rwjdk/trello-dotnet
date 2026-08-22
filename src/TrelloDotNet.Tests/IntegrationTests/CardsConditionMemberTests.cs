using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class CardExtensionsTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
    [Fact]
    public async Task MemberFilter()
    {
        List list1 = await AddDummyList(_board.Id, "List 1");
        List list2 = await AddDummyList(_board.Id, "List 2");
        List list3 = await AddDummyList(_board.Id, "List 3");

        Card card1 = await AddDummyCardToList(list1);
        Card card2 = await AddDummyCardToList(list2);
        Card card3 = await AddDummyCardToList(list3);

        Member member = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);

        await TrelloClient.AddMembersToCardAsync(card1.Id, TestCancellationToken, member.Id);
        await TrelloClient.AddMembersToCardAsync(card3.Id, TestCancellationToken, member.Id);

        // ReSharper disable once JoinDeclarationAndInitializer
        List<Card> cards;

        //Special
        CardFields cardFields = new CardFields(CardFieldsType.Name);
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.HasNoMembers()]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.HasAnyMember()]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        //Between / Not Between *********************************************************************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberCountBetween(1, 3)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberCountNotBetween(1, 3)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        //Count *********************************************************************************************************
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberCount(CardsConditionCount.Equal, 1)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberCount(CardsConditionCount.GreaterThan, 1)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberCount(CardsConditionCount.GreaterThanOrEqual, 1)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberCount(CardsConditionCount.NotEqual, 1)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberCount(CardsConditionCount.LessThan, 3)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberCount(CardsConditionCount.LessThanOrEqual, 1)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        //Equal *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.Equal, member.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.Equal, member.Id, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.Equal, member.FullName)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.Equal, member.FullName, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        //NotEqual *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.NotEqual, member.Id)]
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.NotEqual, member.Id, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.NotEqual, member.FullName)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.NotEqual, member.FullName, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        //Any of These *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.AnyOfThese, member.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.AnyOfThese, member.Id, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.AnyOfThese, member.FullName)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.AnyOfThese, member.FullName, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        //None of These *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.NoneOfThese, member.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.NoneOfThese, member.Id, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);


        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.NoneOfThese, member.FullName)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.NoneOfThese, member.FullName, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);
        Assert.Contains(cards, x => x.Name == card2.Name);

        //All of These *********************************************************************************************************

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.AllOfThese, member.Id)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberId(CardsConditionIds.AllOfThese, member.Id, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.AllOfThese, member.FullName)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);
        Assert.Contains(cards, x => x.Name == card1.Name);
        Assert.Contains(cards, x => x.Name == card3.Name);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.AllOfThese, member.FullName, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        //Other *****************************************************************************************************************

        string firstName = member.FullName.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0];
        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.Contains, firstName)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.Contains, Guid.NewGuid().ToString(), Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.DoNotContains, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.DoNotContains, Guid.NewGuid().ToString(), Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.StartsWith, firstName)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.StartsWith, Guid.NewGuid().ToString(), Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.DoNotStartWith, firstName)]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(1, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.DoNotStartWith, Guid.NewGuid().ToString(), Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.EndsWith, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.EndsWith, Guid.NewGuid().ToString(), Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.DoNotEndWith, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.DoNotEndWith, Guid.NewGuid().ToString(), Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(3, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.RegEx, Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);

        cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            CardFields = cardFields,
            FilterConditions = [CardsFilterCondition.MemberName(CardsConditionString.RegEx, Guid.NewGuid().ToString(), Guid.NewGuid().ToString())]
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(0, cards.Count);
    }
}

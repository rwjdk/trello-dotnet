using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public sealed class CreatedCardsFixture : TestFixtureWithNewBoard
{
    public IReadOnlyList<Card> Cards { get; private set; } = [];

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        List list = await AddDummyList(BoardId!, "Created filter");
        Card first = await AddDummyCardToList(list, "Card 1");
        await WaitForNextTrelloTimestampAsync(first.Created);
        Card second = await AddDummyCardToList(list, "Card 2");
        await WaitForNextTrelloTimestampAsync(second.Created);
        Card third = await AddDummyCardToList(list, "Card 3");

        Cards = [first, second, third];
    }
}

public class CardsConditionCreatedTests(CreatedCardsFixture fixture) : TestBase(fixture.TrelloClient), IClassFixture<CreatedCardsFixture>
{
    [Theory]
    [InlineData(CardsConditionDate.AnyOfThese, 1, 0)]
    [InlineData(CardsConditionDate.AnyOfThese, 2, 0, 1)]
    [InlineData(CardsConditionDate.Equal, 1, 0)]
    [InlineData(CardsConditionDate.Equal, 0, 0, 1)]
    [InlineData(CardsConditionDate.NotEqual, 2, 0)]
    [InlineData(CardsConditionDate.NotEqual, 3, 0, 1)]
    [InlineData(CardsConditionDate.Between, 3, 0, 2)]
    [InlineData(CardsConditionDate.NotBetween, 1, 0, 1)]
    [InlineData(CardsConditionDate.GreaterThan, 2, 0)]
    [InlineData(CardsConditionDate.GreaterThanOrEqual, 3, 0)]
    [InlineData(CardsConditionDate.LessThan, 0, 0)]
    [InlineData(CardsConditionDate.LessThanOrEqual, 1, 0)]
    [InlineData(CardsConditionDate.NoneOfThese, 2, 0)]
    [InlineData(CardsConditionDate.NoneOfThese, 1, 0, 1)]
    public async Task FiltersCardsByCreatedDate(CardsConditionDate condition, int expectedCount, params int[] cardIndexes)
    {
        DateTimeOffset[] timestamps = cardIndexes.Select(index => fixture.Cards[index].Created!.Value).ToArray();

        List<Card> cards = await GetCardsAsync(CardsFilterCondition.Created(condition, timestamps));

        Assert.Equal(expectedCount, cards.Count);
    }

    [Theory]
    [InlineData(CardsConditionDate.DoNotHaveAnyValue)]
    [InlineData(CardsConditionDate.HasAnyValue)]
    public async Task RejectsCreatedDateConditionsWithoutValues(CardsConditionDate condition)
    {
        await Assert.ThrowsAsync<TrelloApiException>(() => GetCardsAsync(CardsFilterCondition.Created(condition)));
    }

    private async Task<List<Card>> GetCardsAsync(CardsFilterCondition condition)
    {
        return await TrelloClient.GetCardsOnBoardAsync(fixture.BoardId!, new GetCardOptions
        {
            CardFields = new CardFields(CardFieldsType.Name),
            FilterConditions = [condition]
        }, cancellationToken: TestCancellationToken);
    }
}

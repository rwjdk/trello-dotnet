using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public sealed class OrderCardsFixture : TestFixtureWithNewBoard
{
    public IReadOnlyList<Card> Cards { get; private set; } = [];

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        List list = await AddDummyList(BoardId!, "Ordering");
        Card first = await AddDummyCardToList(list, "B", start: DateTimeOffset.UtcNow, due: DateTimeOffset.UtcNow.AddDays(3));
        await WaitForNextTrelloTimestampAsync(first.Created);
        Card second = await AddDummyCardToList(list, "X", start: DateTimeOffset.UtcNow.AddDays(1), due: DateTimeOffset.UtcNow.AddDays(2));
        await WaitForNextTrelloTimestampAsync(second.Created);
        Card third = await AddDummyCardToList(list, "A", start: DateTimeOffset.UtcNow.AddDays(-20), due: DateTimeOffset.UtcNow.AddDays(1));

        Cards = [first, second, third];
    }
}

public class OrderCardsTests(OrderCardsFixture fixture) : TestBase(fixture.TrelloClient), IClassFixture<OrderCardsFixture>
{
    [Theory]
    [InlineData(CardsOrderBy.CreateDateAsc, 0, 1, 2)]
    [InlineData(CardsOrderBy.CreateDateDesc, 2, 1, 0)]
    [InlineData(CardsOrderBy.NameAsc, 2, 0, 1)]
    [InlineData(CardsOrderBy.NameDesc, 1, 0, 2)]
    [InlineData(CardsOrderBy.DueDateAsc, 2, 1, 0)]
    [InlineData(CardsOrderBy.DueDateDesc, 0, 1, 2)]
    [InlineData(CardsOrderBy.StartDateAsc, 2, 0, 1)]
    [InlineData(CardsOrderBy.StartDateDesc, 1, 0, 2)]
    public async Task OrdersCardsBySelectedField(CardsOrderBy orderBy, int first, int second, int third)
    {
        List<Card> cards = await TrelloClient.GetCardsOnBoardAsync(fixture.BoardId!, new GetCardOptions
        {
            CardFields = new CardFields(CardFieldsType.Name),
            OrderBy = orderBy
        }, cancellationToken: TestCancellationToken);

        Assert.Collection(
            cards,
            card => Assert.Equal(fixture.Cards[first].Id, card.Id),
            card => Assert.Equal(fixture.Cards[second].Id, card.Id),
            card => Assert.Equal(fixture.Cards[third].Id, card.Id));
    }
}

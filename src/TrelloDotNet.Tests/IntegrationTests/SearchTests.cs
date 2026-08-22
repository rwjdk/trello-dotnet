using TrelloDotNet.Model;
using TrelloDotNet.Model.Search;

namespace TrelloDotNet.Tests.IntegrationTests;

public class SearchTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;

    [Fact]
    public async Task SearchPerson()
    {
        List<Member>? searchMembersAsync = await TrelloClient.SearchMembersAsync(new SearchMemberRequest("Rasmus Wulff Jensen")
        {
            Limit = 10,
        }, cancellationToken: TestCancellationToken);

        Member? firstOrDefault = searchMembersAsync.FirstOrDefault(x => x.Id == "63c14bd0466af2001c308467");
        Assert.NotNull(firstOrDefault);
    }

    [Fact]
    public async Task Search()
    {
        (List List, Card Card) card = await AddDummyCardAndList(_board.Id);

        SearchResult searchResult = await TrelloClient.SearchAsync(new SearchRequest("e")
        {
            BoardFields = new SearchRequestBoardFields("name"),
            CardFields = new SearchRequestCardFields("name"),
            OrganizationFields = new SearchRequestOrganizationFields("name"),
            BoardFilter = SearchRequestBoardFilter.Mine,
            BoardLimit = 10,
            CardLimit = 10,
            OrganizationLimit = 10,
            OrganizationFilter = new SearchRequestOrganizationFilter(_board.OrganizationId),
            PartialSearch = true,
            SearchCards = true,
            SearchBoards = true,
            SearchOrganizations = true,
            CardFilter = new SearchRequestCardFilter(card.Card.Id),
            CardPage = 0,
        }, cancellationToken: TestCancellationToken);

        Assert.NotNull(searchResult.Cards);
        Assert.NotNull(searchResult.Boards);
        Assert.NotNull(searchResult.Organizations);
        Assert.NotNull(searchResult.Options);
    }
}

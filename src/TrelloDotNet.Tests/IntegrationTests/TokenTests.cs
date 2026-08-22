using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetBoardOptions;
using TrelloDotNet.Model.Options.GetOrganizationOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class TokenTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
    private readonly Organization _organization = fixture.Organization!;

    [Fact]
    public async Task GetBoardsCurrentTokenCanAccess()
    {
        List<Board>? boards = await TrelloClient.GetBoardsCurrentTokenCanAccessAsync(cancellationToken: TestCancellationToken);
        Assert.Contains(boards, x => x.Id == _board.Id);
    }

    [Fact]
    public async Task GetBoardsCurrentTokenCanAccessWithOptions()
    {
        List<Board>? boards = await TrelloClient.GetBoardsCurrentTokenCanAccessAsync(new GetBoardOptions
        {
            BoardFields = new BoardFields(BoardFieldsType.Name),
            Filter = GetBoardOptionsFilter.Open
        }, cancellationToken: TestCancellationToken);

        Board board = Assert.Single(boards, x => x.Id == _board.Id);
        Assert.Equal(_board.Name, board.Name);
        Assert.Null(board.Description);
    }

    [Fact]
    public async Task GetCurrentTokenMembershipsAsync()
    {
        TokenMembershipOverview memberships = await TrelloClient.GetCurrentTokenMembershipsAsync(cancellationToken: TestCancellationToken);
        Assert.NotNull(memberships);
        Assert.NotEmpty(memberships.OrganizationMemberships);
        Assert.Contains(memberships.OrganizationMemberships, pair => pair.Key.Id == _board.OrganizationId);
        Assert.NotEmpty(memberships.BoardMemberships);
        Assert.Contains(memberships.BoardMemberships, pair => pair.Key.Id == _board.Id);
    }

    [Fact]
    public async Task GetCurrentTokenMembershipsAsyncWithOptions()
    {
        TokenMembershipOverview memberships = await TrelloClient.GetCurrentTokenMembershipsAsync(new GetBoardOptions
        {
            BoardFields = new BoardFields(BoardFieldsType.Name),
            Filter = GetBoardOptionsFilter.Open
        }, new GetOrganizationOptions
        {
            OrganizationFields = OrganizationFields.All
        }, TestCancellationToken);

        Assert.NotNull(memberships);
        Assert.Contains(memberships.OrganizationMemberships, pair => pair.Key.Id == _board.OrganizationId);
        KeyValuePair<Board, MembershipType> boardMembership = Assert.Single(memberships.BoardMemberships, pair => pair.Key.Id == _board.Id);
        Assert.Equal(_board.Name, boardMembership.Key.Name);
    }

    [Fact]
    public async Task GetOrganizationsCurrentTokenCanAccess()
    {
        List<Organization>? organizations = await TrelloClient.GetOrganizationsCurrentTokenCanAccessAsync(cancellationToken: TestCancellationToken);
        Assert.Contains(organizations, x => x.Id == _organization.Id);
    }

    [Fact]
    public async Task GetOrganizationsCurrentTokenCanAccessWithOptions()
    {
        List<Organization>? organizations = await TrelloClient.GetOrganizationsCurrentTokenCanAccessAsync(new GetOrganizationOptions
        {
            OrganizationFields = new OrganizationFields(OrganizationFieldsType.Name, OrganizationFieldsType.Url)
        }, cancellationToken: TestCancellationToken);
        Assert.Contains(organizations, x => x.Id == _organization.Id);
    }

    [Fact]
    public async Task TokenInformation()
    {
        TokenInformation? tokenInformation = await TrelloClient.GetTokenInformationAsync(cancellationToken: TestCancellationToken);
        Assert.NotNull(tokenInformation);
        Assert.NotNull(tokenInformation.Created);
        Assert.Null(tokenInformation.Expires);
        Assert.NotNull(tokenInformation.Id);
        Assert.NotNull(tokenInformation.Identifier);
        Assert.NotNull(tokenInformation.MemberId);
        Assert.NotNull(tokenInformation.Permissions);
        Assert.NotNull(tokenInformation.Permissions[0].ModelId);
        Assert.NotNull(tokenInformation.Permissions[0].ModelType);
        Assert.True(tokenInformation.Permissions[0].Read);
        Assert.True(tokenInformation.Permissions[0].Write);

        Member? tokenMember = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);
        Assert.NotNull(tokenMember);
    }
}

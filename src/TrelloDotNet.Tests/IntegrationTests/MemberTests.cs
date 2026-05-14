using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetMemberOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class MemberTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
    private readonly Organization _organization = fixture.Organization!;

    [Fact]
    public async Task GetTokenMember()
    {
        Member member = await TrelloClient.GetTokenMemberAsync(new GetMemberOptions
        {
            MemberFields = new MemberFields(MemberFieldsType.FullName)
        }, cancellationToken: TestCancellationToken);
        Assert.NotNull(member);
        Assert.NotEmpty(member.Id);
        Assert.NotEmpty(member.FullName);
        Assert.Null(member.Username);
        Assert.Null(member.AvatarUrl);
        Assert.Null(member.AvatarUrl30);
        Assert.Null(member.AvatarUrl170);
        Assert.Null(member.AvatarUrlOriginal);
    }

    [Fact]
    public async Task GetCardsForMember()
    {
        Member member = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);
        (List list, Card card) = await AddDummyCardAndList(_board.Id, "GetCardsForMember");
        await TrelloClient.AddMembersToCardAsync(card.Id, TestCancellationToken, member.Id);
        List<Card>? cardForMember = await TrelloClient.GetCardsForMemberAsync(member.Id, cancellationToken: TestCancellationToken);
        Assert.Contains(cardForMember, x => x.Id == card.Id && x.ListId == list.Id);
    }

    [Fact]
    public async Task GetBoardsForMember()
    {
        Member member = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);
        List<Board>? boards = await TrelloClient.GetBoardsForMemberAsync(member.Id, cancellationToken: TestCancellationToken);
        Assert.Contains(boards, x => x.Id == _board.Id);
    }

    [Fact]
    public async Task GetOrganizationsForMember()
    {
        Member member = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);
        List<Organization>? organizations = await TrelloClient.GetOrganizationsForMemberAsync(member.Id, cancellationToken: TestCancellationToken);
        Assert.Contains(organizations, x => x.Id == _organization.Id);
    }

    [Fact]
    public async Task AddRemoveChangeMemberOnBoard()
    {
        const string memberId = "69062db07766a797a98c6a13"; //Test_user ai@rwj.dk
        await TrelloClient.AddMemberToBoardAsync(_board.Id, memberId, MembershipType.Normal, cancellationToken: TestCancellationToken);

        List<Member>? members = await TrelloClient.GetMembersOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        Assert.Contains(members, x => x.Id == memberId);

        List<Membership>? memberships = await TrelloClient.GetMembershipsOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        Assert.Contains(memberships, x => x.MemberId == memberId && x.MemberType == MembershipType.Normal);
        Membership membership = memberships.Single(x => x.MemberId == memberId && x.MemberType == MembershipType.Normal);

        await TrelloClient.UpdateMembershipTypeOfMemberOnBoardAsync(_board.Id, membership.Id, MembershipType.Admin, cancellationToken: TestCancellationToken);

        List<Membership>? membershipsAfter = await TrelloClient.GetMembershipsOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        Assert.Contains(membershipsAfter, x => x.MemberId == memberId && x.MemberType == MembershipType.Admin);

        await TrelloClient.RemoveMemberFromBoardAsync(_board.Id, memberId, cancellationToken: TestCancellationToken);

        List<Member>? membersAfter = await TrelloClient.GetMembersOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        Assert.True(membersAfter.All(x => x.Id != memberId));

        await TrelloClient.InviteMemberToBoardViaEmailAsync(_board.Id, "ai@rwj.dk", MembershipType.Normal, cancellationToken: TestCancellationToken);

        List<Member>? membersAfterInvite = await TrelloClient.GetMembersOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        Assert.Contains(membersAfterInvite, x => x.Id == memberId);
    }

    [Fact]
    public async Task GetMembersOfCard()
    {
        List list = await AddDummyList(_board.Id);
        Card card = await AddDummyCardToList(list);
        Member? member = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);

        await TrelloClient.AddMembersToCardAsync(card.Id, TestCancellationToken, member.Id);

        List<Member>? membersOnCard = await TrelloClient.GetMembersOfCardAsync(card.Id, cancellationToken: TestCancellationToken);
        Assert.Contains(membersOnCard, x => x.Id == member.Id);

        // Test with options
        List<Member>? membersWithOptions = await TrelloClient.GetMembersOfCardAsync(card.Id, new GetMemberOptions
        {
            MemberFields = new MemberFields(MemberFieldsType.FullName)
        }, cancellationToken: TestCancellationToken);
        Assert.Contains(membersWithOptions, x => x.Id == member.Id);
    }

    [Fact]
    public async Task GetMembersWhoVotedOnCard()
    {
        List list = await AddDummyList(_board.Id);
        Card card = await AddDummyCardToList(list);

        List<Member>? votingMembers = await TrelloClient.GetMembersWhoVotedOnCardAsync(card.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(votingMembers);

        // Test with options
        List<Member>? votingMembersWithOptions = await TrelloClient.GetMembersWhoVotedOnCardAsync(card.Id, new GetMemberOptions
        {
            MemberFields = new MemberFields(MemberFieldsType.FullName)
        }, cancellationToken: TestCancellationToken);
        Assert.Empty(votingMembersWithOptions);
    }

    [Fact]
    public async Task GetMembersOfOrganization()
    {
        // Test with options
        List<Member>? membersWithOptions = await TrelloClient.GetMembersOfOrganizationAsync(fixture.OrganizationId!, new GetMemberOptions
        {
            MemberFields = new MemberFields(MemberFieldsType.FullName)
        }, cancellationToken: TestCancellationToken);
        Assert.NotEmpty(membersWithOptions);
        Assert.All(membersWithOptions, member => Assert.NotNull(member.FullName));
    }
}

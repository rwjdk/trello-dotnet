using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.IntegrationTests;

public class MemberTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;
    private readonly Organization _organization = fixture.Organization!;

    [Fact]
    public async Task GetCardsForMember()
    {
        Member member = await TrelloClient.GetTokenMemberAsync();
        (List list, Card card) = await AddDummyCardAndList(_board.Id, "GetCardsForMember");
        await TrelloClient.AddMembersToCardAsync(card.Id, member.Id);
        List<Card> cardForMember = await TrelloClient.GetCardsForMemberAsync(member.Id);
        Assert.Contains(cardForMember, x => x.Id == card.Id && x.ListId == list.Id);
    }

    [Fact]
    public async Task GetBoardsForMember()
    {
        Member member = await TrelloClient.GetTokenMemberAsync();
        List<Board> boards = await TrelloClient.GetBoardsForMemberAsync(member.Id);
        Assert.Contains(boards, x => x.Id == _board.Id);
    }

    [Fact]
    public async Task GetOrganizationsForMember()
    {
        Member member = await TrelloClient.GetTokenMemberAsync();
        List<Organization> organizations = await TrelloClient.GetOrganizationsForMemberAsync(member.Id);
        Assert.Contains(organizations, x => x.Id == _organization.Id);
    }

    [Fact]
    public async Task AddRemoveChangeMemberOnBoard()
    {
        const string memberId = "69062db07766a797a98c6a13"; //Test_user ai@rwj.dk

        await TrelloClient.AddMemberToBoardAsync(_board.Id, memberId, MembershipType.Normal);

        List<Member> members = await TrelloClient.GetMembersOfBoardAsync(_board.Id);
        Assert.Contains(members, x => x.Id == memberId);

        List<Membership> memberships = await TrelloClient.GetMembershipsOfBoardAsync(_board.Id);
        Assert.Contains(memberships, x => x.MemberId == memberId && x.MemberType == MembershipType.Normal);
        Membership membership = memberships.Single(x => x.MemberId == memberId && x.MemberType == MembershipType.Normal);

        await TrelloClient.UpdateMembershipTypeOfMemberOnBoardAsync(_board.Id, membership.Id, MembershipType.Admin);

        List<Membership> membershipsAfter = await TrelloClient.GetMembershipsOfBoardAsync(_board.Id);
        Assert.Contains(membershipsAfter, x => x.MemberId == memberId && x.MemberType == MembershipType.Admin);

        await TrelloClient.RemoveMemberFromBoardAsync(_board.Id, memberId);

        List<Member> membersAfter = await TrelloClient.GetMembersOfBoardAsync(_board.Id);
        Assert.True(membersAfter.All(x => x.Id != memberId));

        await TrelloClient.InviteMemberToBoardViaEmailAsync(_board.Id, "ai@rwj.dk", MembershipType.Normal);

        List<Member> membersAfterInvite = await TrelloClient.GetMembersOfBoardAsync(_board.Id);
        Assert.Contains(membersAfterInvite, x => x.Id == memberId);
    }
}
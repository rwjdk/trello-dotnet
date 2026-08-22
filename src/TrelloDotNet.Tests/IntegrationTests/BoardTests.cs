using System.Diagnostics;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Options.GetListOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class BoardTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly string _boardId = fixture.BoardId!;
    private readonly string _boardName = fixture.BoardName!;
    private readonly string _boardDescription = fixture.BoardDescription!;
    private readonly Board _board = fixture.Board!;

    [Fact]
    public void DefaultBoardData()
    {
        //Check Create date
        AssertTimeIsNow(_board.Created);

        Assert.False(_board.Closed);
        Assert.NotEmpty(_board.ShortUrl);
        Assert.NotEmpty(_board.Url);
        Assert.False(_board.Pinned);
        Assert.Null(_board.EnterpriseId);
        Assert.NotEmpty(_board.OrganizationId);
    }

    [Fact]
    public async Task DefaultMembers()
    {
        //Member Tests
        List<Member> members = await TrelloClient.GetMembersOfBoardAsync(_boardId);
        Assert.Single(members);
        Member member = await TrelloClient.GetMemberAsync(members.First().Id);
        Assert.Equal(member.FullName, members.First().FullName);
        Assert.Equal(member.Username, members.First().Username);
    }

    [Fact]
    public async Task DefaultLists()
    {
        List<List> lists = await TrelloClient.GetListsOnBoardAsync(_boardId);
        List? todoList = lists.FirstOrDefault(x => x.Name == "To Do");
        List? doingList = lists.FirstOrDefault(x => x.Name == "Doing");
        List? doneList = lists.FirstOrDefault(x => x.Name == "Done");
        Assert.NotNull(todoList);
        Assert.NotNull(doingList);
        Assert.NotNull(doneList);
    }

    [Fact]
    public async Task NoClosedLists()
    {
        //No closed lists
        List<List> listsFiltered = await TrelloClient.GetListsOnBoardAsync(_boardId, new GetListOptions
        {
            Filter = ListFilter.Closed
        });
        Assert.Empty(listsFiltered);
    }

    [Fact]
    public async Task DefaultCards()
    {
        //No Cards on the lists
        List<List> lists = await TrelloClient.GetListsOnBoardAsync(_boardId);
        foreach (List list in lists)
        {
            Assert.Empty(await TrelloClient.GetCardsInListAsync(list.Id));
        }

        Assert.Empty(await TrelloClient.GetCardsOnBoardAsync(_boardId));
        Assert.Empty(await TrelloClient.GetCardsOnBoardAsync(_boardId, new GetCardOptions
            {
                Filter = CardsFilter.All
            }
        ));
    }

    [Fact]
    public async Task BoardUpdate()
    {
        //Board Update
        Debug.Assert(_board != null, nameof(_board) + " != null");
        _board.Name = _boardName + "X";
        _board.Description = _boardDescription + "X";
        Board updatedBoard = await TrelloClient.UpdateBoardAsync(_board);
        Board getBoard = await TrelloClient.GetBoardAsync(_boardId);
        //Assert.EndsWith("X", updatedBoard.Name);
        Assert.EndsWith("X", updatedBoard.Description);
        Assert.EndsWith("X", getBoard.Name);
        Assert.EndsWith("X", getBoard.Description);
        Assert.Equal(updatedBoard.Description, getBoard.Description);
        Assert.Equal(updatedBoard.Description, getBoard.Description);
    }

    [Fact]
    public async Task DefaultChecklists()
    {
        //Checklist Defaults
        List<Checklist> checklists = await TrelloClient.GetChecklistsOnBoardAsync(_boardId);
        Assert.Empty(checklists);
    }

    [Fact]
    public async Task DefaultLabels()
    {
        //Label Defaults
        List<Label> labels = await TrelloClient.GetLabelsOfBoardAsync(_boardId);
        Assert.Equal(6, labels.Count);
        foreach (Label label in labels)
        {
            Assert.NotEmpty(label.Id);
            Assert.NotEmpty(label.BoardId);
            Assert.Empty(label.Name);
            AssertTimeIsNow(label.Created);
        }

        Label addedLabel = await TrelloClient.AddLabelAsync(new Label(_boardId, "MyLabel", "red"));
        Assert.NotNull(addedLabel.Id);
        Assert.Equal("red", addedLabel.Color);

        Label updateLabel = labels[0];
        updateLabel.Name = "Hello world";
        updateLabel.Color = "red";
        await TrelloClient.UpdateLabelAsync(updateLabel);

        await TrelloClient.DeleteLabelAsync(labels[1].Id);
        await TrelloClient.DeleteLabelAsync(labels[2].Id);

        List<Label> labelsAfterAddUpdateAndRemove = await TrelloClient.GetLabelsOfBoardAsync(_boardId);
        Assert.Equal(5, labelsAfterAddUpdateAndRemove.Count);
        Assert.Equal(3, labelsAfterAddUpdateAndRemove.Count(x => x.Color == "red"));
    }

    [Fact]
    public async Task MembershipInformation()
    {
        Member tokenMember = await TrelloClient.GetTokenMemberAsync();
        List<Membership> memberships = await TrelloClient.GetMembershipsOfBoardAsync(_boardId);
        Assert.Single(memberships);
        Membership membership = memberships.Single(x => x.MemberId == tokenMember.Id);
        Assert.Equal(MembershipType.Admin, membership.MemberType);
        Assert.False(membership.Unconfirmed);
        Assert.False(membership.Deactivated);
    }

    [Fact]
    public async Task BoardsInOrganization()
    {
        List<Board> boards = await TrelloClient.GetBoardsInOrganizationAsync(_board.OrganizationId);
        Assert.Single(boards);
        Assert.Equal(_board.Id, boards.First().Id);
    }
}
using System.Diagnostics;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetBoardOptions;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Options.GetLabelOptions;
using TrelloDotNet.Model.Options.GetListOptions;
using TrelloDotNet.Model.Options.GetMemberOptions;
using TrelloDotNet.Model.Options.UpdateBoardPreferencesOptions;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.IntegrationTests;

public class BoardTests(TestFixtureWithNewBoard fixture) : TestBase(fixture.TrelloClient), IClassFixture<TestFixtureWithNewBoard>
{
    private readonly string _boardId = fixture.BoardId!;
    private readonly string _boardName = fixture.BoardName!;
    private readonly string _boardDescription = fixture.BoardDescription!;
    private readonly Board _board = fixture.Board!;

    [Fact]
    public async Task DefaultBoardData()
    {
        await using TemporaryBoardContext boardContext = await CreateTemporaryBoardAsync(nameof(DefaultBoardData));
        Board boardUnderTest = boardContext.Board;

        //Check Create date
        AssertTimeIsNow(boardUnderTest.Created);

        Assert.False(boardUnderTest.Closed);
        Assert.NotEmpty(boardUnderTest.ShortUrl);
        Assert.NotEmpty(boardUnderTest.Url);
        Assert.False(boardUnderTest.Pinned);
        Assert.Null(boardUnderTest.EnterpriseId);
        Assert.NotEmpty(boardUnderTest.OrganizationId);

        Board? getBoardWithOptions = await TrelloClient.GetBoardAsync(boardUnderTest.Id, new GetBoardOptions
        {
            IncludeLists = GetBoardOptionsIncludeLists.Open,
            IncludeCards = GetBoardOptionsIncludeCards.OpenCards,
            BoardFields = new BoardFields(BoardFieldsType.Name),
            CardFields = new CardFields(CardFieldsType.Cover),
            Filter = GetBoardOptionsFilter.All,
            OrganizationFields = OrganizationFields.All,
            IncludeLabels = true,
            IncludePluginData = true,
            ActionsTypes = ActionTypesToInclude.All,
            CardsOrderBy = CardsOrderBy.CreateDateAsc,
            AdditionalParameters = [new QueryParameter("x", "x")],
            CardsFilterConditions =
            [
                CardsFilterCondition.Name(CardsConditionString.NotEqual, Guid.NewGuid().ToString()),
                CardsFilterCondition.Description(CardsConditionString.NotEqual, Guid.NewGuid().ToString()),
                CardsFilterCondition.MemberCount(CardsConditionCount.NotEqual, -1),
                CardsFilterCondition.MemberName(CardsConditionString.NotEqual, Guid.NewGuid().ToString()),
                CardsFilterCondition.Due(CardsConditionDate.NotEqual, true, DateTimeOffset.MinValue),
                CardsFilterCondition.Start(CardsConditionDate.NotEqual, DateTimeOffset.MinValue),
                CardsFilterCondition.IsComplete(),
                CardsFilterCondition.LabelId(CardsConditionIds.NotEqual, Guid.NewGuid().ToString()),
                CardsFilterCondition.LabelName(CardsConditionString.NotEqual, Guid.NewGuid().ToString()),
                CardsFilterCondition.ListName(CardsConditionString.NotEqual, Guid.NewGuid().ToString()),
            ]
        }, cancellationToken: TestCancellationToken);

        Assert.NotNull(getBoardWithOptions.Lists);
        Assert.Contains(getBoardWithOptions.Lists, x => x.Name == "To Do");
        Assert.NotNull(getBoardWithOptions.Cards);
        Assert.Null(getBoardWithOptions.Url);

        TrelloPlanInformation plan = await TrelloClient.GetTrelloPlanInformationForBoardAsync(boardUnderTest.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(boardUnderTest.Name, plan.Name);
        Assert.NotEmpty(plan.Features);
    }

    [Fact]
    public async Task DefaultMembers()
    {
        //Member Tests
        List<Member>? members = await TrelloClient.GetMembersOfBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        Assert.Single(members);
        Member? member = await TrelloClient.GetMemberAsync(members.First().Id, cancellationToken: TestCancellationToken);
        Assert.Equal(member.FullName, members.First().FullName);
        Assert.Equal(member.Username, members.First().Username);
    }

    [Fact]
    public async Task GetMembersOfBoardWithOptions()
    {
        List<Member>? members = await TrelloClient.GetMembersOfBoardAsync(_boardId, new GetMemberOptions
        {
            MemberFields = new MemberFields(MemberFieldsType.FullName)
        }, cancellationToken: TestCancellationToken);
        Assert.Single(members);
        Assert.NotEmpty(members[0].Id);
        Assert.NotEmpty(members[0].FullName);
        Assert.Null(members[0].Username);
    }

    [Fact]
    public async Task DefaultLists()
    {
        List<List>? lists = await TrelloClient.GetListsOnBoardAsync(_boardId, cancellationToken: TestCancellationToken);
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
        List<List>? listsFiltered = await TrelloClient.GetListsOnBoardAsync(_boardId, new GetListOptions
        {
            Filter = ListFilter.Closed
        }, cancellationToken: TestCancellationToken);
        Assert.Empty(listsFiltered);
    }

    [Fact]
    public async Task DefaultCards()
    {
        //No Cards on the lists
        List<List>? lists = await TrelloClient.GetListsOnBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        foreach (List list in lists)
        {
            Assert.Empty(await TrelloClient.GetCardsInListAsync(list.Id, cancellationToken: TestCancellationToken));
        }

        Assert.Empty(await TrelloClient.GetCardsOnBoardAsync(_boardId, cancellationToken: TestCancellationToken));
        Assert.Empty(await TrelloClient.GetCardsOnBoardAsync(_boardId, new GetCardOptions
            {
                Filter = CardsFilter.All
            }
            , cancellationToken: TestCancellationToken));
    }

    [Fact]
    public async Task BoardUpdate()
    {
        //Board Update
        Debug.Assert(_board != null, nameof(_board) + " != null");
        Board? updatedBoard = await TrelloClient.UpdateBoardAsync(_board.Id, [
            Model.BoardUpdate.Name(_boardName + "X"),
            Model.BoardUpdate.Description(_boardDescription + "X")
        ], cancellationToken: TestCancellationToken);
        Board? getBoard = await TrelloClient.GetBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        Assert.EndsWith("X", updatedBoard.Name);
        Assert.EndsWith("X", updatedBoard.Description);
        Assert.EndsWith("X", getBoard.Name);
        Assert.EndsWith("X", getBoard.Description);
        Assert.Equal(updatedBoard.Description, getBoard.Description);

        Board? getBoardWithOptions = await TrelloClient.GetBoardAsync(_boardId, new GetBoardOptions
        {
            ActionsTypes = new ActionTypesToInclude(WebhookActionTypes.UpdateBoard)
        }, cancellationToken: TestCancellationToken);
        Assert.NotEmpty(getBoardWithOptions.Actions);
    }

    [Fact]
    public async Task GetAndUpdateBoardPref()
    {
        await TrelloClient.UpdateBoardPreferencesAsync(_boardId, new UpdateBoardPreferencesOptions
        {
            CardAging = BoardPreferenceCardAging.Regular,
            CardCovers = BoardPreferenceCardCovers.DoNotShow,
            HideVotes = BoardPreferenceHideVotes.Hide,
            SelfJoin = BoardPreferenceSelfJoin.Yes,
            ShowCompletedStatusOnCardFront = BoardPreferenceShowCompletedStatusOnCardFront.DoNotShow,
            Visibility = BoardPreferenceVisibility.Workspace,
            WhoCanComment = BoardPreferenceWhoCanComment.Disabled,
            WhoCanVote = BoardPreferenceWhoCanVote.Disabled,
            WhoCanAddAndRemoveMembers = BoardPreferenceWhoCanAddAndRemoveMembers.Members
        }, cancellationToken: TestCancellationToken);

        Board board = await TrelloClient.GetBoardAsync(_boardId, new GetBoardOptions
        {
            BoardFields = new BoardFields(BoardFieldsType.Preferences)
        }, cancellationToken: TestCancellationToken);
        Assert.NotNull(board.Preferences);
        Assert.Equal(BoardPreferenceCardAging.Regular, board.Preferences.CardAging);
        Assert.Equal(BoardPreferenceCardCovers.DoNotShow, board.Preferences.CardCovers);
        Assert.Equal(BoardPreferenceHideVotes.Hide, board.Preferences.HideVotes);
        Assert.Equal(BoardPreferenceSelfJoin.Yes, board.Preferences.SelfJoin);
        Assert.Equal(BoardPreferenceShowCompletedStatusOnCardFront.DoNotShow, board.Preferences.ShowCompletedStatusOnCardFront);
        Assert.Equal(BoardPreferenceVisibility.Workspace, board.Preferences.Visibility);
        Assert.Equal(BoardPreferenceWhoCanComment.Disabled, board.Preferences.WhoCanComment);
        Assert.Equal(BoardPreferenceWhoCanVote.Disabled, board.Preferences.WhoCanVote);
        Assert.Equal(BoardPreferenceWhoCanAddAndRemoveMembers.Members, board.Preferences.WhoCanAddAndRemoveMembers);
    }

    [Fact]
    public async Task DefaultChecklists()
    {
        //Checklist Defaults
        List<Checklist>? checklists = await TrelloClient.GetChecklistsOnBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        Assert.Empty(checklists);
    }

    [Fact]
    public async Task DefaultLabels()
    {
        //Label Defaults
        List<Label>? labels = await TrelloClient.GetLabelsOfBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        Assert.Equal(6, labels.Count);
        foreach (Label label in labels)
        {
            Assert.NotEmpty(label.Id);
            Assert.NotEmpty(label.BoardId);
            Assert.Empty(label.Name);
            Assert.NotNull(label.Color);
            Assert.NotNull(label.ColorAsInfo);
            Assert.NotNull(label.ColorAsInfo.BackgroundHex);
            Assert.NotNull(label.ColorAsInfo.TextHex);
            AssertTimeIsNow(label.Created);
        }

        List<Label>? labelsWithOptions = await TrelloClient.GetLabelsOfBoardAsync(_boardId, new GetLabelOptions
        {
            Limit = 2,
            LabelFields = new LabelFields(LabelFieldsType.Name)
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, labelsWithOptions.Count);
        Assert.Null(labelsWithOptions[0].Color);
        Assert.Null(labelsWithOptions[1].Color);


        Label addedLabel = await TrelloClient.AddLabelAsync(new Label(_boardId, "MyLabel", "red"), cancellationToken: TestCancellationToken);
        Assert.NotNull(addedLabel.Id);
        Assert.Equal("red", addedLabel.Color);

        Label updateLabel = labels[0];
        updateLabel.Name = "Hello world";
        updateLabel.Color = "red";
        await TrelloClient.UpdateLabelAsync(updateLabel, cancellationToken: TestCancellationToken);

        await TrelloClient.DeleteLabelAsync(labels[1].Id, cancellationToken: TestCancellationToken);
        await TrelloClient.DeleteLabelAsync(labels[2].Id, cancellationToken: TestCancellationToken);

        List<Label>? labelsAfterAddUpdateAndRemove = await TrelloClient.GetLabelsOfBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        Assert.Equal(5, labelsAfterAddUpdateAndRemove.Count);
        Assert.Equal(3, labelsAfterAddUpdateAndRemove.Count(x => x.Color == "red"));
    }

    [Fact]
    public async Task MembershipInformation()
    {
        Member? tokenMember = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);
        List<Membership> memberships = await TrelloClient.GetMembershipsOfBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        Assert.Single(memberships);
        Membership membership = memberships.Single(x => x.MemberId == tokenMember.Id);
        Assert.Equal(MembershipType.Admin, membership.MemberType);
        Assert.False(membership.Unconfirmed);
        Assert.False(membership.Deactivated);
    }

    [Fact]
    public async Task PluginInformation()
    {
        List<Plugin>? plugins = await TrelloClient.GetPluginsOnBoardAsync(_boardId, cancellationToken: TestCancellationToken);
        Assert.Contains(plugins, x => x.Name == "Butler");
    }

    [Fact]
    public async Task BoardsInOrganization()
    {
        List<Board>? boards = await TrelloClient.GetBoardsInOrganizationAsync(_board.OrganizationId, cancellationToken: TestCancellationToken);
        Assert.Single(boards);
        Assert.Equal(_board.Id, boards.First().Id);

        boards = await TrelloClient.GetBoardsInOrganizationAsync(_board.OrganizationId, new GetBoardOptions
        {
            TypesOfBoardsToInclude = GetBoardOptionsTypesOfBoardsToInclude.All,
            IncludeLists = GetBoardOptionsIncludeLists.All,
            BoardFields = new BoardFields(BoardFieldsType.Name),
            IncludeOrganization = true,
            IncludeCards = GetBoardOptionsIncludeCards.OpenCards,
            CardFields = new CardFields(CardFieldsType.Name),
            OrganizationFields = new OrganizationFields(OrganizationFieldsType.Name),
            CardsOrderBy = CardsOrderBy.CreateDateAsc,
            Filter = GetBoardOptionsFilter.All,
            ActionsTypes = ActionTypesToInclude.All,
            AdditionalParameters = [new QueryParameter("x", "x")],
            CardsFilterConditions = [CardsFilterCondition.Name(CardsConditionString.NotEqual, Guid.NewGuid().ToString()),],
            IncludePluginData = true,
            IncludeLabels = true
        }, cancellationToken: TestCancellationToken);
        Assert.Single(boards);
        Assert.Equal(_board.Id, boards.First().Id);
        foreach (Board board in boards)
        {
            Assert.NotNull(board.Lists);
        }
    }
}

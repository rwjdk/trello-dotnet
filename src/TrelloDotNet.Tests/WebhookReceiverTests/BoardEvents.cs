using System.Reflection;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.WebhookReceiverTests;

public class BoardEvents : TestBase
{
    private bool _smartEventOnCardMovedToNewList;
    private bool _smartEventOnChecklistComplete;
    private bool _smartEventOnDueCardIsMarkedAsComplete;
    private bool _smartEventOnLabelAddedToCard;
    private bool _smartEventOnLabelRemovedFromCard;
    private bool _smartEventOnMemberAddedToCard;
    private bool _smartEventOnMemberRemovedFromCard;
    private bool _basicEventOnUpdateCard;
#pragma warning disable CS0414
    private bool _basicEventOnAcceptEnterpriseJoinRequest;
    private bool _basicEventOnAddAttachmentToCard;
    private bool _basicEventOnAddChecklistToCard;
    private bool _basicEventOnAddMemberToBoard;
    private bool _basicEventOnAddMemberToCard;
    private bool _basicEventOnAddMemberToOrganization;
    private bool _basicEventOnAddOrganizationToEnterprise;
    private bool _basicEventOnAddToEnterprisePluginWhitelist;
    private bool _basicEventOnAddToOrganizationBoard;
    private bool _basicEventOnCommentCard;
    private bool _basicEventOnConvertToCardFromCheckItem;
    private bool _basicEventOnCopyBoard;
    private bool _basicEventOnCopyCard;
    private bool _basicEventOnCopyCommentCard;
    private bool _basicEventOnCreateBoard;
    private bool _basicEventOnCreateCard;
    private bool _basicEventOnCreateList;
    private bool _basicEventOnCreateOrganization;
    private bool _basicEventOnDeleteBoardInvitation;
    private bool _basicEventOnDeleteCard;
    private bool _basicEventOnDeleteOrganizationInvitation;
    private bool _basicEventOnDisableEnterprisePluginWhitelist;
    private bool _basicEventOnDisablePlugin;
    private bool _basicEventOnDisablePowerUp;
    private bool _basicEventOnEmailCard;
    private bool _basicEventOnEnableEnterprisePluginWhitelist;
    private bool _basicEventOnEnablePlugin;
    private bool _basicEventOnEnablePowerUp;
    private bool _basicEventOnMakeAdminOfBoard;
    private bool _basicEventOnMakeNormalMemberOfBoard;
    private bool _basicEventOnMakeNormalMemberOfOrganization;
    private bool _basicEventOnMakeObserverOfBoard;
    private bool _basicEventOnMemberJoinedTrello;
    private bool _basicEventOnMoveCardFromBoard;
    private bool _basicEventOnMoveCardToBoard;
    private bool _basicEventOnMoveListFromBoard;
    private bool _basicEventOnMoveListToBoard;
    private bool _basicEventOnRemoveChecklistFromCard;
    private bool _basicEventOnRemoveFromEnterprisePluginWhitelist;
    private bool _basicEventOnRemoveFromOrganizationBoard;
    private bool _basicEventOnRemoveMemberFromCard;
    private bool _basicEventOnRemoveOrganizationFromEnterprise;
    private bool _basicEventOnUnconfirmedBoardInvitation;
    private bool _basicEventOnUnconfirmedOrganizationInvitation;
    private bool _basicEventOnUpdateBoard;
    private bool _basicEventOnUpdateCheckItemStateOnCard;
    private bool _basicEventOnUpdateChecklist;
    private bool _basicEventOnUpdateList;
    private bool _basicEventOnUpdateMember;
    private bool _basicEventOnUpdateOrganization;
    private bool _basicEventOnAddLabelToCard;
    private bool _basicEventOnCopyChecklist;
    private bool _basicEventOnCreateBoardInvitation;
    private bool _basicEventOnCreateBoardPreference;
    private bool _basicEventOnCreateCheckItem;
    private bool _basicEventOnCreateLabel;
    private bool _basicEventOnCreateOrganizationInvitation;
    private bool _basicEventOnDeleteAttachmentFromCard;
    private bool _basicEventOnDeleteCheckItem;
    private bool _basicEventOnDeleteComment;
    private bool _basicEventOnDeleteLabel;
    private bool _basicEventOnMakeAdminOfOrganization;
    private bool _basicEventOnRemoveLabelFromCard;
    private bool _basicEventOnRemoveMemberFromBoard;
    private bool _basicEventOnRemoveMemberFromOrganization;
    private bool _basicEventOnUpdateCheckItem;
    private bool _basicEventOnUpdateComment;
    private bool _basicEventOnUpdateLabel;
    private bool _basicEventOnVoteOnCard;
    private bool _basicEventOnUnknownActionType;
#pragma warning restore CS0414

    [Fact]
    public void BaseTest()
    {
        WebhookDataReceiver receiver = new WebhookDataReceiver(TrelloClient);
        receiver.ProcessJsonIntoEvents(string.Empty);
        string json = GetJsonFromSampleFile("AddStartDate.json");
        WebhookNotificationBoard eBoard = receiver.ConvertJsonToWebhookNotificationBoard(json);

        //Board
        Board board = eBoard.Board;
        Assert.Equal("63d128787441d05619f44dbe", board.Id);
        Assert.Equal("New Name2", board.Name);
        Assert.Equal("New description", board.Description);
        Assert.False(board.Closed);
        Assert.Equal("https://trello.com/b/SCPjg8ON", board.ShortUrl);
        Assert.Equal("https://trello.com/b/SCPjg8ON/new-name2", board.Url);
        Assert.NotNull(board.Created);

        //Action
        WebhookAction action = eBoard.Action;
        Assert.NotNull(action);
        Assert.Equal("63d1239e857afaa8b003c633", action.MemberCreatorId);
        Assert.Equal("updateCard", action.Type);
        Assert.Equal("63e2ae46d200aae1c0c784c2", action.Id);
        Assert.Equal(new DateTimeOffset(2023, 2, 7, 20, 2, 14, 641, TimeSpan.Zero), action.Date);

        //Display
        WebhookActionDisplay display = action.Display;
        Assert.Equal("action_added_a_start_date", display.TranslationKey);

        //Member creator
        Member memberCreator = action.MemberCreator;
        Assert.Equal("63d1239e857afaa8b003c633", memberCreator.Id);
        Assert.Equal("Rasmus", memberCreator.FullName);
        Assert.Equal("rasmus58348007", memberCreator.Username);

        //Action Data
        WebhookActionData data = action.Data;
        Assert.Equal(board.Id, data.Board.Id);
        Assert.Equal(board.Name, data.Board.Name);
        Assert.Equal("63d9942f12260e27b257d067", data.List.Id);
        Assert.Equal("qqq", data.List.Name);

        AssertData(data, PresentThings.Board, PresentThings.Card, PresentThings.List);
        AssertOld(data.Old);

        WebhookNotificationCard eCard = receiver.ConvertJsonToWebhookNotificationCard(json);
        Assert.NotNull(eCard.Action);

        WebhookNotificationList eList = receiver.ConvertJsonToWebhookNotificationList(json);
        Assert.NotNull(eList.Action);

        WebhookNotificationMember eMember = receiver.ConvertJsonToWebhookNotificationMember(json);
        Assert.NotNull(eMember.Action);

        WebhookNotification noMember = receiver.ConvertJsonToWebhookNotification(json);
        Assert.NotNull(noMember.Action);

        SubscribeToEventsProcessAndWait(receiver, json);
        AssertSmartEvents(); //None
        Assert.True(_basicEventOnUpdateCard);
    }

    private void SubscribeToEventsProcessAndWait(WebhookDataReceiver receiver, string json)
    {
        SubscribeToSmartEvents(receiver);
        SubscribeToBasicEvents(receiver);
        receiver.ProcessJsonIntoEvents(json);

        Thread.Sleep(500);
    }

    [Fact]
    public void AddChecklist()
    {
        WebhookDataReceiver receiver = new WebhookDataReceiver(TrelloClient);
        string json = GetJsonFromSampleFile("AddChecklist.json");
        WebhookNotificationBoard eBoard = receiver.ConvertJsonToWebhookNotificationBoard(json);

        //Action
        WebhookAction action = eBoard.Action;
        Assert.NotNull(action);

        //Display
        WebhookActionDisplay display = action.Display;
        Assert.Equal("action_add_checklist_to_card", display.TranslationKey);

        //Action Data
        WebhookActionData data = action.Data;
        AssertData(data, PresentThings.Board, PresentThings.Card, PresentThings.Checklist);
        AssertOld(data.Old);

        SubscribeToEventsProcessAndWait(receiver, json);

        AssertSmartEvents();
        Assert.True(_basicEventOnAddChecklistToCard);
    }

    [Fact]
    public void AddMemberToCard()
    {
        WebhookDataReceiver receiver = new WebhookDataReceiver(TrelloClient);
        string json = GetJsonFromSampleFile("AddMember.json");
        WebhookNotificationBoard eBoard = receiver.ConvertJsonToWebhookNotificationBoard(json);

        //Action
        WebhookAction action = eBoard.Action;
        Assert.NotNull(action);

        //Display
        WebhookActionDisplay display = action.Display;
        Assert.Equal("action_member_joined_card", display.TranslationKey);

        //Action Data
        WebhookActionData data = action.Data;
        AssertData(data, PresentThings.Member, PresentThings.Board, PresentThings.Card);
        AssertOld(data.Old);

        SubscribeToEventsProcessAndWait(receiver, json);

        AssertSmartEvents(SmartEventsFired.MemberAddedToCard);
        Assert.True(_basicEventOnAddMemberToCard);
    }

    [Fact]
    public void RemoveMemberFromCard()
    {
        WebhookDataReceiver receiver = new WebhookDataReceiver(TrelloClient);
        string json = GetJsonFromSampleFile("RemoveMember.json");
        WebhookNotificationBoard eBoard = receiver.ConvertJsonToWebhookNotificationBoard(json);

        //Action
        WebhookAction action = eBoard.Action;
        Assert.NotNull(action);

        //Display
        WebhookActionDisplay display = action.Display;
        Assert.Equal("action_member_left_card", display.TranslationKey);

        //Action Data
        WebhookActionData data = action.Data;
        AssertData(data, PresentThings.Member, PresentThings.Board, PresentThings.Card);
        AssertOld(data.Old);

        SubscribeToEventsProcessAndWait(receiver, json);

        AssertSmartEvents(SmartEventsFired.MemberRemovedFromCard);
        Assert.True(_basicEventOnRemoveMemberFromCard);
    }

    [Fact]
    public void MoveCardToList()
    {
        WebhookDataReceiver receiver = new WebhookDataReceiver(TrelloClient);
        string json = GetJsonFromSampleFile("MoveCardFromListToList.json");
        WebhookNotificationBoard eBoard = receiver.ConvertJsonToWebhookNotificationBoard(json);

        //Action
        WebhookAction action = eBoard.Action;
        Assert.NotNull(action);

        //Display
        WebhookActionDisplay display = action.Display;
        Assert.Equal("action_move_card_from_list_to_list", display.TranslationKey);

        //Action Data
        WebhookActionData data = action.Data;
        AssertData(data, PresentThings.ListBefore, PresentThings.ListAfter, PresentThings.Board, PresentThings.Card);
        AssertOld(data.Old, OldDataPresent.ListId);

        SubscribeToEventsProcessAndWait(receiver, json);

        AssertSmartEvents(SmartEventsFired.CardMovedToNewList);

        Assert.True(_basicEventOnUpdateCard);
    }

    [Fact]
    public void MarkDueAsComplete()
    {
        WebhookDataReceiver receiver = new WebhookDataReceiver(TrelloClient);
        string json = GetJsonFromSampleFile("MarkDueAsComplete.json");
        WebhookNotificationBoard eBoard = receiver.ConvertJsonToWebhookNotificationBoard(json);

        //Action
        WebhookAction action = eBoard.Action;
        Assert.NotNull(action);

        //Display
        WebhookActionDisplay display = action.Display;
        Assert.Equal("action_marked_the_due_date_complete", display.TranslationKey);

        //Action Data
        WebhookActionData data = action.Data;
        AssertData(data, PresentThings.Board, PresentThings.Card, PresentThings.List);
        AssertOld(data.Old, OldDataPresent.DueComplete);

        SubscribeToEventsProcessAndWait(receiver, json);

        AssertSmartEvents(SmartEventsFired.DueCardIsMarkedAsComplete);

        Assert.True(_basicEventOnUpdateCard);
    }

    [Fact]
    public void AddLabelToCard()
    {
        WebhookDataReceiver receiver = new WebhookDataReceiver(TrelloClient);
        string json = GetJsonFromSampleFile("AddLabel_Part1.json");
        WebhookNotificationBoard eBoard = receiver.ConvertJsonToWebhookNotificationBoard(json);

        //Action
        WebhookAction action = eBoard.Action;
        Assert.NotNull(action);

        //Display
        WebhookActionDisplay display = action.Display;
        Assert.Equal("action_add_label_to_card", display.TranslationKey);

        //Action Data
        WebhookActionData data = action.Data;
        AssertData(data, PresentThings.Board, PresentThings.Card, PresentThings.Label);
        AssertOld(data.Old);

        SubscribeToEventsProcessAndWait(receiver, json);

        AssertSmartEvents(SmartEventsFired.LabelAddedToCard);

        Assert.True(_basicEventOnAddLabelToCard);
    }

    [Fact]
    public void RemoveLabelFromCard()
    {
        WebhookDataReceiver receiver = new WebhookDataReceiver(TrelloClient);
        string json = GetJsonFromSampleFile("RemoveLabel_Part1.json");
        WebhookNotificationBoard eBoard = receiver.ConvertJsonToWebhookNotificationBoard(json);

        //Action
        WebhookAction action = eBoard.Action;
        Assert.NotNull(action);

        //Display
        WebhookActionDisplay display = action.Display;
        Assert.Equal("action_remove_label_from_card", display.TranslationKey);

        //Action Data
        WebhookActionData data = action.Data;
        AssertData(data, PresentThings.Board, PresentThings.Card, PresentThings.Label);
        AssertOld(data.Old);

        SubscribeToEventsProcessAndWait(receiver, json);

        AssertSmartEvents(SmartEventsFired.LabelRemovedFromCard);

        Assert.True(_basicEventOnRemoveLabelFromCard);
    }

    private void AssertOld(WebhookActionDataOld? old, params OldDataPresent[] oldDataPresent)
    {
        if (old == null)
        {
            Assert.Empty(oldDataPresent);
            return;
        }

        Assert.Equal(oldDataPresent.Contains(OldDataPresent.Description), old.Description != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.Name), old.Name != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.Address), old.Address != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.Coordinates), old.Coordinates != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.Labels), old.Labels != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.ListId), old.ListId != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.LocationName), old.LocationName != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.Start), old.Start != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.Due), old.Due != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.DueComplete), old.DueComplete != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.DueReminder), old.DueReminder != null);
        Assert.Equal(oldDataPresent.Contains(OldDataPresent.Position), old.Position != null);
    }

    private static void AssertData(WebhookActionData data, params PresentThings[] thingsPresent)
    {
        Assert.Equal(thingsPresent.Contains(PresentThings.Board), data.Board != null);
        Assert.Equal(thingsPresent.Contains(PresentThings.Card), data.Card != null);
        Assert.Equal(thingsPresent.Contains(PresentThings.Checklist), data.Checklist != null);
        Assert.Equal(thingsPresent.Contains(PresentThings.CheckItem), data.CheckItem != null);
        Assert.Equal(thingsPresent.Contains(PresentThings.List), data.List != null);
        Assert.Equal(thingsPresent.Contains(PresentThings.Label), data.Label != null);
        Assert.Equal(thingsPresent.Contains(PresentThings.Member), data.Member != null);
        Assert.Equal(thingsPresent.Contains(PresentThings.ListBefore), data.ListBefore != null);
        Assert.Equal(thingsPresent.Contains(PresentThings.ListAfter), data.ListAfter != null);
    }

    private void SubscribeToBasicEvents(WebhookDataReceiver receiver)
    {
        // ReSharper disable UnusedParameter.Local
        receiver.BasicEvents.OnUpdateCard += args => { _basicEventOnUpdateCard = true; };
        receiver.BasicEvents.OnAcceptEnterpriseJoinRequest += args => { _basicEventOnAcceptEnterpriseJoinRequest = true; };
        receiver.BasicEvents.OnAddAttachmentToCard += args => { _basicEventOnAddAttachmentToCard = true; };
        receiver.BasicEvents.OnAddChecklistToCard += args => { _basicEventOnAddChecklistToCard = true; };
        receiver.BasicEvents.OnAddMemberToBoard += args => { _basicEventOnAddMemberToBoard = true; };
        receiver.BasicEvents.OnAddMemberToCard += args => { _basicEventOnAddMemberToCard = true; };
        receiver.BasicEvents.OnAddMemberToOrganization += args => { _basicEventOnAddMemberToOrganization = true; };
        receiver.BasicEvents.OnAddOrganizationToEnterprise += args => { _basicEventOnAddOrganizationToEnterprise = true; };
        receiver.BasicEvents.OnAddToEnterprisePluginWhitelist += args => { _basicEventOnAddToEnterprisePluginWhitelist = true; };
        receiver.BasicEvents.OnAddToOrganizationBoard += args => { _basicEventOnAddToOrganizationBoard = true; };
        receiver.BasicEvents.OnCommentCard += args => { _basicEventOnCommentCard = true; };
        receiver.BasicEvents.OnConvertToCardFromCheckItem += args => { _basicEventOnConvertToCardFromCheckItem = true; };
        receiver.BasicEvents.OnCopyBoard += args => { _basicEventOnCopyBoard = true; };
        receiver.BasicEvents.OnCopyCard += args => { _basicEventOnCopyCard = true; };
        receiver.BasicEvents.OnCopyCommentCard += args => { _basicEventOnCopyCommentCard = true; };
        receiver.BasicEvents.OnCreateBoard += args => { _basicEventOnCreateBoard = true; };
        receiver.BasicEvents.OnCreateCard += args => { _basicEventOnCreateCard = true; };
        receiver.BasicEvents.OnCreateList += args => { _basicEventOnCreateList = true; };
        receiver.BasicEvents.OnCreateOrganization += args => { _basicEventOnCreateOrganization = true; };
        receiver.BasicEvents.OnDeleteBoardInvitation += args => { _basicEventOnDeleteBoardInvitation = true; };
        receiver.BasicEvents.OnDeleteCard += args => { _basicEventOnDeleteCard = true; };
        receiver.BasicEvents.OnDeleteOrganizationInvitation += args => { _basicEventOnDeleteOrganizationInvitation = true; };
        receiver.BasicEvents.OnDisableEnterprisePluginWhitelist += args => { _basicEventOnDisableEnterprisePluginWhitelist = true; };
        receiver.BasicEvents.OnDisablePlugin += args => { _basicEventOnDisablePlugin = true; };
        receiver.BasicEvents.OnDisablePowerUp += args => { _basicEventOnDisablePowerUp = true; };
        receiver.BasicEvents.OnEmailCard += args => { _basicEventOnEmailCard = true; };
        receiver.BasicEvents.OnEnableEnterprisePluginWhitelist += args => { _basicEventOnEnableEnterprisePluginWhitelist = true; };
        receiver.BasicEvents.OnEnablePlugin += args => { _basicEventOnEnablePlugin = true; };
        receiver.BasicEvents.OnEnablePowerUp += args => { _basicEventOnEnablePowerUp = true; };
        receiver.BasicEvents.OnMakeAdminOfBoard += args => { _basicEventOnMakeAdminOfBoard = true; };
        receiver.BasicEvents.OnMakeNormalMemberOfBoard += args => { _basicEventOnMakeNormalMemberOfBoard = true; };
        receiver.BasicEvents.OnMakeNormalMemberOfOrganization += args => { _basicEventOnMakeNormalMemberOfOrganization = true; };
        receiver.BasicEvents.OnMakeObserverOfBoard += args => { _basicEventOnMakeObserverOfBoard = true; };
        receiver.BasicEvents.OnMemberJoinedTrello += args => { _basicEventOnMemberJoinedTrello = true; };
        receiver.BasicEvents.OnMoveCardFromBoard += args => { _basicEventOnMoveCardFromBoard = true; };
        receiver.BasicEvents.OnMoveCardToBoard += args => { _basicEventOnMoveCardToBoard = true; };
        receiver.BasicEvents.OnMoveListFromBoard += args => { _basicEventOnMoveListFromBoard = true; };
        receiver.BasicEvents.OnMoveListToBoard += args => { _basicEventOnMoveListToBoard = true; };
        receiver.BasicEvents.OnRemoveChecklistFromCard += args => { _basicEventOnRemoveChecklistFromCard = true; };
        receiver.BasicEvents.OnRemoveFromEnterprisePluginWhitelist += args => { _basicEventOnRemoveFromEnterprisePluginWhitelist = true; };
        receiver.BasicEvents.OnRemoveFromOrganizationBoard += args => { _basicEventOnRemoveFromOrganizationBoard = true; };
        receiver.BasicEvents.OnRemoveMemberFromCard += args => { _basicEventOnRemoveMemberFromCard = true; };
        receiver.BasicEvents.OnRemoveOrganizationFromEnterprise += args => { _basicEventOnRemoveOrganizationFromEnterprise = true; };
        receiver.BasicEvents.OnUnconfirmedBoardInvitation += args => { _basicEventOnUnconfirmedBoardInvitation = true; };
        receiver.BasicEvents.OnUnconfirmedOrganizationInvitation += args => { _basicEventOnUnconfirmedOrganizationInvitation = true; };
        receiver.BasicEvents.OnUpdateBoard += args => { _basicEventOnUpdateBoard = true; };
        receiver.BasicEvents.OnUpdateCheckItemStateOnCard += args => { _basicEventOnUpdateCheckItemStateOnCard = true; };
        receiver.BasicEvents.OnUpdateChecklist += args => { _basicEventOnUpdateChecklist = true; };
        receiver.BasicEvents.OnUpdateList += args => { _basicEventOnUpdateList = true; };
        receiver.BasicEvents.OnUpdateMember += args => { _basicEventOnUpdateMember = true; };
        receiver.BasicEvents.OnUpdateOrganization += args => { _basicEventOnUpdateOrganization = true; };
        receiver.BasicEvents.OnAddLabelToCard += args => { _basicEventOnAddLabelToCard = true; };
        receiver.BasicEvents.OnCopyChecklist += args => { _basicEventOnCopyChecklist = true; };
        receiver.BasicEvents.OnCreateBoardInvitation += args => { _basicEventOnCreateBoardInvitation = true; };
        receiver.BasicEvents.OnCreateBoardPreference += args => { _basicEventOnCreateBoardPreference = true; };
        receiver.BasicEvents.OnCreateCheckItem += args => { _basicEventOnCreateCheckItem = true; };
        receiver.BasicEvents.OnCreateLabel += args => { _basicEventOnCreateLabel = true; };
        receiver.BasicEvents.OnCreateOrganizationInvitation += args => { _basicEventOnCreateOrganizationInvitation = true; };
        receiver.BasicEvents.OnDeleteAttachmentFromCard += args => { _basicEventOnDeleteAttachmentFromCard = true; };
        receiver.BasicEvents.OnDeleteCheckItem += args => { _basicEventOnDeleteCheckItem = true; };
        receiver.BasicEvents.OnDeleteComment += args => { _basicEventOnDeleteComment = true; };
        receiver.BasicEvents.OnDeleteLabel += args => { _basicEventOnDeleteLabel = true; };
        receiver.BasicEvents.OnMakeAdminOfOrganization += args => { _basicEventOnMakeAdminOfOrganization = true; };
        receiver.BasicEvents.OnRemoveLabelFromCard += args => { _basicEventOnRemoveLabelFromCard = true; };
        receiver.BasicEvents.OnRemoveMemberFromBoard += args => { _basicEventOnRemoveMemberFromBoard = true; };
        receiver.BasicEvents.OnRemoveMemberFromOrganization += args => { _basicEventOnRemoveMemberFromOrganization = true; };
        receiver.BasicEvents.OnUpdateCheckItem += args => { _basicEventOnUpdateCheckItem = true; };
        receiver.BasicEvents.OnUpdateComment += args => { _basicEventOnUpdateComment = true; };
        receiver.BasicEvents.OnUpdateLabel += args => { _basicEventOnUpdateLabel = true; };
        receiver.BasicEvents.OnVoteOnCard += args => { _basicEventOnVoteOnCard = true; };
        receiver.BasicEvents.OnUnknownActionType += args => { _basicEventOnUnknownActionType = true; };
    }

    private void AssertSmartEvents(params SmartEventsFired[] eventsFired)
    {
        Assert.Equal(eventsFired.Contains(SmartEventsFired.CardMovedToNewList), _smartEventOnCardMovedToNewList);
        Assert.Equal(eventsFired.Contains(SmartEventsFired.ChecklistComplete), _smartEventOnChecklistComplete);
        Assert.Equal(eventsFired.Contains(SmartEventsFired.DueCardIsMarkedAsComplete), _smartEventOnDueCardIsMarkedAsComplete);
        Assert.Equal(eventsFired.Contains(SmartEventsFired.LabelAddedToCard), _smartEventOnLabelAddedToCard);
        Assert.Equal(eventsFired.Contains(SmartEventsFired.LabelRemovedFromCard), _smartEventOnLabelRemovedFromCard);
        Assert.Equal(eventsFired.Contains(SmartEventsFired.MemberAddedToCard), _smartEventOnMemberAddedToCard);
        Assert.Equal(eventsFired.Contains(SmartEventsFired.MemberRemovedFromCard), _smartEventOnMemberRemovedFromCard);
    }

    private void SubscribeToSmartEvents(WebhookDataReceiver receiver)
    {
        receiver.SmartEvents.OnCardMovedToNewList += args =>
        {
            Assert.NotEmpty(args.NewListId);
            Assert.NotEmpty(args.NewListName);
            Assert.NotEmpty(args.OldListId);
            Assert.NotEmpty(args.OldListName);
            AssertBasicSmartEventCardBaseArgs(args);
            _smartEventOnCardMovedToNewList = true;
        };
        receiver.SmartEvents.OnChecklistComplete += args =>
        {
            Assert.NotEmpty(args.ChecklistId);
            Assert.NotEmpty(args.ChecklistName);
            Assert.NotEmpty(args.LastCheckItemCompletedId);
            Assert.NotEmpty(args.LastCheckItemCompletedName);
            AssertBasicSmartEventCardBaseArgs(args);
            _smartEventOnChecklistComplete = true;
        };
        receiver.SmartEvents.OnDueCardIsMarkedAsComplete += args =>
        {
            AssertBasicSmartEventCardBaseArgs(args);
            _smartEventOnDueCardIsMarkedAsComplete = true;
        };
        receiver.SmartEvents.OnLabelAddedToCard += args =>
        {
            Assert.NotEmpty(args.AddedLabelId);
            Assert.NotNull(args.AddedLabelName);
            AssertBasicSmartEventCardBaseArgs(args);
            _smartEventOnLabelAddedToCard = true;
        };
        receiver.SmartEvents.OnLabelRemovedFromCard += args =>
        {
            Assert.NotEmpty(args.RemovedLabelId);
            Assert.NotNull(args.RemovedLabelName);
            AssertBasicSmartEventCardBaseArgs(args);
            _smartEventOnLabelRemovedFromCard = true;
        };
        receiver.SmartEvents.OnMemberAddedToCard += args =>
        {
            Assert.NotEmpty(args.AddedMemberId);
            Assert.NotEmpty(args.AddedMemberName);
            AssertBasicSmartEventCardBaseArgs(args);
            _smartEventOnMemberAddedToCard = true;
        };
        receiver.SmartEvents.OnMemberRemovedFromCard += args =>
        {
            Assert.NotEmpty(args.RemovedMemberId);
            Assert.NotEmpty(args.RemovedMemberName);
            AssertBasicSmartEventCardBaseArgs(args);
            _smartEventOnMemberRemovedFromCard = true;
        };
    }

    private void AssertBasicSmartEventCardBaseArgs(WebhookSmartEventCardBase args)
    {
        Assert.NotEmpty(args.CardId);
        Assert.NotEmpty(args.CardName);
        Assert.NotEmpty(args.BoardId);
        Assert.NotEmpty(args.BoardName);
        Assert.NotNull(args.MemberCreator);
        Assert.NotEmpty(args.MemberCreator.FullName);
        Assert.NotEmpty(args.MemberCreator.Id);
        Assert.NotEmpty(args.MemberCreator.Username);
        Assert.True(args.TimeOfEvent < DateTimeOffset.Now);
    }

    private string GetJsonFromSampleFile(string filename)
    {
        const string sampleJsonFolder = "SampleJson";
        const string webhookEventsFolder = "WebhookEvents";
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        string path = Path.GetDirectoryName(assemblyLocation) +
                      Path.DirectorySeparatorChar +
                      sampleJsonFolder +
                      Path.DirectorySeparatorChar +
                      webhookEventsFolder +
                      Path.DirectorySeparatorChar +
                      filename;
        return File.ReadAllText(path);
    }

    private enum PresentThings
    {
        Board,
        Card,
        Checklist,
        CheckItem,
        Label,
        List,
        Member,
        ListBefore,
        ListAfter
    }

    private enum SmartEventsFired
    {
        CardMovedToNewList,
        ChecklistComplete,
        DueCardIsMarkedAsComplete,
        LabelAddedToCard,
        LabelRemovedFromCard,
        MemberAddedToCard,
        MemberRemovedFromCard
    }

    private enum OldDataPresent
    {
        Description,
        Name,
        Address,
        Coordinates,
        Labels,
        ListId,
        LocationName,
        Start,
        Due,
        DueComplete,
        DueReminder,
        Position
    }
}
using TrelloDotNet.AutomationEngine;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.AutomationEngine.Model;
using TrelloDotNet.AutomationEngine.Model.Actions;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Actions;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Webhook;
using Label = TrelloDotNet.Model.Label;

namespace TrelloDotNet.Tests.AutomationEngineTests.ActionTests;

public class ActionTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;

    [Fact]
    public async Task TestRemoveChecklistToCardAction()
    {
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card"));
        Checklist checklist = new Checklist("My Checklist", [new("A"), new("B")]);
        await TrelloClient.AddChecklistAsync(card.Id, checklist);
        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        IAutomationAction action = new RemoveChecklistFromCardAction(checklist.Name);
        await action.PerformActionAsync(webhookAction, processingResult);
        await action.PerformActionAsync(webhookAction, processingResult);
        List<Checklist> checklists = await TrelloClient.GetChecklistsOnCardAsync(card.Id);

        Assert.Empty(checklists);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task TestAddChecklistToCardAction()
    {
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card"));
        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        Checklist checklistToAdd = new Checklist("My Checklist", [new("A"), new("B")]);
        IAutomationAction action = new AddChecklistToCardAction(checklistToAdd);

        await action.PerformActionAsync(webhookAction, processingResult);
        List<Checklist> checklists = await TrelloClient.GetChecklistsOnCardAsync(card.Id);
        Assert.Single(checklists);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        await action.PerformActionAsync(webhookAction, processingResult);
        checklists = await TrelloClient.GetChecklistsOnCardAsync(card.Id);
        Assert.Single(checklists);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task TestAddChecklistToCardActionWithKeywords()
    {
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card"));
        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        Checklist checklistToAdd = new Checklist("My Checklist **ID** | **NAME**", [new("A | **ID** | **NAME**"), new("B | **ID** | **NAME**")]);
        IAutomationAction action = new AddChecklistToCardAction(checklistToAdd);

        await action.PerformActionAsync(webhookAction, processingResult);
        List<Checklist> checklists = await TrelloClient.GetChecklistsOnCardAsync(card.Id);
        Assert.Single(checklists);
        Assert.Equal($"My Checklist {card.Id} | {card.Name}", checklists[0].Name);
        Assert.Equal($"A | {card.Id} | {card.Name}", checklists[0].Items[0].Name);
        Assert.Equal($"B | {card.Id} | {card.Name}", checklists[0].Items[1].Name);
        Assert.Equal("My Checklist **ID** | **NAME**", checklistToAdd.Name);
        Assert.Equal("A | **ID** | **NAME**", checklistToAdd.Items[0].Name);
        Assert.Equal("B | **ID** | **NAME**", checklistToAdd.Items[1].Name);

        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);
    }

    [Fact]
    public async Task TestAddChecklistToCardActionToExsitingList()
    {
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card"));
        await TrelloClient.AddChecklistAsync(card.Id, new Checklist("My Checklist", [new("C")]));
        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        Checklist checklistToAdd = new Checklist("My Checklist", [new("A"), new("B")]);
        IAutomationAction action = new AddChecklistToCardAction(checklistToAdd)
        {
            AddCheckItemsToExistingChecklist = true,
        };

        await action.PerformActionAsync(webhookAction, processingResult);
        List<Checklist> checklists = await TrelloClient.GetChecklistsOnCardAsync(card.Id);
        Assert.Single(checklists);
        Assert.Equal(3, checklists.First().Items.Count);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        await action.PerformActionAsync(webhookAction, processingResult);
        checklists = await TrelloClient.GetChecklistsOnCardAsync(card.Id);
        Assert.Single(checklists);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task TestAddStickerToCardAction()
    {
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card"));
        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        const StickerDefaultImageId stickerDefaultImageId = StickerDefaultImageId.Check;
        IAutomationAction action = new AddStickerToCardAction(new Sticker(stickerDefaultImageId));
        await action.PerformActionAsync(webhookAction, processingResult);
        List<Sticker> stickers = await TrelloClient.GetStickersOnCardAsync(card.Id);
        Assert.Single(stickers);
        Assert.Equal(stickerDefaultImageId, stickers.First().ImageIdAsDefaultEnum);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        await action.PerformActionAsync(webhookAction, processingResult);
        stickers = await TrelloClient.GetStickersOnCardAsync(card.Id);
        Assert.Single(stickers);
        Assert.Equal(stickerDefaultImageId, stickers.First().ImageIdAsDefaultEnum);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);


        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task TestRemoveCoverFromCardAction()
    {
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card"));
        await TrelloClient.AddCoverToCardAsync(card.Id, new CardCover(CardCoverColor.Lime, CardCoverSize.Normal));

        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        IAutomationAction action = new RemoveCoverFromCardAction();
        await action.PerformActionAsync(webhookAction, processingResult);
        Card cardAfterAction = await TrelloClient.GetCardAsync(card.Id);
        Assert.Null(cardAfterAction.Cover.Color);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task TestSetFieldsOnCardAction()
    {
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        const string orgName = "Some Card";
        const string someDescription = "Some Description";
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, orgName, someDescription));
        await TrelloClient.AddStickerToCardAsync(card.Id, new Sticker(StickerDefaultImageId.Check));
        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        const string name = "x **NAME**";
        const string description = "y **DESCRIPTION**";
        DateTimeOffset start = DateTimeOffset.UtcNow;
        DateTimeOffset due = start.AddDays(2);
        const bool dueComplete = true;
        IAutomationAction action = new SetFieldsOnCardAction(
            new SetCardNameFieldValue(name),
            new SetCardDescriptionFieldValue(description, SetFieldsOnCardValueCriteria.OverwriteAnyPreviousValue),
            new SetCardStartFieldValue(start),
            new SetCardDueFieldValue(due),
            new SetCardDueCompleteFieldValue(dueComplete));

        await action.PerformActionAsync(webhookAction, processingResult);

        Card cardAfterAction = await TrelloClient.GetCardAsync(card.Id);
        Assert.Equal("x Some Card", cardAfterAction.Name);
        Assert.Equal("y Some Description", cardAfterAction.Description);
        Assert.Equal(start.Date, cardAfterAction.Start!.Value.Date);
        Assert.Equal(due.Date, cardAfterAction.Due!.Value.Date);
        Assert.Equal(dueComplete, cardAfterAction.DueComplete);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        IAutomationAction actionThatIsSkipped = new SetFieldsOnCardAction(
            new SetCardNameFieldValue(name + "xxx", SetFieldsOnCardValueCriteria.OnlySetIfBlank),
            new SetCardDescriptionFieldValue(description + "yyy"),
            new SetCardStartFieldValue(start.AddDays(1)),
            new SetCardDueFieldValue(due.AddDays(1)));

        await actionThatIsSkipped.PerformActionAsync(webhookAction, processingResult);

        Card cardAfterSkipAction = await TrelloClient.GetCardAsync(card.Id);
        Assert.Equal("x Some Card", cardAfterSkipAction.Name);
        Assert.Equal("y Some Description", cardAfterSkipAction.Description);
        Assert.Equal(start.Date, cardAfterSkipAction.Start!.Value.Date);
        Assert.Equal(due.Date, cardAfterSkipAction.Due!.Value.Date);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);

        await TrelloClient.UpdateCardAsync(cardAfterSkipAction.Id, [
            CardUpdate.Name(""),
            CardUpdate.Description(""),
        ]);

        IAutomationAction actionThatIsOverwrite = new SetFieldsOnCardAction(
            new SetCardNameFieldValue("xxx", SetFieldsOnCardValueCriteria.OnlySetIfBlank),
            new SetCardDescriptionFieldValue("yyy"),
            new SetCardStartFieldValue(start.AddDays(1), SetFieldsOnCardValueCriteria.OverwriteAnyPreviousValue),
            new SetCardDueFieldValue(due.AddDays(1), SetFieldsOnCardValueCriteria.OverwriteAnyPreviousValue));

        await actionThatIsOverwrite.PerformActionAsync(webhookAction, processingResult);

        Card cardAfterOverwriteAction = await TrelloClient.GetCardAsync(card.Id);
        Assert.Equal("xxx", cardAfterOverwriteAction.Name);
        Assert.Equal("yyy", cardAfterOverwriteAction.Description);
        Assert.Equal(start.AddDays(1).Date, cardAfterOverwriteAction.Start!.Value.Date);
        Assert.Equal(due.AddDays(1).Date, cardAfterOverwriteAction.Due!.Value.Date);
        Assert.Equal(2, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task TestRemoveStickerFromCardAction()
    {
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card"));
        await TrelloClient.AddStickerToCardAsync(card.Id, new Sticker(StickerDefaultImageId.Check));
        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        IAutomationAction action = new RemoveStickerFromCardAction(StickerDefaultImageId.Check);
        await action.PerformActionAsync(webhookAction, processingResult);

        action = new RemoveStickerFromCardAction("check"); //Second call to test no sticker exists
        await action.PerformActionAsync(webhookAction, processingResult);
        List<Sticker> stickers = await TrelloClient.GetStickersOnCardAsync(card.Id);

        Assert.Empty(stickers);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task AddCoverOnCardAction()
    {
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card"));
        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        const CardCoverColor cardCoverColor = CardCoverColor.Black;
        const CardCoverSize cardCoverSize = CardCoverSize.Full;
        IAutomationAction action = new AddCoverOnCardAction(new CardCover(cardCoverColor, cardCoverSize));
        await action.PerformActionAsync(webhookAction, processingResult);
        Card cardAfterPerformAction = await TrelloClient.GetCardAsync(card.Id);
        Assert.Equal(cardCoverColor, cardAfterPerformAction.Cover.Color);
        Assert.Equal(cardCoverSize, cardAfterPerformAction.Cover.Size);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task RemoveCardDataAction()
    {
        List<Label> labels = await TrelloClient.GetLabelsOfBoardAsync(_board.Id);
        Member member = await TrelloClient.GetTokenMemberAsync();
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card")
        {
            Due = DateTimeOffset.Now.AddDays(1),
            DueComplete = true,
            Start = DateTimeOffset.Now,
            Description = "My Description",
            MemberIds = [member.Id],
            LabelIds = labels.Take(2).Select(x => x.Id).ToList()
        });
        await TrelloClient.AddCoverToCardAsync(card.Id, new CardCover(CardCoverColor.Black, CardCoverSize.Full));

        await TrelloClient.AddCommentAsync(card.Id, new Comment("Hello World"));
        await TrelloClient.AddChecklistAsync(card.Id, new Checklist("My Checklist"));
        await TrelloClient.AddStickerToCardAsync(card.Id, new Sticker(StickerDefaultImageId.Check));
        await TrelloClient.AddAttachmentToCardAsync(card.Id, new AttachmentUrlLink("https://www.google.com", "Google"));

        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);

        IAutomationAction action = new RemoveCardDataAction(
            RemoveCardDataType.AllAttachments,
            RemoveCardDataType.AllChecklists,
            RemoveCardDataType.AllComments,
            RemoveCardDataType.AllLabels,
            RemoveCardDataType.AllMembers,
            RemoveCardDataType.AllStickers,
            RemoveCardDataType.Cover,
            RemoveCardDataType.Description,
            RemoveCardDataType.DueComplete,
            RemoveCardDataType.DueDate,
            RemoveCardDataType.StartDate
        );
        await action.PerformActionAsync(webhookAction, processingResult);

        Card cardAfterPerformAction = await TrelloClient.GetCardAsync(card.Id, new GetCardOptions
        {
            IncludeAttachments = GetCardOptionsIncludeAttachments.True
        });
        Assert.Equal(string.Empty, cardAfterPerformAction.Description);
        Assert.Empty(cardAfterPerformAction.Attachments);
        Assert.Null(cardAfterPerformAction.Due);
        Assert.False(cardAfterPerformAction.DueComplete);
        Assert.Null(cardAfterPerformAction.Start);
        Assert.Null(cardAfterPerformAction.Cover.Color);
        Assert.Empty(cardAfterPerformAction.MemberIds);
        Assert.Empty(cardAfterPerformAction.LabelIds);
        Assert.Empty(cardAfterPerformAction.ChecklistIds);
        List<Sticker> strickers = await TrelloClient.GetStickersOnCardAsync(cardAfterPerformAction.Id);
        Assert.Empty(strickers);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        //Call Second time to test "no update needed code branch"
        processingResult = new ProcessingResult();
        await action.PerformActionAsync(webhookAction, processingResult);
        Assert.Equal(0, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task AddMembersToCardAction()
    {
        Member member = await TrelloClient.GetTokenMemberAsync();
        Card card = await AddDummyCard(_board.Id, "AddMembersToCardAction");

        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card, boardToSimulate: _board);

        AddMembersToCardAction action = new AddMembersToCardAction(member.FullName) { TreatMemberNameAsId = true };
        await action.PerformActionAsync(webhookAction, processingResult);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        Card cardAfter = await TrelloClient.GetCardAsync(card.Id);
        Assert.Single(cardAfter.MemberIds);
        Assert.Contains(cardAfter.MemberIds, x => x.Contains(member.Id));

        //Call Second time to test "no update needed code branch"
        processingResult = new ProcessingResult();
        await action.PerformActionAsync(webhookAction, processingResult);
        Assert.Equal(0, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task RemoveMembersFromCardAction()
    {
        Member member = await TrelloClient.GetTokenMemberAsync();
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card")
        {
            MemberIds = [member.Id]
        });

        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card, boardToSimulate: _board);

        RemoveMembersFromCardAction action = new RemoveMembersFromCardAction(member.FullName) { TreatMemberNameAsId = true };
        await action.PerformActionAsync(webhookAction, processingResult);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        Card cardAfter = await TrelloClient.GetCardAsync(card.Id);
        Assert.Empty(cardAfter.MemberIds);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task AddLabelsToCardAction()
    {
        List<Label> labels = await TrelloClient.GetLabelsOfBoardAsync(_board.Id);
        Label label = labels.First();
        label.Name = Guid.NewGuid().ToString();
        label = await TrelloClient.UpdateLabelAsync(label);
        Card card = await AddDummyCard(_board.Id, "AddLabelsToCardAction");

        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card, boardToSimulate: _board);

        AddLabelsToCardAction action = new AddLabelsToCardAction(label.Name) { TreatLabelNameAsId = true };
        await action.PerformActionAsync(webhookAction, processingResult);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        Card cardAfter = await TrelloClient.GetCardAsync(card.Id);
        Assert.Single(cardAfter.LabelIds);
        Assert.Contains(cardAfter.LabelIds, x => x.Contains(label.Id));

        //Call Second time to test "no update needed code branch"
        processingResult = new ProcessingResult();
        await action.PerformActionAsync(webhookAction, processingResult);
        Assert.Equal(0, processingResult.ActionsExecuted);
        Assert.Equal(1, processingResult.ActionsSkipped);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task RemoveLabelsFromCardAction()
    {
        List<Label> labels = await TrelloClient.GetLabelsOfBoardAsync(_board.Id);
        Label label = labels.Last();
        label.Name = Guid.NewGuid().ToString();
        label = await TrelloClient.UpdateLabelAsync(label);
        List list = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Some Card")
        {
            LabelIds = [label.Id]
        });

        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card, boardToSimulate: _board);

        RemoveLabelsFromCardAction action = new RemoveLabelsFromCardAction(label.Name) { TreatLabelNameAsId = true };
        await action.PerformActionAsync(webhookAction, processingResult);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        Card cardAfter = await TrelloClient.GetCardAsync(card.Id);
        Assert.Empty(cardAfter.LabelIds);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task AddCommentToCardAction()
    {
        Card card = await AddDummyCard(_board.Id, "AddCommentToCardAction");

        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card, boardToSimulate: _board);

        string comment = Guid.NewGuid().ToString();
        AddCommentToCardAction action = new AddCommentToCardAction(comment);
        await action.PerformActionAsync(webhookAction, processingResult);
        Assert.Equal(1, processingResult.ActionsExecuted);
        Assert.Equal(0, processingResult.ActionsSkipped);

        List<TrelloAction> comments = await TrelloClient.GetAllCommentsOnCardAsync(card.Id);
        Assert.Single(comments);
        Assert.Contains(comments, x => x.Data.Text == comment);

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Fact]
    public async Task StopProcessingFurtherAction()
    {
        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated);

        StopProcessingFurtherAction action = new StopProcessingFurtherAction();
        await Assert.ThrowsAsync<StopProcessingFurtherActionException>(async () => await action.PerformActionAsync(webhookAction, processingResult));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AddChecklistToCardIfLabelMatchAction(bool treatLabelNameAsId)
    {
        List<Label> labels = await TrelloClient.GetLabelsOfBoardAsync(_board.Id);
        Label label1 = labels[1];
        Label label2 = labels[2];
        label1.Name = Guid.NewGuid().ToString();
        label1 = await TrelloClient.UpdateLabelAsync(label1);
        label2.Name = Guid.NewGuid().ToString();
        label2 = await TrelloClient.UpdateLabelAsync(label2);

        Card card = await AddDummyCard(_board.Id, "AddChecklistToCardIfLabelMatchAction");
        await TrelloClient.AddLabelsToCardAsync(card.Id, label1.Id);

        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card, boardToSimulate: _board);

        AddChecklistToCardIfLabelMatchAction action;
        if (treatLabelNameAsId)
        {
            action = new AddChecklistToCardIfLabelMatchAction(
                new AddChecklistToCardIfLabelMatch([label1.Name], new AddChecklistToCardAction(new Checklist("My Checklist1"))) { TreatLabelNameAsId = true },
                new AddChecklistToCardIfLabelMatch([label2.Name], new AddChecklistToCardAction(new Checklist("My Checklist2"))) { TreatLabelNameAsId = true }
            );
        }
        else
        {
            action = new AddChecklistToCardIfLabelMatchAction(
                new AddChecklistToCardIfLabelMatch(label1.Id, new AddChecklistToCardAction(new Checklist("My Checklist1"))) { TreatLabelNameAsId = false },
                new AddChecklistToCardIfLabelMatch(label2.Id, new AddChecklistToCardAction(new Checklist("My Checklist2"))) { TreatLabelNameAsId = false }
            );
        }

        await action.PerformActionAsync(webhookAction, processingResult);

        List<Checklist> checklists = await TrelloClient.GetChecklistsOnCardAsync(card.Id);
        Assert.Single(checklists);
        Assert.Contains(checklists, x => x.Name == "My Checklist1");

        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AddChecklistToCardIfLabelMatchActionWithExclude(bool treatLabelNameAsId)
    {
        List<Label> labels = await TrelloClient.GetLabelsOfBoardAsync(_board.Id);
        Label label3 = labels[3];
        Label label4 = labels[4];
        Label label5 = labels[5];
        label3.Name = Guid.NewGuid().ToString();
        label3 = await TrelloClient.UpdateLabelAsync(label3);
        label4.Name = Guid.NewGuid().ToString();
        label4 = await TrelloClient.UpdateLabelAsync(label4);
        label5.Name = Guid.NewGuid().ToString();
        label5 = await TrelloClient.UpdateLabelAsync(label5);

        Card card = await AddDummyCard(_board.Id, "AddChecklistToCardIfLabelMatchAction");
        await TrelloClient.AddLabelsToCardAsync(card.Id, label3.Id);
        await TrelloClient.AddLabelsToCardAsync(card.Id, label4.Id);
        await TrelloClient.AddLabelsToCardAsync(card.Id, label5.Id);

        ProcessingResult processingResult = new ProcessingResult();
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card, boardToSimulate: _board);

        AddChecklistToCardIfLabelMatchAction action;
        if (treatLabelNameAsId)
        {
            action = new AddChecklistToCardIfLabelMatchAction(
                new AddChecklistToCardIfLabelMatch([label3.Name], new AddChecklistToCardAction(new Checklist("My Checklist1"))) { TreatLabelNameAsId = true },
                new AddChecklistToCardIfLabelMatch([label4.Name], [label5.Name], new AddChecklistToCardAction(new Checklist("My Checklist2"))) { TreatLabelNameAsId = true }
            );
        }
        else
        {
            action = new AddChecklistToCardIfLabelMatchAction(
                new AddChecklistToCardIfLabelMatch(label3.Id, new AddChecklistToCardAction(new Checklist("My Checklist1"))) { TreatLabelNameAsId = false },
                new AddChecklistToCardIfLabelMatch(label4.Id, label5.Id, new AddChecklistToCardAction(new Checklist("My Checklist2"))) { TreatLabelNameAsId = false }
            );
        }

        await action.PerformActionAsync(webhookAction, processingResult);

        List<Checklist> checklists = await TrelloClient.GetChecklistsOnCardAsync(card.Id);
        Assert.Single(checklists);
        Assert.Contains(checklists, x => x.Name == "My Checklist1");


        await Assert.ThrowsAsync<AutomationException>(async () => await action.PerformActionAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated), processingResult));
    }
}
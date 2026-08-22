using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.AutomationEngine.Model;
using TrelloDotNet.AutomationEngine.Model.Conditions;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.ConditionTests;

public class ConditionTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;

    [Fact]
    public async Task TestCardFieldCondition()
    {
        List aList = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(aList.Id, "Some Card"));
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardCreated, cardToSimulate: card);

        Assert.True(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.IsNotSet).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Due, CardFieldConditionConstraint.IsNotSet).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Name, CardFieldConditionConstraint.IsSet).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Start, CardFieldConditionConstraint.IsNotSet).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.DueComplete, CardFieldConditionConstraint.IsNotSet).IsConditionMetAsync(webhookAction));

        card = await TrelloClient.UpdateCardAsync(card.Id, [
            CardUpdate.Description("Some Description"),
            CardUpdate.StartDate(DateTimeOffset.Now),
            CardUpdate.DueComplete(true),
            CardUpdate.DueDate(DateTimeOffset.Now.AddDays(1))
        ]);

        Assert.True(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.IsSet).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.Value, card.Description).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.Value, "Some", StringMatchCriteria.StartsWith).IsConditionMetAsync(webhookAction));
        Assert.False(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.Value, "xyz", StringMatchCriteria.StartsWith).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.Value, "ion", StringMatchCriteria.EndsWith).IsConditionMetAsync(webhookAction));
        Assert.False(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.Value, "xyz", StringMatchCriteria.EndsWith).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.Value, "me Des", StringMatchCriteria.Contains).IsConditionMetAsync(webhookAction));
        Assert.False(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.Value, "xyz", StringMatchCriteria.Contains).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.Value, "me Des", StringMatchCriteria.RegEx).IsConditionMetAsync(webhookAction));
        Assert.False(await new CardFieldCondition(CardField.Description, CardFieldConditionConstraint.Value, "xyz", StringMatchCriteria.RegEx).IsConditionMetAsync(webhookAction));

        Assert.True(await new CardFieldCondition(CardField.Due, CardFieldConditionConstraint.IsSet).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Due, CardFieldConditionConstraint.Value, card.Due.Value) { MatchDateOnlyOnDateTimeOffsetFields = true }.IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Due, CardFieldConditionConstraint.Value, card.Due.Value).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Due, CardFieldConditionConstraint.Value, card.Due.Value.AddDays(-1), DateTimeOffsetMatchCriteria.After).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Due, CardFieldConditionConstraint.Value, card.Due.Value.AddDays(+1), DateTimeOffsetMatchCriteria.Before).IsConditionMetAsync(webhookAction));

        Assert.True(await new CardFieldCondition(CardField.Name, CardFieldConditionConstraint.IsSet).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.Start, CardFieldConditionConstraint.IsSet).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardFieldCondition(CardField.DueComplete, CardFieldConditionConstraint.IsSet).IsConditionMetAsync(webhookAction));

        await Assert.ThrowsAsync<AutomationException>(async () => await new CardFieldCondition(CardField.Name, CardFieldConditionConstraint.IsSet).IsConditionMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated)));
    }

    [Fact]
    public async Task TestLabelCondition()
    {
        List<Label> labelsOfBoardAsync = await TrelloClient.GetLabelsOfBoardAsync(_board.Id);

        Label label1 = labelsOfBoardAsync[0];
        label1.Name = "Hello";
        label1.Color = "green";
        Label label2 = labelsOfBoardAsync[1];
        label2.Name = "World";
        label2.Color = "blue";
        Label label3 = labelsOfBoardAsync[2];
        label3.Name = "NoUsed";
        label3.Color = "red";

        Label updateLabel1 = await TrelloClient.UpdateLabelAsync(label1);
        Assert.Equal(label1.Color, updateLabel1.Color);
        Assert.Equal(label1.Name, updateLabel1.Name);

        Label updateLabel2 = await TrelloClient.UpdateLabelAsync(label2);
        Assert.Equal(label2.Color, updateLabel2.Color);
        Assert.Equal(label2.Name, updateLabel2.Name);

        Label updateLabel3 = await TrelloClient.UpdateLabelAsync(label3);
        Assert.Equal(label3.Color, updateLabel3.Color);
        Assert.Equal(label3.Name, updateLabel3.Name);

        List aList = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(aList.Id, "Some Card"));
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardCreated, cardToSimulate: card, boardToSimulate: _board);

        Assert.True(await new LabelCondition(LabelConditionConstraint.NonePresent) { TreatLabelNameAsId = true }.IsConditionMetAsync(webhookAction));
        Assert.False(await new LabelCondition(LabelConditionConstraint.AllOfThesePresent, label1.Name, label2.Name) { TreatLabelNameAsId = true }.IsConditionMetAsync(webhookAction));
        Assert.False(await new LabelCondition(LabelConditionConstraint.AnyOfThesePresent, label1.Id) { TreatLabelNameAsId = false }.IsConditionMetAsync(webhookAction));
        Assert.True(await new LabelCondition(LabelConditionConstraint.NoneOfTheseArePresent, label1.Id, label2.Id) { TreatLabelNameAsId = false }.IsConditionMetAsync(webhookAction));

        await TrelloClient.AddLabelsToCardAsync(card.Id, label1.Id, label2.Id);

        Assert.False(await new LabelCondition(LabelConditionConstraint.NonePresent) { TreatLabelNameAsId = true }.IsConditionMetAsync(webhookAction));
        Assert.True(await new LabelCondition(LabelConditionConstraint.AllOfThesePresent, label1.Name, label2.Name) { TreatLabelNameAsId = true }.IsConditionMetAsync(webhookAction));
        Assert.False(await new LabelCondition(LabelConditionConstraint.AllOfThesePresent, label1.Name, label2.Name, label3.Name) { TreatLabelNameAsId = true }.IsConditionMetAsync(webhookAction));
        Assert.True(await new LabelCondition(LabelConditionConstraint.AnyOfThesePresent, label1.Id) { TreatLabelNameAsId = false }.IsConditionMetAsync(webhookAction));
        Assert.False(await new LabelCondition(LabelConditionConstraint.NoneOfTheseArePresent, label1.Id, label2.Id) { TreatLabelNameAsId = false }.IsConditionMetAsync(webhookAction));
        Assert.True(await new LabelCondition(LabelConditionConstraint.NoneOfTheseArePresent, label3.Id) { TreatLabelNameAsId = false }.IsConditionMetAsync(webhookAction));

        await Assert.ThrowsAsync<AutomationException>(async () => await new LabelCondition(LabelConditionConstraint.AllOfThesePresent).IsConditionMetAsync(webhookAction));
    }

    [Fact]
    public async Task TestMemberCondition()
    {
        List<Member> membersOfBoardAsync = await TrelloClient.GetMembersOfBoardAsync(_board.Id);

        Member member1 = membersOfBoardAsync[0];

        List aList = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(aList.Id, "Some Card"));
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardCreated, cardToSimulate: card, boardToSimulate: _board);

        Assert.True(await new MemberCondition(MemberConditionConstraint.NonePresent) { TreatMemberNameAsId = true }.IsConditionMetAsync(webhookAction));
        Assert.False(await new MemberCondition(MemberConditionConstraint.AllOfThesePresent, member1.FullName) { TreatMemberNameAsId = true }.IsConditionMetAsync(webhookAction));
        Assert.False(await new MemberCondition(MemberConditionConstraint.AnyOfThesePresent, member1.Id) { TreatMemberNameAsId = false }.IsConditionMetAsync(webhookAction));
        Assert.True(await new MemberCondition(MemberConditionConstraint.NoneOfTheseArePresent, member1.Id) { TreatMemberNameAsId = false }.IsConditionMetAsync(webhookAction));

        await TrelloClient.AddMembersToCardAsync(card.Id, member1.Id);

        Assert.False(await new MemberCondition(MemberConditionConstraint.NonePresent) { TreatMemberNameAsId = true }.IsConditionMetAsync(webhookAction));
        Assert.True(await new MemberCondition(MemberConditionConstraint.AllOfThesePresent, member1.FullName) { TreatMemberNameAsId = true }.IsConditionMetAsync(webhookAction));
        Assert.True(await new MemberCondition(MemberConditionConstraint.AnyOfThesePresent, member1.Id) { TreatMemberNameAsId = false }.IsConditionMetAsync(webhookAction));
        Assert.False(await new MemberCondition(MemberConditionConstraint.NoneOfTheseArePresent, member1.Id) { TreatMemberNameAsId = false }.IsConditionMetAsync(webhookAction));
        Assert.True(await new MemberCondition(MemberConditionConstraint.NoneOfTheseArePresent, "1234") { TreatMemberNameAsId = false }.IsConditionMetAsync(webhookAction));

        await Assert.ThrowsAsync<AutomationException>(async () => await new MemberCondition(MemberConditionConstraint.AllOfThesePresent).IsConditionMetAsync(webhookAction));
    }

    [Fact]
    public async Task TestChecklistIncompleteCondition()
    {
        List aList = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(aList.Id, "Some Card"));

        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardCreated, cardToSimulate: card);
        const string checklistNameToCheck = "CheckList1";
        IAutomationCondition condition = new ChecklistIncompleteCondition(checklistNameToCheck);
        Assert.False(await condition.IsConditionMetAsync(webhookAction));

        await TrelloClient.AddChecklistAsync(card.Id, new Checklist(checklistNameToCheck));
        Assert.False(await condition.IsConditionMetAsync(webhookAction));

        const string checklistNameToCheck2 = "CheckList2";
        condition = new ChecklistIncompleteCondition(checklistNameToCheck2);
        Checklist checklist = await TrelloClient.AddChecklistAsync(card.Id, new Checklist(checklistNameToCheck2, [new("Item A"), new("Item B")]));
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        checklist.Items[0].State = ChecklistItemState.Complete;
        await TrelloClient.UpdateChecklistItemAsync(card.Id, checklist.Items[0]);
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        checklist.Items[1].State = ChecklistItemState.Complete;
        await TrelloClient.UpdateChecklistItemAsync(card.Id, checklist.Items[1]);
        Assert.False(await condition.IsConditionMetAsync(webhookAction));

        condition = new ChecklistIncompleteCondition("CheckList") { ChecklistNameMatchCriteria = StringMatchCriteria.StartsWith };
        Assert.False(await condition.IsConditionMetAsync(webhookAction));

        checklist.Items[1].State = ChecklistItemState.Incomplete;
        await TrelloClient.UpdateChecklistItemAsync(card.Id, checklist.Items[1]);

        condition = new ChecklistIncompleteCondition("2") { ChecklistNameMatchCriteria = StringMatchCriteria.EndsWith };
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        condition = new ChecklistIncompleteCondition("List") { ChecklistNameMatchCriteria = StringMatchCriteria.Contains };
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        condition = new ChecklistIncompleteCondition("hec\\w+List") { ChecklistNameMatchCriteria = StringMatchCriteria.RegEx };
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        condition = new ChecklistIncompleteCondition("hec\\w+List2") { ChecklistNameMatchCriteria = StringMatchCriteria.RegEx };
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        Assert.False(await condition.IsConditionMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated)));
    }

    [Fact]
    public async Task TestListCondition()
    {
        List aList = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        WebhookAction? webhookActionWithList = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.ListUpdated, listToSimulate: aList);
        await TestListConditionForSpecificWebhookAction(aList, webhookActionWithList);

        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(aList.Id, "Some Card"));
        WebhookAction webhookActionWithCard = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated, cardToSimulate: card);
        await TestListConditionForSpecificWebhookAction(aList, webhookActionWithCard);

        WebhookAction webhookActionWithBoardOnly = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated, cardToSimulate: card);
        ListCondition noDataCondition = new ListCondition(ListConditionConstraint.AnyOfTheseLists, aList.Name) { TreatListNameAsId = true };
        Assert.False(await noDataCondition.IsConditionMetAsync(webhookActionWithBoardOnly));
    }

    private async Task TestListConditionForSpecificWebhookAction(List aList, WebhookAction? webhookActionWithList)
    {
        ListCondition conditionTrueName = new ListCondition(ListConditionConstraint.AnyOfTheseLists, aList.Name) { TreatListNameAsId = true };
        Assert.True(await conditionTrueName.IsConditionMetAsync(webhookActionWithList));

        ListCondition conditionTrueId = new ListCondition(ListConditionConstraint.AnyOfTheseLists, aList.Id) { TreatListNameAsId = false };
        Assert.True(await conditionTrueId.IsConditionMetAsync(webhookActionWithList));

        ListCondition conditionFalseName = new ListCondition(ListConditionConstraint.AnyOfTheseLists, "SomeNameThatDoesNotExist") { TreatListNameAsId = true };
        Assert.False(await conditionFalseName.IsConditionMetAsync(webhookActionWithList));

        ListCondition conditionFalseId = new ListCondition(ListConditionConstraint.AnyOfTheseLists, Guid.NewGuid().ToString()) { TreatListNameAsId = false };
        Assert.False(await conditionFalseId.IsConditionMetAsync(webhookActionWithList));

        ListCondition noneConditionTrueName = new ListCondition(ListConditionConstraint.NoneOfTheseLists, aList.Name) { TreatListNameAsId = true };
        Assert.False(await noneConditionTrueName.IsConditionMetAsync(webhookActionWithList));

        ListCondition noneConditionTrueId = new ListCondition(ListConditionConstraint.NoneOfTheseLists, aList.Id) { TreatListNameAsId = false };
        Assert.False(await noneConditionTrueId.IsConditionMetAsync(webhookActionWithList));

        ListCondition noneConditionFalseName = new ListCondition(ListConditionConstraint.NoneOfTheseLists, "SomeNameThatDoesNotExist") { TreatListNameAsId = true };
        Assert.True(await noneConditionFalseName.IsConditionMetAsync(webhookActionWithList));

        ListCondition noneConditionFalseId = new ListCondition(ListConditionConstraint.NoneOfTheseLists, Guid.NewGuid().ToString()) { TreatListNameAsId = false };
        Assert.True(await noneConditionFalseId.IsConditionMetAsync(webhookActionWithList));
    }

    [Fact]
    public async Task TestCardCoverCondition()
    {
        List aList = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(aList.Id, "Some Card"));
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardCreated, cardToSimulate: card);

        Assert.True(await new CardCoverCondition(CardCoverConditionConstraint.DoesNotHaveACover).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardCoverCondition(CardCoverConditionConstraint.DoesNotHaveACoverOfTypeColor).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardCoverCondition(CardCoverConditionConstraint.DoesNotHaveACoverOfTypeImage).IsConditionMetAsync(webhookAction));

        Assert.False(await new CardCoverCondition(CardCoverConditionConstraint.HaveACover).IsConditionMetAsync(webhookAction));
        Assert.False(await new CardCoverCondition(CardCoverConditionConstraint.HaveACoverOfTypeColor).IsConditionMetAsync(webhookAction));
        Assert.False(await new CardCoverCondition(CardCoverConditionConstraint.HaveACoverOfTypeImage).IsConditionMetAsync(webhookAction));

        await TrelloClient.UpdateCoverOnCardAsync(card.Id, new CardCover(CardCoverColor.Blue, CardCoverSize.Full));

        Assert.False(await new CardCoverCondition(CardCoverConditionConstraint.DoesNotHaveACover).IsConditionMetAsync(webhookAction));
        Assert.False(await new CardCoverCondition(CardCoverConditionConstraint.DoesNotHaveACoverOfTypeColor).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardCoverCondition(CardCoverConditionConstraint.DoesNotHaveACoverOfTypeImage).IsConditionMetAsync(webhookAction));

        Assert.True(await new CardCoverCondition(CardCoverConditionConstraint.HaveACover).IsConditionMetAsync(webhookAction));
        Assert.True(await new CardCoverCondition(CardCoverConditionConstraint.HaveACoverOfTypeColor).IsConditionMetAsync(webhookAction));
        Assert.False(await new CardCoverCondition(CardCoverConditionConstraint.HaveACoverOfTypeImage).IsConditionMetAsync(webhookAction));

        WebhookAction webhookActionNoCard = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated);
        await Assert.ThrowsAsync<AutomationException>(async () => await new CardCoverCondition(CardCoverConditionConstraint.HaveACover).IsConditionMetAsync(webhookActionNoCard));
    }

    [Fact]
    public async Task TestChecklistNotStartedCondition()
    {
        List aList = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(aList.Id, "Some Card"));

        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardCreated, cardToSimulate: card);
        WebhookAction webhookActionNoCard = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated);

        const string checklistNameToCheck = "Test";
        IAutomationCondition condition = new ChecklistNotStartedCondition(checklistNameToCheck);
        Assert.False(await condition.IsConditionMetAsync(webhookAction));
        Assert.False(await condition.IsConditionMetAsync(webhookActionNoCard));

        Checklist checklist = await TrelloClient.AddChecklistAsync(card.Id, new Checklist(checklistNameToCheck, [new("Item A"), new("Item B")]));
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        checklist.Items[0].State = ChecklistItemState.Complete;
        await TrelloClient.UpdateChecklistItemAsync(card.Id, checklist.Items[0]);
        Assert.False(await condition.IsConditionMetAsync(webhookAction));
    }

    [Fact]
    public async Task TestChecklistItemsIncompleteCondition()
    {
        List aList = await TrelloClient.AddListAsync(new List(Guid.NewGuid().ToString(), _board.Id));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(aList.Id, "Some Card"));

        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardCreated, cardToSimulate: card);
        IAutomationCondition condition = new ChecklistItemsIncompleteCondition();
        Assert.False(await condition.IsConditionMetAsync(webhookAction));

        await TrelloClient.AddChecklistAsync(card.Id, new Checklist("CheckList1"));
        Assert.False(await condition.IsConditionMetAsync(webhookAction));

        Checklist checklist = await TrelloClient.AddChecklistAsync(card.Id, new Checklist("CheckList2", [new("Item A"), new("Item B")]));
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        checklist.Items[0].State = ChecklistItemState.Complete;
        await TrelloClient.UpdateChecklistItemAsync(card.Id, checklist.Items[0]);
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        checklist.Items[1].State = ChecklistItemState.Complete;
        await TrelloClient.UpdateChecklistItemAsync(card.Id, checklist.Items[1]);
        Assert.False(await condition.IsConditionMetAsync(webhookAction));

        Assert.False(await condition.IsConditionMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated)));
    }

    [Fact]
    public async Task TestChecklistItemsCompleteCondition()
    {
        Card card = await AddDummyCard(_board.Id, "TestChecklistItemsCompleteCondition");

        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardCreated, cardToSimulate: card);
        IAutomationCondition condition = new ChecklistItemsCompleteCondition();
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        await TrelloClient.AddChecklistAsync(card.Id, new Checklist("CheckList1"));
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        Checklist checklist = await TrelloClient.AddChecklistAsync(card.Id, new Checklist("CheckList2", [new("Item A"), new("Item B")]));
        Assert.False(await condition.IsConditionMetAsync(webhookAction));

        checklist.Items[0].State = ChecklistItemState.Complete;
        await TrelloClient.UpdateChecklistItemAsync(card.Id, checklist.Items[0]);
        Assert.False(await condition.IsConditionMetAsync(webhookAction));

        checklist.Items[1].State = ChecklistItemState.Complete;
        await TrelloClient.UpdateChecklistItemAsync(card.Id, checklist.Items[1]);
        Assert.True(await condition.IsConditionMetAsync(webhookAction));

        Assert.False(await condition.IsConditionMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated)));
    }
}
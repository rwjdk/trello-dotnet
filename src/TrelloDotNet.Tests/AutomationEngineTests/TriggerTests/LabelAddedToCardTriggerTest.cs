using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class LabelAddedToCardTriggerTest : TestBase
{
    [Fact]
    public async Task NameAnyTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelAddedToCard);
        LabelAddedToCardTrigger trigger = new LabelAddedToCardTrigger(LabelAddedToCardTriggerConstraint.AnyLabel) { TreatLabelNameAsId = true };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelAddedToCard);
        LabelAddedToCardTrigger trigger = new LabelAddedToCardTrigger(LabelAddedToCardTriggerConstraint.AnyOfTheseLabelsAreAdded, webhookAction.Data.Label.Name) { TreatLabelNameAsId = true };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelAddedToCard);
        LabelAddedToCardTrigger trigger = new LabelAddedToCardTrigger(LabelAddedToCardTriggerConstraint.AnyButTheseLabelsAreAreAdded, webhookAction.Data.Label.Name) { TreatLabelNameAsId = true };
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task IdTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelAddedToCard);
        LabelAddedToCardTrigger trigger = new LabelAddedToCardTrigger(LabelAddedToCardTriggerConstraint.AnyOfTheseLabelsAreAdded, webhookAction.Data.Label.Id) { TreatLabelNameAsId = false };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task TriggerException()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelAddedToCard);
        LabelAddedToCardTriggerConstraint constraint = Enum.Parse<LabelAddedToCardTriggerConstraint>("99");
        LabelAddedToCardTrigger cardMovedToListTrigger = new LabelAddedToCardTrigger(constraint, Guid.NewGuid().ToString());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }
}
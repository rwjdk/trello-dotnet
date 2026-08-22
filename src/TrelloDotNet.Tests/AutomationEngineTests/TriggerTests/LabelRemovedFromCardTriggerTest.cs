using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class LabelRemovedFromCardTriggerTest : TestBase
{
    [Fact]
    public async Task NameAnyTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelRemovedFromCard);
        LabelRemovedFromCardTrigger trigger = new LabelRemovedFromCardTrigger(LabelRemovedFromCardTriggerConstraint.AnyLabel) { TreatLabelNameAsId = true };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelRemovedFromCard);
        LabelRemovedFromCardTrigger trigger = new LabelRemovedFromCardTrigger(LabelRemovedFromCardTriggerConstraint.AnyOfTheseLabelsAreRemoved, webhookAction.Data.Label.Name) { TreatLabelNameAsId = true };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelRemovedFromCard);
        LabelRemovedFromCardTrigger trigger = new LabelRemovedFromCardTrigger(LabelRemovedFromCardTriggerConstraint.AnyButTheseLabelsAreRemoved, webhookAction.Data.Label.Name) { TreatLabelNameAsId = true };
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task IdTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelRemovedFromCard);
        LabelRemovedFromCardTrigger trigger = new LabelRemovedFromCardTrigger(LabelRemovedFromCardTriggerConstraint.AnyOfTheseLabelsAreRemoved, webhookAction.Data.Label.Id) { TreatLabelNameAsId = false };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task TriggerException()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.LabelRemovedFromCard);
        LabelRemovedFromCardTriggerConstraint constraint = Enum.Parse<LabelRemovedFromCardTriggerConstraint>("99");
        LabelRemovedFromCardTrigger cardMovedToListTrigger = new LabelRemovedFromCardTrigger(constraint, Guid.NewGuid().ToString());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }
}
using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class CheckItemStateUpdatedOnCardTriggerTest : TestBase
{
    [Fact]
    public async Task TriggerNoneWrongEvent()
    {
        CheckItemStateUpdatedOnCardTrigger trigger = new CheckItemStateUpdatedOnCardTrigger();
        Assert.False(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated)));
    }

    [Fact]
    public async Task TriggerNoneRightEvent()
    {
        CheckItemStateUpdatedOnCardTrigger trigger = new CheckItemStateUpdatedOnCardTrigger();
        Assert.True(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CheckItemStateUpdated)));
    }

    [Fact]
    public async Task TriggerIncompleteTrue()
    {
        CheckItemStateUpdatedOnCardTrigger trigger = new CheckItemStateUpdatedOnCardTrigger(ChecklistItemState.Incomplete);
        Assert.True(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CheckItemStateUpdated)));
    }

    [Fact]
    public async Task TriggerFalse()
    {
        CheckItemStateUpdatedOnCardTrigger trigger = new CheckItemStateUpdatedOnCardTrigger(ChecklistItemState.Complete);
        Assert.False(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CheckItemStateUpdated)));
    }
}
using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class CardUpdatedTriggerTest : TestBase
{
    [Fact]
    public async Task TriggerTrue()
    {
        CardUpdatedTrigger trigger = new CardUpdatedTrigger();
        Assert.True(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardUpdated)));
    }

    [Fact]
    public async Task TriggerFalse()
    {
        CardUpdatedTrigger trigger = new CardUpdatedTrigger();
        Assert.False(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardEmailed)));
    }
}
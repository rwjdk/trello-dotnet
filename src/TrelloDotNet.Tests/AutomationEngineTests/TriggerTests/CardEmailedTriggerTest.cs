using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class CardEmailedTriggerTest : TestBase
{
    [Fact]
    public async Task TriggerTrue()
    {
        CardEmailedTrigger trigger = new CardEmailedTrigger();
        Assert.True(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardEmailed)));
    }

    [Fact]
    public async Task TriggerFalse()
    {
        CardEmailedTrigger trigger = new CardEmailedTrigger();
        Assert.False(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList)));
    }
}
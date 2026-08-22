using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class CardCreatedTriggerTest : TestBase
{
    [Fact]
    public async Task TriggerTrue()
    {
        CardCreatedTrigger trigger = new CardCreatedTrigger();
        Assert.True(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardCreated)));
    }

    [Fact]
    public async Task TriggerFalse()
    {
        CardCreatedTrigger trigger = new CardCreatedTrigger();
        Assert.False(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList)));
    }
}
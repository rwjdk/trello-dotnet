using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class CardMovedToBoardTriggerTest : TestBase
{
    [Fact]
    public async Task TriggerTrue()
    {
        CardMovedToBoardTrigger trigger = new CardMovedToBoardTrigger();
        Assert.True(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.CardMovedToBoard)));
    }

    [Fact]
    public async Task TriggerFalse()
    {
        CardMovedToBoardTrigger trigger = new CardMovedToBoardTrigger();
        Assert.False(await trigger.IsTriggerMetAsync(WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList)));
    }
}
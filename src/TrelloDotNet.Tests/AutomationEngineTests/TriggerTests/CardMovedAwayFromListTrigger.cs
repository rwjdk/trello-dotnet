using TrelloDotNet.AutomationEngine.Model;
using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class CardMovedAwayFromListTriggerTest : TestBase
{
    [Fact]
    public async Task NameTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyOfTheseListsAreMovedAwayFrom, webhookAction.Data.ListAfter.Name) { TreatListNameAsId = true };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameFalseTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyOfTheseListsAreMovedAwayFrom, Guid.NewGuid().ToString()) { TreatListNameAsId = true };
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task IdTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyOfTheseListsAreMovedAwayFrom, webhookAction.Data.ListAfter.Id);
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameStartWithTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyOfTheseListsAreMovedAwayFrom, "My")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.StartsWith
        };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameStartWithTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyButTheseListsAreMovedAwayFrom, "My")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.StartsWith
        };
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameEndsWithTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyOfTheseListsAreMovedAwayFrom, "st")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.EndsWith
        };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameEndsWithTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyButTheseListsAreMovedAwayFrom, "st")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.EndsWith
        };
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameContainsTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyOfTheseListsAreMovedAwayFrom, "yLi")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.Contains
        };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameContainsTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyButTheseListsAreMovedAwayFrom, "yLi")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.Contains
        };
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameRegExTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyOfTheseListsAreMovedAwayFrom, "M\\w+t")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.RegEx
        };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameRegExTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyButTheseListsAreMovedAwayFrom, "M\\w+t")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.RegEx
        };
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task IdFalseTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyOfTheseListsAreMovedAwayFrom, Guid.NewGuid().ToString());
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButThisFalseTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyButTheseListsAreMovedAwayFrom, webhookAction.Data.ListAfter.Id);
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButThisTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyButTheseListsAreMovedAwayFrom, Guid.NewGuid().ToString());
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task TriggerException()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedAwayFromListTriggerConstraint constraint = Enum.Parse<CardMovedAwayFromListTriggerConstraint>("99");
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(constraint, Guid.NewGuid().ToString());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task WrongWebActionException()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.NoListBefore);
        CardMovedAwayFromListTrigger trigger = new CardMovedAwayFromListTrigger(CardMovedAwayFromListTriggerConstraint.AnyOfTheseListsAreMovedAwayFrom, "MyList");
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }
}
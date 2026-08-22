using TrelloDotNet.AutomationEngine.Model;
using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class CardMovedToListTriggerTests : TestBase
{
    [Fact]
    public async Task NameTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, webhookAction.Data.ListAfter.Name) { TreatListNameAsId = true };
        Assert.True(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameFalseTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, Guid.NewGuid().ToString()) { TreatListNameAsId = true };
        Assert.False(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task IdTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, webhookAction.Data.ListAfter.Id);
        Assert.True(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameStartWithTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, "My")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.StartsWith
        };
        Assert.True(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameStartWithTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyButTheseListsAreMovedTo, "My")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.StartsWith
        };
        Assert.False(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameEndsWithTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, "st")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.EndsWith
        };
        Assert.True(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameEndsWithTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyButTheseListsAreMovedTo, "st")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.EndsWith
        };
        Assert.False(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameContainsTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, "yLi")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.Contains
        };
        Assert.True(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameContainsTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyButTheseListsAreMovedTo, "yLi")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.Contains
        };
        Assert.False(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameRegExTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, "M\\w+t")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.RegEx
        };
        Assert.True(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameRegExTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyButTheseListsAreMovedTo, "M\\w+t")
        {
            TreatListNameAsId = true,
            ListNameMatchCriteria = StringMatchCriteria.RegEx
        };
        Assert.False(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task IdFalseTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, Guid.NewGuid().ToString());
        Assert.False(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButThisFalseTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyButTheseListsAreMovedTo, webhookAction.Data.ListAfter.Id);
        Assert.False(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButThisTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyButTheseListsAreMovedTo, Guid.NewGuid().ToString());
        Assert.True(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task TriggerException()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList);
        CardMovedToListTriggerConstraint constraint = Enum.Parse<CardMovedToListTriggerConstraint>("99");
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(constraint, Guid.NewGuid().ToString());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task WrongWebActionException()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.NoListAfter);
        CardMovedToListTrigger cardMovedToListTrigger = new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, "MyList");
        Assert.False(await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }
}
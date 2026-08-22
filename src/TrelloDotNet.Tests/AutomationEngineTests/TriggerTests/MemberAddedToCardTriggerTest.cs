using TrelloDotNet.AutomationEngine.Model.Triggers;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.AutomationEngineTests.TriggerTests;

public class MemberAddedToCardTriggerTest : TestBase
{
    [Fact]
    public async Task NameAnyTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MemberAddedToCard);
        MemberAddedToCardTrigger trigger = new MemberAddedToCardTrigger(MemberAddedToCardTriggerConstraint.AnyMember) { TreatMemberNameAsId = true };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task NameTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MemberAddedToCard);
        MemberAddedToCardTrigger trigger = new MemberAddedToCardTrigger(MemberAddedToCardTriggerConstraint.AnyOfTheseMembersAreAdded, webhookAction.Data.Member.Name) { TreatMemberNameAsId = true };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task AnyButNameTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MemberAddedToCard);
        MemberAddedToCardTrigger trigger = new MemberAddedToCardTrigger(MemberAddedToCardTriggerConstraint.AnyButTheseMembersAreAreAdded, webhookAction.Data.Member.Name) { TreatMemberNameAsId = true };
        Assert.False(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task IdTrueTrigger()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MemberAddedToCard);
        MemberAddedToCardTrigger trigger = new MemberAddedToCardTrigger(MemberAddedToCardTriggerConstraint.AnyOfTheseMembersAreAdded, webhookAction.Data.Member.Id) { TreatMemberNameAsId = false };
        Assert.True(await trigger.IsTriggerMetAsync(webhookAction));
    }

    [Fact]
    public async Task TriggerException()
    {
        WebhookAction webhookAction = WebhookAction.CreateDummy(TrelloClient, WebhookAction.WebhookActionDummyCreationScenario.MemberAddedToCard);
        MemberAddedToCardTriggerConstraint constraint = Enum.Parse<MemberAddedToCardTriggerConstraint>("99");
        MemberAddedToCardTrigger cardMovedToListTrigger = new MemberAddedToCardTrigger(constraint, Guid.NewGuid().ToString());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await cardMovedToListTrigger.IsTriggerMetAsync(webhookAction));
    }
}
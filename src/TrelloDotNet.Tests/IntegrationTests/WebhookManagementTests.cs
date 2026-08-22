using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.IntegrationTests;

[Collection("Webhook Management")] //In own collection to not overlap other tests
public class WebhookManagementTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly string? _boardId = fixture.BoardId;

    [Fact]
    public async Task WebHookCrud()
    {
        //Find current webhooks
        List<Webhook> currentWebhooks = await TrelloClient.GetWebhooksForCurrentTokenAsync();

        //Add Webhook
        const string callbackUrl = "https://trello.com";
        string description = Guid.NewGuid().ToString();
        Webhook webhook = new Webhook(description, callbackUrl, _boardId);
        Webhook addedWebHook = await TrelloClient.AddWebhookAsync(webhook);
        Assert.Equal(callbackUrl, addedWebHook.CallbackUrl);
        Assert.Equal(description, addedWebHook.Description);
        Assert.True(addedWebHook.Active);
        string webhookId = addedWebHook.Id;

        //Update Webhook
        string updatedDescription = Guid.NewGuid().ToString();
        addedWebHook.Description = updatedDescription;
        addedWebHook.Active = false;
        Webhook updatedWebHook = await TrelloClient.UpdateWebhookAsync(addedWebHook);
        Assert.Equal(callbackUrl, updatedWebHook.CallbackUrl);
        Assert.Equal(updatedDescription, updatedWebHook.Description);
        Assert.False(addedWebHook.Active);

        //Check lists of webhooks are update
        List<Webhook> webhooksAfterAdd = await TrelloClient.GetWebhooksForCurrentTokenAsync();

        Assert.Equal(currentWebhooks.Count + 1, webhooksAfterAdd.Count);
        Assert.Equal(webhookId, webhooksAfterAdd.First(x => x.Id == webhookId).Id);
        Assert.Equal(updatedDescription, webhooksAfterAdd.First(x => x.Id == webhookId).Description);
        Assert.Equal(callbackUrl, webhooksAfterAdd.First(x => x.Id == webhookId).CallbackUrl);

        //Get Webhook
        Webhook getWebhook = await TrelloClient.GetWebhookAsync(webhookId);
        Assert.Equal(getWebhook.Id, updatedWebHook.Id);
        Assert.Equal(getWebhook.Description, updatedWebHook.Description);
        Assert.Equal(getWebhook.CallbackUrl, updatedWebHook.CallbackUrl);

        //Delete Webhook
        await TrelloClient.DeleteWebhookAsync(webhookId);
        List<Webhook> webhooksAfterDelete = await TrelloClient.GetWebhooksForCurrentTokenAsync();
        Assert.Equal(currentWebhooks.Count, webhooksAfterDelete.Count);

        //Update Webhook by URL
        await TrelloClient.AddWebhookAsync(new Webhook("ByCallBack", callbackUrl, _boardId));
        const string newCallbackUrl = "https://www.rwj.dk";
        await TrelloClient.UpdateWebhookByCallbackUrlAsync(callbackUrl, newCallbackUrl);
        List<Webhook> webhooksAfterChangeByCallback = await TrelloClient.GetWebhooksForCurrentTokenAsync();
        Assert.Contains(newCallbackUrl, webhooksAfterChangeByCallback.Select(x => x.CallbackUrl));

        //Delete Webhook by URL
        await TrelloClient.DeleteWebhooksByCallbackUrlAsync(newCallbackUrl);
        List<Webhook> webhooksAfterDeleteByCallback = await TrelloClient.GetWebhooksForCurrentTokenAsync();
        Assert.Equal(webhooksAfterChangeByCallback.Count - 1, webhooksAfterDeleteByCallback.Count);

        await TrelloClient.AddWebhookAsync(new Webhook("DeleteById", callbackUrl, _boardId));
        await TrelloClient.DeleteWebhooksByTargetModelIdAsync(_boardId);
        List<Webhook> webhooksAfterDeleteById = await TrelloClient.GetWebhooksForCurrentTokenAsync();
        Assert.Equal(webhooksAfterDeleteByCallback.Count, webhooksAfterDeleteById.Count);
    }
}
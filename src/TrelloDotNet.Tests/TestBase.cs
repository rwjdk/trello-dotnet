using Microsoft.Extensions.Configuration;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.AddCardOptions;

namespace TrelloDotNet.Tests;

public abstract class TestBase
{
    public TrelloClient TrelloClient;

    protected TestBase()
    {
        TrelloClient = GetClient();
    }

    private TrelloClient GetClient()
    {
        try
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<TestBase>()
                .Build();

            string? apiKey = config["TrelloApiKey"];
            string? token = config["TrelloToken"];
            TrelloClientOptions trelloClientOptions = new TrelloClientOptions
            {
                MaxRetryCountForTokenLimitExceeded = 10,
                DelayInSecondsToWaitInTokenLimitExceededRetry = 2
            };
            return new TrelloClient(apiKey, token, trelloClientOptions);
        }
        catch (Exception)
        {
            throw new Exception("In order to run Unit-tests you need to add a user secrets 'TrelloApiKey' and 'TrelloToken' (both strings). See more here: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows#manage-user-secrets-with-visual-studio");
        }
    }

    protected async Task<List> AddDummyList(string boardId, string? name = null)
    {
        return await TrelloClient.AddListAsync(new List(name ?? Guid.NewGuid().ToString(), boardId));
    }

    protected async Task<Card> AddDummyCard(string boardId, string? name = null)
    {
        return (await AddDummyCardAndList(boardId, name)).Card;
    }

    protected async Task<(List List, Card Card)> AddDummyCardAndList(string boardId, string? name = null)
    {
        List list = await AddDummyList(boardId, name);
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, name ?? Guid.NewGuid().ToString()));
        return (list, card);
    }

    public void AssertTimeIsNow(DateTimeOffset? objectCreationTime)
    {
        bool beforeNow = objectCreationTime < DateTimeOffset.UtcNow.AddMinutes(1);
        bool afterAMinuteAgo = objectCreationTime > DateTimeOffset.UtcNow.AddMinutes(-1);
        Assert.True(beforeNow && afterAMinuteAgo);
    }
}
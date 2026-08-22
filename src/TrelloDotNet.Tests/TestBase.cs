using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Options.GetBoardOptions;

namespace TrelloDotNet.Tests;

public abstract class TestBase
{
    public TrelloClient TrelloClient;

    protected CancellationToken TestCancellationToken => TestContext.Current?.CancellationToken ?? CancellationToken.None;

    protected TestBase()
    {
        TrelloClient = GetClient();
    }

    protected TestBase(TrelloClient trelloClient)
    {
        TrelloClient = trelloClient;
    }

    private TrelloClient GetClient()
    {
        try
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<TestBase>()
                .Build();

            List<TrelloClient> clients = [];
            string? apiKey = config["TrelloApiKey"];
            string? token = config["TrelloToken"];
            if (!string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(token))
            {
                TrelloClientOptions trelloClientOptions = new TrelloClientOptions
                {
                    MaxRetryCountForTokenLimitExceeded = 10,
                    DelayInSecondsToWaitInTokenLimitExceededRetry = 3,
                };
                clients.Add(new TrelloClient(apiKey, token, trelloClientOptions, new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5)
                }));
            }

            for (int i = 1; i < 10; i++)
            {
                apiKey = config["TrelloApiKey" + (i + 1)];
                token = config["TrelloToken" + (i + 1)];
                if (!string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(token))
                {
                    TrelloClientOptions trelloClientOptions = new TrelloClientOptions
                    {
                        MaxRetryCountForTokenLimitExceeded = 10,
                        DelayInSecondsToWaitInTokenLimitExceededRetry = 3
                    };
                    clients.Add(new TrelloClient(apiKey, token, trelloClientOptions));
                }
            }

            return clients[Random.Shared.Next(clients.Count)];
        }
        catch (Exception)
        {
            throw new Exception("In order to run Unit-tests you need to add a user secrets 'TrelloApiKey' and 'TrelloToken' (both strings). See more here: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows#manage-user-secrets-with-visual-studio");
        }
    }

    protected async Task<List> AddDummyList(string boardId, string? name = null)
    {
        return await TrelloClient.AddListAsync(new List(name ?? Guid.NewGuid().ToString(), boardId), TestCancellationToken);
    }

    protected async Task<Card> AddDummyCard(string boardId, string? name = null)
    {
        return (await AddDummyCardAndList(boardId, name)).Card;
    }

    protected async Task<Card> AddDummyCardToList(List list, string? name = null, string? description = null, DateTimeOffset? start = null, DateTimeOffset? due = null, bool? dueComplete = null)
    {
        AddCardOptions addCardOptions = new AddCardOptions(list.Id, name ?? Guid.NewGuid().ToString(), description ?? string.Empty);
        if (start.HasValue)
        {
            addCardOptions.Start = start.Value;
        }

        if (due.HasValue)
        {
            addCardOptions.Due = due.Value;
        }

        if (dueComplete.HasValue)
        {
            addCardOptions.DueComplete = dueComplete.Value;
        }

        return await TrelloClient.AddCardAsync(addCardOptions, TestCancellationToken);
    }

    protected async Task<(List List, Card Card)> AddDummyCardAndList(string boardId, string? name = null)
    {
        List list = await AddDummyList(boardId, name);
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, name ?? Guid.NewGuid().ToString()), TestCancellationToken);
        return (list, card);
    }

    protected async Task<TemporaryBoardContext> CreateTemporaryBoardAsync(string? scenarioName = null, string? description = null)
    {
        string organizationName = $"UnitTestOrganization-{scenarioName ?? "Temp"}-{Guid.NewGuid()}";
        Organization organization = await TrelloClient.AddOrganizationAsync(new Organization(organizationName), TestCancellationToken);

        string boardNameSeed = scenarioName ?? "UnitTestBoard";
        string boardName = $"{boardNameSeed}-{Guid.NewGuid()}";
        Board? board = await TrelloClient.AddBoardAsync(new Board(boardName, description ?? $"BoardDescription-{boardName}")
        {
            OrganizationId = organization.Id
        }, cancellationToken: TestCancellationToken);

        return new TemporaryBoardContext(TrelloClient, board, organization);
    }

    public void AssertTimeIsNow(DateTimeOffset? objectCreationTime)
    {
        bool beforeNow = objectCreationTime < DateTimeOffset.UtcNow.AddMinutes(1);
        bool afterAMinuteAgo = objectCreationTime > DateTimeOffset.UtcNow.AddMinutes(-1);
        Assert.True(beforeNow && afterAMinuteAgo);
    }

    public async Task<Board?> GetSpecialPaidSubscriptionBoard()
    {
        List<Board>? availableBoards = await TrelloClient.GetBoardsCurrentTokenCanAccessAsync(new GetBoardOptions
        {
            BoardFields = new BoardFields(BoardFieldsType.Name)
        }, cancellationToken: TestCancellationToken);

        const string specialSetupBoardsForTheseTests = "67c765705dc85a158981d888";
        return availableBoards.FirstOrDefault(x => x.Id == specialSetupBoardsForTheseTests);
    }
}

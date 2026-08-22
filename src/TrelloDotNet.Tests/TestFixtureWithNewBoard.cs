using TrelloDotNet.Model;

namespace TrelloDotNet.Tests;

public class TestFixtureWithNewBoard : TestBase, IAsyncLifetime
{
    public Board? Board { get; set; }
    public string? BoardId { get; set; }
    public string? BoardName { get; set; }
    public string? BoardDescription { get; set; }
    public Organization? Organization { get; set; }
    public string? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }

    public virtual async ValueTask InitializeAsync()
    {
        Assert.True(TrelloClient.Options.MaxRetryCountForTokenLimitExceeded > 0);
        Assert.True(TrelloClient.Options.DelayInSecondsToWaitInTokenLimitExceededRetry > 0);

        string organizationName = Guid.NewGuid().ToString();
        OrganizationName = $"UnitTestOrganization-{organizationName}";
        Organization = await TrelloClient.AddOrganizationAsync(new Organization(OrganizationName), cancellationToken: TestCancellationToken);
        OrganizationId = Organization.Id;
        Assert.Equal(OrganizationName, Organization.DisplayName);

        string boardName = Guid.NewGuid().ToString();
        BoardName = $"UnitTestBoard-{boardName}";
        BoardDescription = $"BoardDescription-{boardName}";
        Board board = new Board(BoardName, BoardDescription)
        {
            OrganizationId = Organization.Id
        };
        Board = await TrelloClient.AddBoardAsync(board, cancellationToken: TestCancellationToken);
        BoardId = Board.Id;
        Assert.Equal(BoardName, Board.Name);
        Assert.Equal(BoardDescription, Board.Description);
        Assert.Equal(OrganizationId, Board.OrganizationId);
    }

    public virtual async ValueTask DisposeAsync()
    {
        Exception? cleanupException = null;
        using CancellationTokenSource cleanupCancellation = new(TimeSpan.FromSeconds(30));

        try
        {
            TrelloClient.Options.AllowDeleteOfBoards = true;
            if (BoardId != null)
            {
                await TrelloClient.DeleteBoardAsync(BoardId, cancellationToken: cleanupCancellation.Token);
            }
        }
        catch (Exception e)
        {
            cleanupException = e;
        }
        finally
        {
            TrelloClient.Options.AllowDeleteOfBoards = false;
        }

        try
        {
            TrelloClient.Options.AllowDeleteOfOrganizations = true;
            if (OrganizationId != null)
            {
                await TrelloClient.DeleteOrganizationAsync(OrganizationId, cancellationToken: cleanupCancellation.Token);
            }
        }
        catch (Exception e)
        {
            cleanupException = cleanupException == null ? e : new AggregateException(cleanupException, e);
        }
        finally
        {
            TrelloClient.Options.AllowDeleteOfOrganizations = false;
        }

        if (cleanupException != null)
        {
            throw cleanupException;
        }
    }
}

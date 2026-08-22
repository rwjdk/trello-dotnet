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

    public async Task InitializeAsync()
    {
        Assert.True(TrelloClient.Options.MaxRetryCountForTokenLimitExceeded > 0);
        Assert.True(TrelloClient.Options.DelayInSecondsToWaitInTokenLimitExceededRetry > 0);

        string organizationName = Guid.NewGuid().ToString();
        OrganizationName = $"UnitTestOrganization-{organizationName}";
        Organization = await TrelloClient.AddOrganizationAsync(new Organization(OrganizationName));
        OrganizationId = Organization.Id;
        Assert.Equal(OrganizationName, Organization.DisplayName);

        string boardName = Guid.NewGuid().ToString();
        BoardName = $"UnitTestBoard-{boardName}";
        BoardDescription = $"BoardDescription-{boardName}";
        Board board = new Board(BoardName, BoardDescription)
        {
            OrganizationId = Organization.Id
        };
        Board = await TrelloClient.AddBoardAsync(board);
        BoardId = Board.Id;
        Assert.Equal(BoardName, Board.Name);
        Assert.Equal(BoardDescription, Board.Description);
        Assert.Equal(OrganizationId, Board.OrganizationId);
    }

    public async Task DisposeAsync()
    {
        try
        {
            await TrelloClient.DeleteBoardAsync(BoardId);
        }
        catch (Exception e)
        {
            Assert.Contains("Deletion of Boards are disabled via Options.AllowDeleteOfBoards (You need to enable this as a secondary confirmation that you REALLY wish to use that option as there is no going back: https://support.atlassian.com/trello/docs/deleting-a-board/)", e.Message);
        }
        finally
        {
            TrelloClient.Options.AllowDeleteOfBoards = true;
            await TrelloClient.DeleteBoardAsync(BoardId);
        }

        try
        {
            await TrelloClient.DeleteOrganizationAsync(OrganizationId);
        }
        catch (Exception e)
        {
            Assert.Contains("Deletion of Organizations are disabled via Options.AllowDeleteOfOrganizations (You need to enable this as a secondary confirmation that you REALLY wish to use that option as there is no going back)", e.Message);
        }
        finally
        {
            TrelloClient.Options.AllowDeleteOfOrganizations = true;
            await TrelloClient.DeleteOrganizationAsync(OrganizationId);
        }

        TrelloClient.Options.AllowDeleteOfBoards = false;
        TrelloClient.Options.AllowDeleteOfOrganizations = false;
    }
}
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.AddCardToInboxOptions;
using TrelloDotNet.Model.Options.GetInboxCardOptions;

namespace TrelloDotNet.Tests.IntegrationTests;

public class InboxTests : TestBase
{
    /*
    [Fact]
    public async Task GetInbox()
    {
        TokenMemberInbox memberInbox = await TrelloClient.GetTokenMemberInboxAsync(cancellationToken: TestCancellationToken);
        if (memberInbox != null) //Needed for now as it is a beta feature
        {
            Assert.NotNull(memberInbox.ListId);
            Assert.NotNull(memberInbox.BoardId);
            Assert.NotNull(memberInbox.OrganizationId);
        }
    }
    
    [Fact]
    public async Task AddCardToInbox()
    {
        TokenMemberInbox memberInbox = await TrelloClient.GetTokenMemberInboxAsync(cancellationToken: TestCancellationToken);

        if (memberInbox != null) //Needed for now as it is a beta feature
        {
            Card? card = null;
            try
            {
                const string name = "Hello";
                card = await TrelloClient.AddCardToInboxAsync(new AddCardToInboxOptions
                {
                    Name = name
                }, cancellationToken: TestCancellationToken);
                Assert.Equal(name, card.Name);
            }
            finally
            {
                if (card != null)
                {
                    await TrelloClient.DeleteCardAsync(card.Id, cancellationToken: TestCancellationToken);
                }
            }
        }
    }
    
    [Fact]
    public async Task GetCardsToInbox()
    {
        TokenMemberInbox memberInbox = await TrelloClient.GetTokenMemberInboxAsync(cancellationToken: TestCancellationToken);
        if (memberInbox != null) //Needed for now as it is a beta feature
        {
            List<Card>? cards = await TrelloClient.GetCardsInInboxAsync(cancellationToken: TestCancellationToken);
            Assert.NotNull(cards);
            cards = await TrelloClient.GetCardsInInboxAsync(new GetInboxCardOptions
            {
                Limit = 1
            }, cancellationToken: TestCancellationToken);
            Assert.NotNull(cards);
        }
    }
    */    
}
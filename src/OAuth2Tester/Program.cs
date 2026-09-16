using TrelloDotNet;
using TrelloDotNet.Model;

string clientId = "vnSc6oWidNZRYy9tNz1ImuQ4ZU8g1hFc"; //todo: Provide your own
string? refreshToken = null;
refreshToken = await ReadPreviouslyStoredTokenAsync();

if (refreshToken == null)
{
    //Start interactive OAuth Flow
    string redirectUri = "https://localhost:7248/oauth/callback"; //todo: Provide your own
    TrelloOAuth2AuthorizationRequest request = TrelloOAuth2Flow.CreateAuthorizationRequest(
        clientId,
        redirectUri,
        [TrelloOAuth2Scope.ReadBoard], //todo - add needed scopes
        generateRefreshToken: true);

    Console.WriteLine("Open this URL in the Browser and after login, give back the return URL");
    Console.WriteLine("---");
    Console.WriteLine(request.AuthorizationUri);
    Console.WriteLine("---");

    Console.Write("Url back: ");
    string? callbackUrl = Console.ReadLine();
    if (callbackUrl != null)
    {
        (string code, string state) = TrelloOAuth2Flow.ParseCallbackUrl(callbackUrl);
        if (!string.Equals(state, request.State, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("OAuth state validation failed.");
        }
        TrelloOAuth2TokenResponse token = await TrelloOAuth2Flow.ExchangeCodeAsync(clientId, code, request.CodeVerifier, redirectUri);
        refreshToken = token.RefreshToken ?? throw new InvalidOperationException("No refresh token was returned.");
        await StoreTokenAsync(refreshToken);
    }
}

if (refreshToken != null)
{
    TrelloClient client = TrelloClient.FromOAuth2RefreshToken(clientId, refreshToken, async newRefreshToken =>
    {
        //As a refresh token can only be used once it is important that you store it; else you need to do an interactive login
        await StoreTokenAsync(newRefreshToken); //Tip: Use Polly or similar for retry so intermittent issues do not cause you to use Refresh Token
    });

    List<Board> boards = await client.GetBoardsCurrentTokenCanAccessAsync();

    string cardId = "6882cca471fe0117730741f6"; //todo: Provide your own
    Card card = await client.GetCardAsync(cardId);
    Console.WriteLine(card.Name);
}

async Task StoreTokenAsync(string newRefreshToken)
{
    string refreshTokenPath = GetRefreshTokenPath();
    await File.WriteAllTextAsync(refreshTokenPath, newRefreshToken);
}

async Task<string?> ReadPreviouslyStoredTokenAsync()
{
    string refreshTokenPath = GetRefreshTokenPath();
    return File.Exists(refreshTokenPath) ? await File.ReadAllTextAsync(refreshTokenPath) : null;
}

string GetRefreshTokenPath()
{
    return Path.Combine(Path.GetTempPath(), "refreshToken.txt");
}

using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class TrelloOAuth2FlowTests
{
    [Fact]
    public void CreateAuthorizationRequest_CreatesValidPkceRequest()
    {
        TrelloOAuth2AuthorizationRequest request = TrelloOAuth2Flow.CreateAuthorizationRequest(
            "client-id",
            "https://example.com/oauth/callback",
            new[] { TrelloOAuth2Scope.ReadMember },
            generateRefreshToken: true);

        Dictionary<string, string> parameters = ParseQueryString(request.AuthorizationUri.Query);
        Assert.Equal("https://auth.atlassian.com/authorize", request.AuthorizationUri.GetLeftPart(UriPartial.Path));
        Assert.Equal("client-id", parameters["client_id"]);
        Assert.Equal("read:member:trello offline_access", parameters["scope"]);
        Assert.Equal("https://example.com/oauth/callback", parameters["redirect_uri"]);
        Assert.Equal("code", parameters["response_type"]);
        Assert.Equal("consent", parameters["prompt"]);
        Assert.Equal("S256", parameters["code_challenge_method"]);
        Assert.False(string.IsNullOrWhiteSpace(request.State));
        Assert.Equal(request.State, parameters["state"]);
        Assert.Equal(CreateCodeChallenge(request.CodeVerifier), parameters["code_challenge"]);
        Assert.InRange(request.CodeVerifier.Length, 43, 128);
    }

    [Fact]
    public void CreateAuthorizationRequest_OmitsOfflineAccessWhenRefreshTokenIsNotRequested()
    {
        TrelloOAuth2AuthorizationRequest request = TrelloOAuth2Flow.CreateAuthorizationRequest(
            "client-id",
            "https://example.com/oauth/callback",
            new[] { TrelloOAuth2Scope.ReadBoard, TrelloOAuth2Scope.WriteBoard },
            generateRefreshToken: false);

        Dictionary<string, string> parameters = ParseQueryString(request.AuthorizationUri.Query);
        Assert.Equal("read:board:trello write:board:trello", parameters["scope"]);
    }

    [Fact]
    public void CreateAuthorizationRequest_RequestsRefreshTokenByDefault()
    {
        TrelloOAuth2AuthorizationRequest request = TrelloOAuth2Flow.CreateAuthorizationRequest(
            "client-id",
            "https://example.com/oauth/callback",
            new[] { TrelloOAuth2Scope.ReadBoard });

        Dictionary<string, string> parameters = ParseQueryString(request.AuthorizationUri.Query);
        Assert.Equal("read:board:trello offline_access", parameters["scope"]);
    }

    [Fact]
    public void ParseCallbackUrl_ReturnsDecodedCodeAndState()
    {
        (string code, string state) = TrelloOAuth2Flow.ParseCallbackUrl(
            "https://localhost:7248/oauth/callback?state=state%2Bvalue&code=code%2Fvalue");

        Assert.Equal("code/value", code);
        Assert.Equal("state+value", state);
    }

    [Theory]
    [InlineData("https://localhost:7248/oauth/callback?state=state", "authorization code")]
    [InlineData("https://localhost:7248/oauth/callback?code=code", "state")]
    public void ParseCallbackUrl_RejectsMissingRequiredParameter(string callbackUrl, string expectedMessage)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            TrelloOAuth2Flow.ParseCallbackUrl(callbackUrl));

        Assert.Contains(expectedMessage, exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ParseCallbackUrl_ReportsOAuthError()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            TrelloOAuth2Flow.ParseCallbackUrl(
                "https://localhost:7248/oauth/callback?error=access_denied&error_description=The+user+denied+access"));

        Assert.Contains("access_denied: The user denied access", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExchangeCodeAsync_PostsConfidentialClientPayload()
    {
        TokenEndpointHandler handler = new TokenEndpointHandler();
        using HttpClient httpClient = new HttpClient(handler);
        TrelloOAuth2TokenResponse token = await TrelloOAuth2Flow.ExchangeCodeAsync(
            "client-id",
            "authorization-code",
            "code-verifier",
            "https://example.com/oauth/callback",
            clientSecret: "client-secret",
            httpClient: httpClient,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("https://auth.atlassian.com/oauth/token", handler.RequestUri?.AbsoluteUri);
        Assert.Equal("application/json", handler.ContentType);
        Assert.Equal("client-id", handler.Payload["client_id"]);
        Assert.Equal("client-secret", handler.Payload["client_secret"]);
        Assert.Equal("authorization_code", handler.Payload["grant_type"]);
        Assert.Equal("authorization-code", handler.Payload["code"]);
        Assert.Equal("code-verifier", handler.Payload["code_verifier"]);
        Assert.Equal("https://example.com/oauth/callback", handler.Payload["redirect_uri"]);
        Assert.Equal("new-access-token", token.AccessToken);
        Assert.Equal("new-refresh-token", token.RefreshToken);
        Assert.Equal(3600, token.ExpiresIn);
    }

    [Fact]
    public async Task RefreshTokenAsync_OmitsSecretForPublicClient()
    {
        TokenEndpointHandler handler = new TokenEndpointHandler();
        using HttpClient httpClient = new HttpClient(handler);
        _ = await TrelloOAuth2Flow.RefreshTokenAsync(
            "client-id",
            "current-refresh-token",
            httpClient: httpClient,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("refresh_token", handler.Payload["grant_type"]);
        Assert.Equal("current-refresh-token", handler.Payload["refresh_token"]);
        Assert.False(handler.Payload.ContainsKey("client_secret"));
    }

    [Fact]
    public async Task RefreshTokenAsync_ThrowsTrelloApiExceptionForErrorResponse()
    {
        TokenEndpointHandler handler = new TokenEndpointHandler(HttpStatusCode.Forbidden, "{\"error\":\"invalid_grant\"}");
        using HttpClient httpClient = new HttpClient(handler);
        TrelloApiException exception = await Assert.ThrowsAsync<TrelloApiException>(() =>
            TrelloOAuth2Flow.RefreshTokenAsync(
                "client-id",
                "expired-refresh-token",
                httpClient: httpClient,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.Forbidden, exception.ErrorCode);
        Assert.Contains("invalid_grant", exception.Message, StringComparison.Ordinal);
    }

    private static Dictionary<string, string> ParseQueryString(string queryString)
    {
        return queryString.TrimStart('?')
            .Split('&')
            .Select(x => x.Split(new[] { '=' }, 2))
            .ToDictionary(x => Uri.UnescapeDataString(x[0]), x => Uri.UnescapeDataString(x[1]));
    }

    private static string CreateCodeChallenge(string codeVerifier)
    {
        byte[] hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Convert.ToBase64String(hash).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private sealed class TokenEndpointHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;

        public TokenEndpointHandler(HttpStatusCode statusCode = HttpStatusCode.OK, string? responseContent = null)
        {
            _statusCode = statusCode;
            _responseContent = responseContent ?? "{\"access_token\":\"new-access-token\",\"refresh_token\":\"new-refresh-token\",\"expires_in\":3600,\"scope\":\"read:member:trello offline_access\"}";
        }

        public Uri? RequestUri { get; private set; }

        public string? ContentType { get; private set; }

        public Dictionary<string, string> Payload { get; private set; } = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            ContentType = request.Content?.Headers.ContentType?.MediaType;
            string json = await request.Content!.ReadAsStringAsync(cancellationToken);
            Payload = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
            return new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };
        }
    }
}

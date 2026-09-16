using System.Threading.Tasks;

namespace TrelloDotNet
{
    /// <summary>
    /// Persists a replacement OAuth 2.0 refresh token after token rotation.
    /// </summary>
    /// <param name="refreshToken">Replacement refresh token that must overwrite the previously stored refresh token</param>
    /// <returns>A task that completes after the replacement refresh token has been securely persisted</returns>
    public delegate Task TrelloOAuth2RefreshTokenSaver(string refreshToken);
}

using System.Threading;
using System.Threading.Tasks;

namespace TrelloDotNet.Control
{
    internal sealed class FixedTrelloOAuth2TokenProvider : ITrelloOAuth2TokenProvider
    {
        private readonly string _accessToken;

        internal FixedTrelloOAuth2TokenProvider(string accessToken)
        {
            _accessToken = accessToken;
        }

        public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_accessToken);
        }
    }
}

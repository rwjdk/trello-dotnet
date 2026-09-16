using System.Threading;
using System.Threading.Tasks;

namespace TrelloDotNet
{
    internal interface ITrelloOAuth2TokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    }
}

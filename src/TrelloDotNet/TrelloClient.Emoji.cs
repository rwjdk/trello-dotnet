using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Model;

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Retrieves the Emoji available in Trello
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Available Emoji</returns>
        public async Task<List<Emoji>> GetAvailableEmojiAsync(CancellationToken cancellationToken = default)
        {
            EmojiResponse response = await _apiRequestController.Get<EmojiResponse>(UrlPaths.Emoji, cancellationToken);
            return response.Trello;
        }
    }
}

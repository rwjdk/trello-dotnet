using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Add a new Webhook
        /// </summary>
        /// <param name="webhook">The Webhook to add</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Webhook</returns>
        public async Task<Webhook> AddWebhookAsync(Webhook webhook, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Post<Webhook>($"{UrlPaths.Webhooks}", cancellationToken, _queryParametersBuilder.GetViaQueryParameterAttributes(webhook));
        }

        /// <summary>
        /// Update a webhook
        /// </summary>
        /// <param name="webhookWithChanges">The Webhook with changes</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The Updated Webhook</returns>
        public async Task<Webhook> UpdateWebhookAsync(Webhook webhookWithChanges, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Webhook>($"{UrlPaths.Webhooks}/{webhookWithChanges.Id}", cancellationToken, _queryParametersBuilder.GetViaQueryParameterAttributes(webhookWithChanges));
        }

        /// <summary>
        /// Delete a Webhook (WARNING: THERE IS NO WAY GOING BACK!!!).
        /// </summary>
        /// <param name="webhookId">The id of the Webhook to Delete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteWebhookAsync(string webhookId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Webhooks}/{webhookId}", cancellationToken, 0);
        }

        /// <summary>
        /// Delete Webhooks using indicated Callback URL (WARNING: THERE IS NO WAY GOING BACK!!!).
        /// </summary>
        /// <param name="callbackUrl">The URL of the callback URL</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteWebhooksByCallbackUrlAsync(string callbackUrl, CancellationToken cancellationToken = default)
        {
            List<Webhook> currentWebhooks = await GetWebhooksForCurrentTokenAsync(cancellationToken);
            foreach (Webhook webhook in currentWebhooks.Where(x => x.CallbackUrl == callbackUrl))
            {
                await DeleteWebhookAsync(webhook.Id, cancellationToken);
            }
        }

        /// <summary>
        /// Delete Webhooks using indicated target ModelId (WARNING: THERE IS NO WAY GOING BACK!!!).
        /// </summary>
        /// <param name="targetIdModel">The Target Model Id (example an ID of a Board)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteWebhooksByTargetModelIdAsync(string targetIdModel, CancellationToken cancellationToken = default)
        {
            List<Webhook> currentWebhooks = await GetWebhooksForCurrentTokenAsync(cancellationToken);
            foreach (Webhook webhook in currentWebhooks.Where(x => x.IdOfTypeYouWishToMonitor == targetIdModel))
            {
                await DeleteWebhookAsync(webhook.Id, cancellationToken);
            }
        }

        /// <summary>
        /// Get Webhooks linked with the current Token used to authenticate with the API
        /// </summary>
        /// <returns>List of Webhooks</returns>
        public async Task<List<Webhook>> GetWebhooksForCurrentTokenAsync(CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Webhook>>(GetUrlBuilder.GetWebhooksForToken(_apiRequestController.Token), cancellationToken);
        }

        /// <summary>
        /// Get a Webhook from its Id
        /// </summary>
        /// <param name="webhookId">Id of the Webhook</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Webhook</returns>
        public async Task<Webhook> GetWebhookAsync(string webhookId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Webhook>(GetUrlBuilder.GetWebhook(webhookId), cancellationToken);
        }

        /// <summary>
        /// Replace callback URL for one or more Webhooks
        /// </summary>
        /// <param name="oldUrl">The old callback URL to find</param>
        /// <param name="newUrl">The new callback URL to replace it with</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task UpdateWebhookByCallbackUrlAsync(string oldUrl, string newUrl, CancellationToken cancellationToken = default)
        {
            List<Webhook> currentWebhooks = await GetWebhooksForCurrentTokenAsync(cancellationToken);
            foreach (Webhook webhook in currentWebhooks.Where(x => x.CallbackUrl == oldUrl))
            {
                webhook.CallbackUrl = newUrl;
                await UpdateWebhookAsync(webhook, cancellationToken);
            }
        }
    }
}
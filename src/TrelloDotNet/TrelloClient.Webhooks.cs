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
        /// Adds a new webhook to Trello.
        /// </summary>
        /// <param name="webhook">The webhook object to add</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The newly created webhook</returns>
        public async Task<Webhook> AddWebhookAsync(Webhook webhook, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Post<Webhook>($"{UrlPaths.Webhooks}", cancellationToken, _queryParametersBuilder.GetViaQueryParameterAttributes(webhook));
        }

        /// <summary>
        /// Updates the properties of an existing webhook.
        /// </summary>
        /// <param name="webhookWithChanges">The webhook object with updated properties</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated webhook</returns>
        public async Task<Webhook> UpdateWebhookAsync(Webhook webhookWithChanges, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Webhook>($"{UrlPaths.Webhooks}/{webhookWithChanges.Id}", cancellationToken, _queryParametersBuilder.GetViaQueryParameterAttributes(webhookWithChanges));
        }

        /// <summary>
        /// Deletes a webhook by its ID. This operation is irreversible.
        /// </summary>
        /// <param name="webhookId">The ID of the webhook to delete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteWebhookAsync(string webhookId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Webhooks}/{webhookId}", cancellationToken, 0);
        }

        /// <summary>
        /// Deletes all webhooks that use the specified callback URL. This operation is irreversible.
        /// </summary>
        /// <param name="callbackUrl">The callback URL to match for deletion</param>
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
        /// Deletes all webhooks that monitor the specified target model ID. This operation is irreversible.
        /// </summary>
        /// <param name="targetIdModel">The target model ID (for example, the ID of a board) to match for deletion</param>
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
        /// Retrieves all webhooks linked to the current token used to authenticate with the API.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of webhooks for the current token</returns>
        public async Task<List<Webhook>> GetWebhooksForCurrentTokenAsync(CancellationToken cancellationToken = default)
        {
            if (_apiRequestController.UsesOAuth2)
            {
                throw new System.NotSupportedException("Trello does not expose an endpoint that lists every webhook belonging to an OAuth 2.0 grant.");
            }

            return await _apiRequestController.Get<List<Webhook>>(GetUrlBuilder.GetWebhooksForToken(_apiRequestController.Token), cancellationToken);
        }

        /// <summary>
        /// Retrieves a webhook by its ID.
        /// </summary>
        /// <param name="webhookId">ID of the webhook to retrieve</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The requested webhook</returns>
        public async Task<Webhook> GetWebhookAsync(string webhookId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Webhook>(GetUrlBuilder.GetWebhook(webhookId), cancellationToken);
        }

        /// <summary>
        /// Replaces the callback URL for all webhooks that use the specified old URL with a new URL.
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

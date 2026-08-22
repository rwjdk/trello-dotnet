using System.Text.Json;
using System.Threading.Tasks;
using TrelloDotNet.Control.Webhook;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet
{
    /// <summary>
    /// Class to help you process a Webhook event but either turning the JSON into classes or fire events based on the JSON given to it
    /// </summary>
    public class WebhookDataReceiver
    {
        private readonly TrelloClient _trelloClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trelloClient">The base TrelloClient</param>
        public WebhookDataReceiver(TrelloClient trelloClient)
        {
            _trelloClient = trelloClient;
        }

        /// <summary>
        /// Class that can turn data from an into events based on what the data contain
        /// </summary>
        /// <param name="json">The raw incoming JSON</param>
        /// <param name="signature">Signature from X-Trello-Webhook header for signature validation</param>
        /// <param name="webhookUrl">Webhook URL for signature validation</param>
        /// <returns>If the Event was processed (aka it was a supported event)</returns>
        public async void ProcessJsonIntoEvents(string json, string signature = null, string webhookUrl = null)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return; // Most Likely the Head Call when setting up webhook - Just ignore
            }

            if ((!string.IsNullOrEmpty(signature) || !string.IsNullOrEmpty(webhookUrl))
                && !WebhookSignatureValidator.ValidateSignature(json, signature, webhookUrl, _trelloClient.Options.Secret))
            {
                return; // Invalid signature
            }

            WebhookNotification webhookNotification = JsonSerializer.Deserialize<WebhookNotification>(json);
            BasicEvents.FireEvent(webhookNotification.Action);
            await SmartEvents.FireEvent(webhookNotification.Action, _trelloClient);
        }

        /// <summary>
        /// Convert JSON for the webhook into a Notification
        /// </summary>
        /// <param name="json">Raw JSON from the webhook</param>
        /// <returns>C# Class representing the JSON</returns>
        public WebhookNotification ConvertJsonToWebhookNotification(string json)
        {
            WebhookNotification result = JsonSerializer.Deserialize<WebhookNotification>(json);
            SetTrelloClientAndParents(result.Action);
            return result;
        }

        private void SetTrelloClientAndParents(WebhookAction action)
        {
            action.TrelloClient = _trelloClient;
            action.Data.Parent = action;
            if (action.Data.Board != null)
            {
                action.Data.Board.Parent = action.Data;
            }

            if (action.Data.Card != null)
            {
                action.Data.Card.Parent = action.Data;
            }

            if (action.Data.List != null)
            {
                action.Data.List.Parent = action.Data;
            }

            if (action.Data.ListAfter != null)
            {
                action.Data.ListAfter.Parent = action.Data;
            }

            if (action.Data.ListBefore != null)
            {
                action.Data.ListBefore.Parent = action.Data;
            }

            if (action.Data.Checklist != null)
            {
                action.Data.Checklist.Parent = action.Data;
            }

            if (action.Data.Member != null)
            {
                action.Data.Member.Parent = action.Data;
            }
        }

        /// <summary>
        /// Convert JSON for the webhook into a Board Notification (require that you webhook set it idModel to a BoardId)
        /// </summary>
        /// <param name="json">Raw JSON from the webhook</param>
        /// <returns>C# Class representing the JSON</returns>
        public WebhookNotificationBoard ConvertJsonToWebhookNotificationBoard(string json)
        {
            WebhookNotificationBoard result = JsonSerializer.Deserialize<WebhookNotificationBoard>(json);
            SetTrelloClientAndParents(result.Action);
            return result;
        }

        /// <summary>
        /// Convert JSON for the webhook into a Card Notification (require that you webhook set it idModel to a CardId)
        /// </summary>
        /// <param name="json">Raw JSON from the webhook</param>
        /// <returns>C# Class representing the JSON</returns>
        public WebhookNotificationCard ConvertJsonToWebhookNotificationCard(string json)
        {
            WebhookNotificationCard result = JsonSerializer.Deserialize<WebhookNotificationCard>(json);
            SetTrelloClientAndParents(result.Action);
            return result;
        }

        /// <summary>
        /// Convert JSON for the webhook into a List Notification (require that you webhook set it idModel to a ListId)
        /// </summary>
        /// <param name="json">Raw JSON from the webhook</param>
        /// <returns>C# Class representing the JSON</returns>
        public WebhookNotificationList ConvertJsonToWebhookNotificationList(string json)
        {
            WebhookNotificationList result = JsonSerializer.Deserialize<WebhookNotificationList>(json);
            SetTrelloClientAndParents(result.Action);
            return result;
        }

        /// <summary>
        /// Convert JSON for the webhook into a Member Notification (require that you webhook set it idModel to a MemberId)
        /// </summary>
        /// <param name="json">Raw JSON from the webhook</param>
        /// <returns>C# Class representing the JSON</returns>
        public WebhookNotificationMember ConvertJsonToWebhookNotificationMember(string json)
        {
            WebhookNotificationMember result = JsonSerializer.Deserialize<WebhookNotificationMember>(json);
            SetTrelloClientAndParents(result.Action);
            return result;
        }

        /// <summary>
        /// Basic Webhook event where the arguments are generic (aka support everything but there is more work for you to use the event)
        /// </summary>
        public WebhookDataReceiverBasicEvents BasicEvents { get; } = new WebhookDataReceiverBasicEvents();

        /// <summary>
        /// Curated common events that most integrations need (aka much easier to use, but might not have the specific type event you need (ask on GitHub for more events)) 
        /// </summary>
        public WebhookDataReceiverSmartEvents SmartEvents { get; } = new WebhookDataReceiverSmartEvents();
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Actions
{
    /// <summary>
    /// This Automation Action removes a specific Sticker from a card based on imageId
    /// </summary>
    public class RemoveStickerFromCardAction : IAutomationAction
    {
        /// <summary>
        /// ImageId of Sticker to Remove
        /// </summary>
        public string StickerImageIdToRemove { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stickerImageIdToRemove">ImageId of Sticker to Remove</param>
        public RemoveStickerFromCardAction(string stickerImageIdToRemove)
        {
            StickerImageIdToRemove = stickerImageIdToRemove;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stickerToRemove">ImageIdEnum of Default Sticker to Remove</param>
        public RemoveStickerFromCardAction(StickerDefaultImageId stickerToRemove)
        {
            StickerImageIdToRemove = Sticker.DefaultImageToString(stickerToRemove);
        }

        /// <summary>
        /// The method called when an automation should be performed
        /// </summary>
        /// <param name="webhookAction">The Webhook Action that led to the Execution. This object also have the TrelloClient and information about the event at your disposal</param>
        /// <param name="processingResult">An object you can use to report back to the user if the action was performed and details about it</param>
        /// <returns>Void</returns>
        public async Task PerformActionAsync(WebhookAction webhookAction, ProcessingResult processingResult)
        {
            if (webhookAction.Data?.Card == null)
            {
                throw new AutomationException("Could not perform RemoveStickerFromCardAction as WebhookAction did not involve a Card");
            }

            TrelloClient trelloClient = webhookAction.TrelloClient;
            string cardId = webhookAction.Data.Card.Id;
            //Check if same image id sticker already exist
            List<Sticker> stickers = await trelloClient.GetStickersOnCardAsync(cardId);
            Sticker existingSticker = stickers.FirstOrDefault(x => x.ImageId == StickerImageIdToRemove);
            if (existingSticker == null)
            {
                processingResult.AddToLog($"SKIPPED: Sticker '{StickerImageIdToRemove}' is not on card '{webhookAction.Data.Card.Name}'");
                processingResult.ActionsSkipped++;
            }
            else
            {
                await trelloClient.DeleteStickerAsync(cardId, existingSticker.Id);
                processingResult.AddToLog($"Removed Sticker '{StickerImageIdToRemove}' from card '{webhookAction.Data.Card.Name}'");
                processingResult.ActionsExecuted++;
            }
        }
    }
}
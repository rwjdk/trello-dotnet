using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Actions
{
    /// <summary>
    /// This Automation Action adds a specific Sticker to a card if it is not already present.
    /// </summary>
    /// <remarks>
    /// This is often used to warn about something irregular on a Card, for Example when it is moved to 'Done'
    /// </remarks>
    public class AddStickerToCardAction : IAutomationAction
    {
        /// <summary>
        /// The Sticker object to add (if a sticker with same id is not already present)
        /// </summary>
        public Sticker StickerToAdd { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stickerToAdd">The Sticker object to add (if a sticker with same id is not already present)</param>
        public AddStickerToCardAction(Sticker stickerToAdd)
        {
            StickerToAdd = stickerToAdd;
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
                throw new AutomationException("Could not perform AddStickerToCardAction as WebhookAction did not involve a Card");
            }

            TrelloClient trelloClient = webhookAction.TrelloClient;
            string cardId = webhookAction.Data.Card.Id;
            //Check if same image id sticker already exist
            List<Sticker> stickers = await trelloClient.GetStickersOnCardAsync(cardId);
            Sticker existingSticker = stickers.FirstOrDefault(x => x.ImageId == StickerToAdd.ImageId);
            if (existingSticker != null)
            {
                processingResult.AddToLog($"SKIPPED: Sticker '{StickerToAdd.ImageId}' is already on card '{webhookAction.Data.Card.Name}'");
                processingResult.ActionsSkipped++;
            }
            else
            {
                await trelloClient.AddStickerToCardAsync(cardId, StickerToAdd);
                processingResult.AddToLog($"Added Sticker '{StickerToAdd.ImageId}' to card '{webhookAction.Data.Card.Name}'");
                processingResult.ActionsExecuted++;
            }
        }
    }
}
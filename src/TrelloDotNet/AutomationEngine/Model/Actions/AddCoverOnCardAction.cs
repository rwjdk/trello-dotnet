using System.Collections.Generic;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Actions
{
    /// <summary>
    /// This Automation Action adds/updates a Cover on a card
    /// </summary>
    /// <remarks>
    /// This is often used to warn about something irregular on a Card, Example when it is moved to 'Done'
    /// </remarks>
    public class AddCoverOnCardAction : IAutomationAction
    {
        /// <summary>
        /// The Cover object to add/update
        /// </summary>
        public CardCover CardCoverToAdd { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cardCoverToAdd">The Cover object to add/update</param>
        public AddCoverOnCardAction(CardCover cardCoverToAdd)
        {
            CardCoverToAdd = cardCoverToAdd;
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
                throw new AutomationException("Could not perform AddCoverOnCardAction as WebhookAction did not involve a Card");
            }

            TrelloClient trelloClient = webhookAction.TrelloClient;
            await trelloClient.UpdateCardAsync(webhookAction.Data.Card.Id, new List<CardUpdate>
            {
                CardUpdate.Cover(CardCoverToAdd)
            });
            processingResult.AddToLog($"Updated Card '{webhookAction.Data.Card.Name}' with new Cover");
            processingResult.ActionsExecuted++;
        }
    }
}
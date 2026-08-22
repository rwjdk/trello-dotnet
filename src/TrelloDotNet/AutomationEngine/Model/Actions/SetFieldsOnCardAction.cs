using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Actions
{
    /// <summary>
    /// Action to set one or more fields on a card
    /// </summary>
    public class SetFieldsOnCardAction : IAutomationAction
    {
        /// <summary>
        /// List of field-values to set
        /// </summary>
        public ISetCardFieldValue[] FieldValues { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldValues">List of field-values to set</param>
        public SetFieldsOnCardAction(params ISetCardFieldValue[] fieldValues)
        {
            FieldValues = fieldValues;
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
                throw new AutomationException("Could not perform SetFieldsOnCardAction as WebhookAction did not involve a Card");
            }

            Card card = await webhookAction.Data.Card.GetAsync();
            List<CardUpdate> queryParametersToAdd = new List<CardUpdate>();
            foreach (ISetCardFieldValue fieldValue in FieldValues)
            {
                QueryParameter queryParameter = fieldValue.GetQueryParameter(card);
                if (queryParameter != null)
                {
                    queryParametersToAdd.Add(CardUpdate.AdditionalParameter(queryParameter));
                }
            }

            if (queryParametersToAdd.Any())
            {
                processingResult.ActionsExecuted++;
                await webhookAction.TrelloClient.UpdateCardAsync(card.Id, queryParametersToAdd);
            }
            else
            {
                processingResult.ActionsSkipped++;
            }
        }
    }
}
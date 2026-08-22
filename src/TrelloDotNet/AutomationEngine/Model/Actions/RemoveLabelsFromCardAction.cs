using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Actions
{
    /// <summary>
    /// Remove one or more Labels from a card
    /// </summary>
    public class RemoveLabelsFromCardAction : IAutomationAction
    {
        /// <summary>
        /// Label Id's to remove
        /// </summary>
        public string[] LabelsIds { get; }

        /// <summary>
        /// Set this to 'True' if you supplied the names of labels instead of the Ids. While this is more convenient, it will, in certain cases, be slightly slower and less resilient to the renaming of things. 
        /// </summary>
        public bool TreatLabelNameAsId { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="labelsIds">Label Id's to remove</param>
        public RemoveLabelsFromCardAction(params string[] labelsIds)
        {
            LabelsIds = labelsIds;
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
                throw new AutomationException("Could not perform RemoveLabelsFromCardAction as WebhookAction did not involve a Card");
            }

            string[] labelIdsToRemove = LabelsIds;

            if (TreatLabelNameAsId)
            {
                List<Label> allLabels = await webhookAction.TrelloClient.GetLabelsOfBoardAsync(webhookAction.Data.Board.Id);
                List<string> idsFromNames = new List<string>();
                foreach (string labelName in LabelsIds) //Remember; here the 'Id's' are actually Names so we need to find the id's needed for removal
                {
                    Label label = allLabels.FirstOrDefault(x => x.Name == labelName);
                    if (label != null)
                    {
                        idsFromNames.Add(label.Id);
                    }
                }

                labelIdsToRemove = idsFromNames.ToArray();
            }

            await webhookAction.TrelloClient.RemoveLabelsFromCardAsync(webhookAction.Data.Card.Id, labelIdsToRemove);
            processingResult.ActionsExecuted++;
        }
    }
}
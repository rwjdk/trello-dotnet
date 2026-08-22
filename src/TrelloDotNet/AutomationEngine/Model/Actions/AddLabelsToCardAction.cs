using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Actions
{
    /// <summary>
    /// This Automation Action adds a set of Labels to a card if they are not already present.
    /// </summary>
    public class AddLabelsToCardAction : IAutomationAction
    {
        /// <summary>
        /// The Label-ids to add
        /// </summary>
        public string[] LabelIds { get; }

        /// <summary>
        /// Set this to 'True' if you supplied the names of labels instead of the Ids. While this is more convenient, it will, in certain cases, be slightly slower and less resilient to the renaming of things. 
        /// </summary>
        public bool TreatLabelNameAsId { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="labelIds">The Label-ids to add</param>
        public AddLabelsToCardAction(params string[] labelIds)
        {
            LabelIds = labelIds;
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
                throw new AutomationException("Could not perform AddLabelsToCardAction as WebhookAction did not involve a Card");
            }

            TrelloClient trelloClient = webhookAction.TrelloClient;

            string[] labelIdsToRemove = LabelIds;

            if (TreatLabelNameAsId)
            {
                List<Label> allLabels = await webhookAction.TrelloClient.GetLabelsOfBoardAsync(webhookAction.Data.Board.Id);
                List<string> idsFromNames = new List<string>();
                foreach (string labelName in LabelIds) //Remember; here the 'Id's' are actually Names so we need to find the id's needed for removal
                {
                    Label label = allLabels.FirstOrDefault(x => x.Name == labelName);
                    if (label != null)
                    {
                        idsFromNames.Add(label.Id);
                    }
                }

                labelIdsToRemove = idsFromNames.ToArray();
            }

            Card card = await webhookAction.Data.Card.GetAsync();
            bool updateNeeded = false;
            foreach (string labelId in labelIdsToRemove)
            {
                if (card.LabelIds.Contains(labelId))
                {
                    continue;
                }

                card.LabelIds.Add(labelId);
                updateNeeded = true;
            }

            if (updateNeeded)
            {
                await trelloClient.UpdateCardAsync(card.Id, new List<CardUpdate>()
                {
                    CardUpdate.Labels(card.LabelIds)
                });
                processingResult.AddToLog($"Added labels '{string.Join(",", LabelIds)}' to card '{webhookAction.Data.Card.Name}'");
                processingResult.ActionsExecuted++;
            }
            else
            {
                processingResult.AddToLog($"SKIPPED: Adding labels as they are already on card '{webhookAction.Data.Card.Name}'");
                processingResult.ActionsSkipped++;
            }
        }
    }
}
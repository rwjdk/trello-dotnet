using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Actions
{
    /// <summary>
    /// This Automation Action adds Checklists to cards if it is not already present based on what Labels are present on the card.
    /// </summary>
    public class AddChecklistToCardIfLabelMatchAction : IAutomationAction
    {
        /// <summary>
        /// A set of labels and checklist Actions to apply if one or more of the labels are on the card
        /// </summary>
        public AddChecklistToCardIfLabelMatch[] AddChecklistActionsIfLabelsMatch { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addChecklistActionsIfLabelsMatch">A set of labels and checklist Actions to apply if one or more of the labels are on the card</param>
        public AddChecklistToCardIfLabelMatchAction(params AddChecklistToCardIfLabelMatch[] addChecklistActionsIfLabelsMatch)
        {
            AddChecklistActionsIfLabelsMatch = addChecklistActionsIfLabelsMatch;
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
                throw new AutomationException("Could not perform AddChecklistToCardIfLabelMatchAction as WebhookAction did not involve a Card");
            }

            Card card = await webhookAction.Data.Card.GetAsync(new GetCardOptions
            {
                CardFields = new CardFields("labels")
            });


            foreach (AddChecklistToCardIfLabelMatch checklistIfLabelMatch in AddChecklistActionsIfLabelsMatch)
            {
                foreach (string labelId in checklistIfLabelMatch.LabelIdsToMatch)
                {
                    if (checklistIfLabelMatch.TreatLabelNameAsId)
                    {
                        if (card.Labels.Any(x => x.Name == labelId))
                        {
                            bool proceed = true;
                            if (checklistIfLabelMatch.LabelIdsThatCantBePresent.Any())
                            {
                                proceed = !checklistIfLabelMatch.LabelIdsThatCantBePresent.Any(labelThatCantBePresent => card.Labels.Any(x => x.Name == labelThatCantBePresent));
                            }

                            if (proceed)
                            {
                                await PerformActions(checklistIfLabelMatch.AddChecklistToCardActions, webhookAction, processingResult);
                                break; //No more labels should be checked
                            }
                        }
                    }
                    else
                    {
                        if (card.Labels.Any(x => x.Id == labelId))
                        {
                            bool proceed = true;
                            if (checklistIfLabelMatch.LabelIdsThatCantBePresent.Any())
                            {
                                proceed = !checklistIfLabelMatch.LabelIdsThatCantBePresent.Any(labelThatCantBePresent => card.Labels.Any(x => x.Id == labelThatCantBePresent));
                            }

                            if (proceed)
                            {
                                await PerformActions(checklistIfLabelMatch.AddChecklistToCardActions, webhookAction, processingResult);
                                break; //No more labels should be checked
                            }
                        }
                    }
                }
            }
        }

        private async Task PerformActions(AddChecklistToCardAction[] addChecklistToCardActions, WebhookAction webhookAction, ProcessingResult processingResult)
        {
            foreach (AddChecklistToCardAction addChecklistToCardAction in addChecklistToCardActions)
            {
                await addChecklistToCardAction.PerformActionAsync(webhookAction, processingResult);
            }
        }
    }
}
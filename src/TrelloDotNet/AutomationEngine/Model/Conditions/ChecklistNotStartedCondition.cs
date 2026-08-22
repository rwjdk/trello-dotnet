using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Conditions
{
    /// <summary>
    /// Condition that check if a Card have a checklist with a certain name and that none of the items on that list is completed (aka the Checklist have not been started)
    /// </summary>
    public class ChecklistNotStartedCondition : IAutomationCondition
    {
        /// <summary>
        /// The name of the Checklist to check
        /// </summary>
        public string ChecklistNameToCheck { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="checklistNameToCheck">The name of the Checklist ot check</param>
        public ChecklistNotStartedCondition(string checklistNameToCheck)
        {
            ChecklistNameToCheck = checklistNameToCheck;
        }

        /// <summary>
        /// Method to check if the condition is met
        /// </summary>
        /// <remarks>
        /// If Checklist is not present or event is not a card-event this is considered as False
        /// </remarks>
        /// <param name="webhookAction">The Webhook Action that led to check of the condition. This object also have the TrelloClient and information about the event at your disposal</param>
        /// <returns>If condition is met or not</returns>
        public async Task<bool> IsConditionMetAsync(WebhookAction webhookAction)
        {
            if (webhookAction.Data?.Card == null)
            {
                return false;
            }

            List<Checklist> checklists = await webhookAction.TrelloClient.GetChecklistsOnCardAsync(webhookAction.Data.Card.Id);
            Checklist checklistToCheck = checklists.FirstOrDefault(x => x.Name == ChecklistNameToCheck);
            return checklistToCheck != null && checklistToCheck.Items.All(x => x.State != ChecklistItemState.Complete);
        }
    }
}
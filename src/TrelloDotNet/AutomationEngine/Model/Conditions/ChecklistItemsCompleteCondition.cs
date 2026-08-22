using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Conditions
{
    /// <summary>
    /// Condition that check if a Card have all its check-items complete
    /// </summary>
    public class ChecklistItemsCompleteCondition : IAutomationCondition
    {
        /// <summary>
        /// Method to check if the condition is met
        /// </summary>
        /// <remarks>
        /// If no checklists are present or event is not a card-event this is considered as False
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
            return checklists.All(checklist => checklist.Items.All(x => x.State == ChecklistItemState.Complete));
        }
    }
}
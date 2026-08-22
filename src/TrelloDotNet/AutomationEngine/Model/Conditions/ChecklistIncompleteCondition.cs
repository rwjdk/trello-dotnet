using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Conditions
{
    /// <summary>
    /// Condition that checks if a Card has a checklist with a specific name and that checklist has one or more incomplete items
    /// </summary>
    public class ChecklistIncompleteCondition : IAutomationCondition
    {
        /// <summary>
        /// The name of the Checklist to check
        /// </summary>
        public string ChecklistNameToCheck { get; }

        /// <summary>
        /// Defines the criteria on how to match the checklist name. Default is Equal Match
        /// </summary>
        public StringMatchCriteria ChecklistNameMatchCriteria { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="checklistNameToCheck">The name of the Checklist ot check</param>
        public ChecklistIncompleteCondition(string checklistNameToCheck)
        {
            ChecklistNameToCheck = checklistNameToCheck;
            ChecklistNameMatchCriteria = StringMatchCriteria.Equal;
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
            List<Checklist> checklistsToCheck;
            switch (ChecklistNameMatchCriteria)
            {
                case StringMatchCriteria.Equal:
                    checklistsToCheck = checklists.Where(x => x.Name == ChecklistNameToCheck).ToList();
                    break;
                case StringMatchCriteria.StartsWith:
                    checklistsToCheck = checklists.Where(x => x.Name.StartsWith(ChecklistNameToCheck)).ToList();
                    break;
                case StringMatchCriteria.EndsWith:
                    checklistsToCheck = checklists.Where(x => x.Name.EndsWith(ChecklistNameToCheck)).ToList();
                    break;
                case StringMatchCriteria.Contains:
                    checklistsToCheck = checklists.Where(x => x.Name.Contains(ChecklistNameToCheck)).ToList();
                    break;
                case StringMatchCriteria.RegEx:
                    checklistsToCheck = checklists.Where(x => Regex.IsMatch(x.Name, ChecklistNameToCheck)).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return checklistsToCheck.Count != 0 && checklistsToCheck.Any(checklist => checklist.Items.Any(x => x.State != ChecklistItemState.Complete));
        }
    }
}
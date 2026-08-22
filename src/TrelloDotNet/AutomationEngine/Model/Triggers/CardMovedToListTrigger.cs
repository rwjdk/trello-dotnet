using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Triggers
{
    /// <summary>
    /// Trigger of an event that is a Card is Moved to a List
    /// </summary>
    public class CardMovedToListTrigger : IAutomationTrigger
    {
        /// <summary>
        /// The constraints of the Trigger
        /// </summary>
        public CardMovedToListTriggerConstraint Constraint { get; }

        /// <summary>
        /// The Ids of the Lists the trigger should evaluate. Tip: These can be List-names instead of Ids if you set 'TreatListNameAsId' to True
        /// </summary>
        public string[] ListIds { get; }

        /// <summary>
        /// Set this to 'True' if you supplied names of Lists instead of the Ids. While this is more convenient, it will sometimes be slightly slower and less resilient to the renaming of things.
        /// </summary>
        public bool TreatListNameAsId { get; set; }

        /// <summary>
        /// Defines the criteria on how to match Names (only used if TreatListNameAsId = 'True'). Default is Equal Match
        /// </summary>
        public StringMatchCriteria ListNameMatchCriteria { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="constraint">The constraints of the Trigger</param>
        /// <param name="listIds"></param>
        public CardMovedToListTrigger(CardMovedToListTriggerConstraint constraint, params string[] listIds)
        {
            Constraint = constraint;
            ListIds = listIds;
            ListNameMatchCriteria = StringMatchCriteria.Equal;
        }

        /// <summary>
        /// If the Trigger is met
        /// </summary>
        /// <remarks>
        /// While this is built to support async execution, It is best practice to keep a Trigger as light as possible only checking values against the webhook Action and not make any API Calls. The reason for this is that Triggers are called quite often, so it is better to make simple triggers and supplement with Conditions
        /// </remarks>
        /// <param name="webhookAction">The Webhook Action that led to the check of the trigger. This object also have the TrelloClient and information about the event at your disposal</param>
        /// <returns>If trigger is met or not</returns>
        public async Task<bool> IsTriggerMetAsync(WebhookAction webhookAction)
        {
            await Task.CompletedTask;
            string partToCheck = TreatListNameAsId ? webhookAction.Data?.ListAfter?.Name : webhookAction.Data?.ListAfter?.Id;
            if (partToCheck == null)
            {
                return false;
            }

            bool correctType = webhookAction.Type == WebhookActionTypes.UpdateCard;

            if (!TreatListNameAsId)
            {
                ListNameMatchCriteria = StringMatchCriteria.Equal; //Force exact match no matter what user defined as it does not make sense to partly match auto-generated Ids
            }

            switch (Constraint)
            {
                case CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo:
                    switch (ListNameMatchCriteria)
                    {
                        case StringMatchCriteria.StartsWith:
                            return correctType && ListIds.Any(x => partToCheck.StartsWith(x));
                        case StringMatchCriteria.EndsWith:
                            return correctType && ListIds.Any(x => partToCheck.EndsWith(x));
                        case StringMatchCriteria.Contains:
                            return correctType && ListIds.Any(x => partToCheck.Contains(x));
                        case StringMatchCriteria.RegEx:
                            return correctType && ListIds.Any(x => Regex.IsMatch(partToCheck, x));
                        case StringMatchCriteria.Equal:
                        default:
                            return correctType && ListIds.Contains(partToCheck);
                    }

                case CardMovedToListTriggerConstraint.AnyButTheseListsAreMovedTo:
                    switch (ListNameMatchCriteria)
                    {
                        case StringMatchCriteria.StartsWith:
                            return correctType && ListIds.Any(x => !partToCheck.StartsWith(x));
                        case StringMatchCriteria.EndsWith:
                            return correctType && ListIds.Any(x => !partToCheck.EndsWith(x));
                        case StringMatchCriteria.Contains:
                            return correctType && ListIds.Any(x => !partToCheck.Contains(x));
                        case StringMatchCriteria.RegEx:
                            return correctType && ListIds.Any(x => !Regex.IsMatch(partToCheck, x));
                        case StringMatchCriteria.Equal:
                        default:
                            return correctType && !ListIds.Contains(partToCheck);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
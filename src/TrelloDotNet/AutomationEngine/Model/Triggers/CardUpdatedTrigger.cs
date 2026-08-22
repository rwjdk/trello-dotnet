using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Triggers
{
    /// <summary>
    /// Trigger when a card is Updated (This happens in various basic events so is kind of a catch-all Trigger to things happening on a card)
    /// </summary>
    public class CardUpdatedTrigger : IAutomationTrigger
    {
        private readonly CardUpdatedTriggerSubType? _subType;

        /// <summary>
        /// Constructor
        /// </summary>
        public CardUpdatedTrigger()
        {
            _subType = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subType">Optional SubType (Example: if it was the name that was updated, the due date, or something else) - If not specified this will react to any card update event</param>
        public CardUpdatedTrigger(CardUpdatedTriggerSubType subType)
        {
            _subType = subType;
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
            bool isUpdateCard = webhookAction.Type == WebhookActionTypes.UpdateCard;
            if (!isUpdateCard)
            {
                return false;
            }

            switch (_subType)
            {
                case CardUpdatedTriggerSubType.NameChanged:
                    return webhookAction.Display.TranslationKey == "action_renamed_card";
                case CardUpdatedTriggerSubType.DescriptionChanged:
                    return webhookAction.Display.TranslationKey == "action_changed_description_of_card";
                case CardUpdatedTriggerSubType.DueDateAdded:
                    return webhookAction.Display.TranslationKey == "action_added_a_due_date";
                case CardUpdatedTriggerSubType.DueDateChanged:
                    return webhookAction.Display.TranslationKey == "action_changed_a_due_date";
                case CardUpdatedTriggerSubType.DueDateMarkedAsComplete:
                    return webhookAction.Display.TranslationKey == "action_marked_the_due_date_complete";
                case CardUpdatedTriggerSubType.DueDateMarkedAsIncomplete:
                    return webhookAction.Display.TranslationKey == "action_marked_the_due_date_incomplete";
                case CardUpdatedTriggerSubType.DueDateRemoved:
                    return webhookAction.Display.TranslationKey == "action_removed_a_due_date";
                case CardUpdatedTriggerSubType.StartDateAdded:
                    return webhookAction.Display.TranslationKey == "action_added_a_start_date";
                case CardUpdatedTriggerSubType.StartDateChanged:
                    return webhookAction.Display.TranslationKey == "action_changed_a_start_date";
                case CardUpdatedTriggerSubType.StartDateRemoved:
                    return webhookAction.Display.TranslationKey == "action_removed_a_start_date";
                case CardUpdatedTriggerSubType.MovedToOtherList:
                    return webhookAction.Display.TranslationKey == "action_move_card_from_list_to_list";
                case CardUpdatedTriggerSubType.MovedHigherInList:
                    return webhookAction.Display.TranslationKey == "action_moved_card_higher";
                case CardUpdatedTriggerSubType.MovedLowerInList:
                    return webhookAction.Display.TranslationKey == "action_moved_card_lower";
                case CardUpdatedTriggerSubType.Archived:
                    return webhookAction.Display.TranslationKey == "action_archived_card";
                case CardUpdatedTriggerSubType.Unarchived:
                    return webhookAction.Display.TranslationKey == "action_sent_card_to_board";
            }

            return true;
        }
    }
}
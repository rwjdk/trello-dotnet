using System.Threading.Tasks;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Control.Webhook
{
    /// <summary>
    /// The various Smart Events that can be subscribed to
    /// </summary>
    public class WebhookDataReceiverSmartEvents
    {
        /// <summary>
        /// Occur when you move a card from one column to another
        /// </summary>
        public event WebhookEventHandler<WebhookSmartEventCardMovedToNewList> OnCardMovedToNewList;

        /// <summary>
        /// Occur when a card with a due date mark it as complete
        /// </summary>
        public event WebhookEventHandler<WebhookSmartEventDueMarkedAsComplete> OnDueCardIsMarkedAsComplete;

        /// <summary>
        /// Occur when a final check-item on a checklist is marked as complete and the whole Checklist is now complete
        /// </summary>
        public event WebhookEventHandler<WebhookSmartEventChecklistComplete> OnChecklistComplete;

        /// <summary>
        /// Occur when a member is added to a card
        /// </summary>
        public event WebhookEventHandler<WebhookSmartEventMemberAdded> OnMemberAddedToCard;

        /// <summary>
        /// Occur when a member is removed from a card
        /// </summary>
        public event WebhookEventHandler<WebhookSmartEventMemberRemoved> OnMemberRemovedFromCard;

        /// <summary>
        /// Occur when a label is added to a card
        /// </summary>
        public event WebhookEventHandler<WebhookSmartEventLabelAdded> OnLabelAddedToCard;

        /// <summary>
        /// Occur when a label is removed from a card
        /// </summary>
        public event WebhookEventHandler<WebhookSmartEventLabelRemoved> OnLabelRemovedFromCard;

        internal async Task FireEvent(WebhookAction action, TrelloClient trelloClient)
        {
            switch (action.Type)
            {
                case "updateCard":
                    if (action.Data != null)
                    {
                        //OnCardMovedToNewList
                        if (OnCardMovedToNewList != null && action.Data.ListBefore != null && action.Data.ListAfter != null)
                        {
                            OnCardMovedToNewList?.Invoke(new WebhookSmartEventCardMovedToNewList(action));
                        }

                        //OnDueCardIsMarkedAsComplete
                        if (OnDueCardIsMarkedAsComplete != null && action.Data.Card != null && action.Display != null && action.Data.Card.DueComplete.HasValue && action.Display.TranslationKey == "action_marked_the_due_date_complete")
                        {
                            OnDueCardIsMarkedAsComplete?.Invoke(new WebhookSmartEventDueMarkedAsComplete(action));
                        }
                    }

                    break;
                case "updateCheckItemStateOnCard":
                    //OnChecklistComplete
                    if (OnChecklistComplete != null && action.Data?.CheckItem != null && action.Data.CheckItem.State == ChecklistItemState.Complete)
                    {
                        //Check if all items of checklist is complete using the API
                        Checklist checklistAsync = await trelloClient.GetChecklistAsync(action.Data.Checklist.Id);
                        if (checklistAsync.Items.TrueForAll(x => x.State == ChecklistItemState.Complete))
                        {
                            OnChecklistComplete?.Invoke(new WebhookSmartEventChecklistComplete(action));
                        }
                    }

                    break;
                case "addMemberToCard":
                    if (OnMemberAddedToCard != null && action.Data.Member != null)
                    {
                        OnMemberAddedToCard?.Invoke(new WebhookSmartEventMemberAdded(action));
                    }

                    break;
                case "removeMemberFromCard":
                    if (OnMemberRemovedFromCard != null && action.Data.Member != null)
                    {
                        OnMemberRemovedFromCard?.Invoke(new WebhookSmartEventMemberRemoved(action));
                    }

                    break;
                case "addLabelToCard":
                    if (OnLabelAddedToCard != null && action.Data.Label != null)
                    {
                        OnLabelAddedToCard?.Invoke(new WebhookSmartEventLabelAdded(action));
                    }

                    break;
                case "removeLabelFromCard":
                    if (OnLabelRemovedFromCard != null && action.Data.Label != null)
                    {
                        OnLabelRemovedFromCard?.Invoke(new WebhookSmartEventLabelRemoved(action));
                    }

                    break;
            }
        }
    }
}
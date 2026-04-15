using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Control.Webhook
{
    /// <summary>
    /// The various Basic Events that can be subscribed to
    /// </summary>
    public class WebhookDataReceiverBasicEvents
    {
        /// <summary>
        /// OnUpdateCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateCard;

        /// <summary>
        /// OnAcceptEnterpriseJoinRequest
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAcceptEnterpriseJoinRequest;

        /// <summary>
        /// OnAddAttachmentToCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddAttachmentToCard;

        /// <summary>
        /// OnAddChecklistToCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddChecklistToCard;

        /// <summary>
        /// OnAddMemberToBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddMemberToBoard;

        /// <summary>
        /// OnAddMemberToCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddMemberToCard;

        /// <summary>
        /// OnAddMemberToOrganization
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddMemberToOrganization;

        /// <summary>
        /// OnAddOrganizationToEnterprise
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddOrganizationToEnterprise;

        /// <summary>
        /// OnAddToEnterprisePluginWhitelist
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddToEnterprisePluginWhitelist;

        /// <summary>
        /// OnAddToOrganizationBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddToOrganizationBoard;

        /// <summary>
        /// OnCommentCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCommentCard;

        /// <summary>
        /// OnConvertToCardFromCheckItem
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnConvertToCardFromCheckItem;

        /// <summary>
        /// OnCopyBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCopyBoard;

        /// <summary>
        /// OnCopyCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCopyCard;

        /// <summary>
        /// OnCopyCommentCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCopyCommentCard;

        /// <summary>
        /// OnCreateBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCreateBoard;

        /// <summary>
        /// OnCreateCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCreateCard;

        /// <summary>
        /// OnCreateList
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCreateList;

        /// <summary>
        /// OnCreateOrganization
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCreateOrganization;

        /// <summary>
        /// OnDeleteBoardInvitation
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDeleteBoardInvitation;

        /// <summary>
        /// OnDeleteCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDeleteCard;

        /// <summary>
        /// OnDeleteOrganizationInvitation
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDeleteOrganizationInvitation;

        /// <summary>
        /// OnDisableEnterprisePluginWhitelist
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDisableEnterprisePluginWhitelist;

        /// <summary>
        /// OnDisablePlugin
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDisablePlugin;

        /// <summary>
        /// OnDisablePowerUp
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDisablePowerUp;

        /// <summary>
        /// OnEmailCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnEmailCard;

        /// <summary>
        /// OnEnableEnterprisePluginWhitelist
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnEnableEnterprisePluginWhitelist;

        /// <summary>
        /// OnEnablePlugin
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnEnablePlugin;

        /// <summary>
        /// OnEnablePowerUp
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnEnablePowerUp;

        /// <summary>
        /// OnMakeAdminOfBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMakeAdminOfBoard;

        /// <summary>
        /// OnMakeNormalMemberOfBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMakeNormalMemberOfBoard;

        /// <summary>
        /// OnMakeNormalMemberOfOrganization
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMakeNormalMemberOfOrganization;

        /// <summary>
        /// OnMakeObserverOfBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMakeObserverOfBoard;

        /// <summary>
        /// OnMemberJoinedTrello
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMemberJoinedTrello;

        /// <summary>
        /// OnMoveCardFromBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMoveCardFromBoard;

        /// <summary>
        /// OnMoveCardToBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMoveCardToBoard;

        /// <summary>
        /// OnMoveListFromBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMoveListFromBoard;

        /// <summary>
        /// OnMoveListToBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMoveListToBoard;

        /// <summary>
        /// OnRemoveChecklistFromCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnRemoveChecklistFromCard;

        /// <summary>
        /// OnRemoveFromEnterprisePluginWhitelist
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnRemoveFromEnterprisePluginWhitelist;

        /// <summary>
        /// OnRemoveFromOrganizationBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnRemoveFromOrganizationBoard;

        /// <summary>
        /// OnRemoveMemberFromCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnRemoveMemberFromCard;

        /// <summary>
        /// OnRemoveOrganizationFromEnterprise
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnRemoveOrganizationFromEnterprise;

        /// <summary>
        /// OnUnconfirmedBoardInvitation
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUnconfirmedBoardInvitation;

        /// <summary>
        /// OnUnconfirmedOrganizationInvitation
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUnconfirmedOrganizationInvitation;

        /// <summary>
        /// OnUpdateBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateBoard;

        /// <summary>
        /// OnUpdateCheckItemStateOnCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateCheckItemStateOnCard;

        /// <summary>
        /// OnUpdateChecklist
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateChecklist;

        /// <summary>
        /// OnUpdateList
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateList;

        /// <summary>
        /// OnUpdateMember
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateMember;

        /// <summary>
        /// OnUpdateOrganization
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateOrganization;

        /// <summary>
        /// OnAddLabelToCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddLabelToCard;

        /// <summary>
        /// OnCopyChecklist
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCopyChecklist;

        /// <summary>
        /// OnCreateBoardInvitation
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCreateBoardInvitation;

        /// <summary>
        /// OnCreateBoardPreference
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCreateBoardPreference;

        /// <summary>
        /// OnCreateCheckItem
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCreateCheckItem;

        /// <summary>
        /// OnCreateLabel
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCreateLabel;

        /// <summary>
        /// OnCreateOrganizationInvitation
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnCreateOrganizationInvitation;

        /// <summary>
        /// OnDeleteAttachmentFromCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDeleteAttachmentFromCard;

        /// <summary>
        /// OnDeleteCheckItem
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDeleteCheckItem;

        /// <summary>
        /// OnDeleteComment
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDeleteComment;

        /// <summary>
        /// OnDeleteLabel
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDeleteLabel;

        /// <summary>
        /// OnMakeAdminOfOrganization
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMakeAdminOfOrganization;

        /// <summary>
        /// OnRemoveLabelFromCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnRemoveLabelFromCard;

        /// <summary>
        /// OnRemoveMemberFromBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnRemoveMemberFromBoard;

        /// <summary>
        /// OnRemoveMemberFromOrganization
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnRemoveMemberFromOrganization;

        /// <summary>
        /// OnUpdateCheckItem
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateCheckItem;

        /// <summary>
        /// OnUpdateComment
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateComment;

        /// <summary>
        /// OnUpdateLabel
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateLabel;

        /// <summary>
        /// OnVoteOnCard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnVoteOnCard;

        /// <summary>
        /// OnUnknownActionType
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUnknownActionType;

        /// <summary>
        /// OnAddCustomField
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnAddCustomField;

        /// <summary>
        /// OnDeleteCustomField
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnDeleteCustomField;

        /// <summary>
        /// OnUpdateCustomFieldItem (update on card)
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateCustomFieldItem;

        /// <summary>
        /// OnUpdateCustomField (update definition)
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnUpdateCustomField;

        /// <summary>
        /// OnMoveInboxCardToBoard
        /// </summary>
        public event WebhookEventHandler<WebhookAction> OnMoveInboxCardToBoard;

        internal void FireEvent(WebhookAction action)
        {
            switch (action.Type)
            {
                case WebhookActionTypes.AcceptEnterpriseJoinRequest:
                    OnAcceptEnterpriseJoinRequest?.Invoke(action);
                    break;
                case WebhookActionTypes.AddAttachmentToCard:
                    OnAddAttachmentToCard?.Invoke(action);
                    break;
                case WebhookActionTypes.AddChecklistToCard:
                    OnAddChecklistToCard?.Invoke(action);
                    break;
                case WebhookActionTypes.AddMemberToBoard:
                    OnAddMemberToBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.AddMemberToCard:
                    OnAddMemberToCard?.Invoke(action);
                    break;
                case WebhookActionTypes.AddMemberToOrganization:
                    OnAddMemberToOrganization?.Invoke(action);
                    break;
                case WebhookActionTypes.AddOrganizationToEnterprise:
                    OnAddOrganizationToEnterprise?.Invoke(action);
                    break;
                case WebhookActionTypes.AddToEnterprisePluginWhitelist:
                    OnAddToEnterprisePluginWhitelist?.Invoke(action);
                    break;
                case WebhookActionTypes.AddToOrganizationBoard:
                    OnAddToOrganizationBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.CommentCard:
                    OnCommentCard?.Invoke(action);
                    break;
                case WebhookActionTypes.ConvertToCardFromCheckItem:
                    OnConvertToCardFromCheckItem?.Invoke(action);
                    break;
                case WebhookActionTypes.CopyBoard:
                    OnCopyBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.CopyCard:
                    OnCopyCard?.Invoke(action);
                    break;
                case WebhookActionTypes.CopyCommentCard:
                    OnCopyCommentCard?.Invoke(action);
                    break;
                case WebhookActionTypes.CreateBoard:
                    OnCreateBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.CreateCard:
                    OnCreateCard?.Invoke(action);
                    break;
                case WebhookActionTypes.CreateList:
                    OnCreateList?.Invoke(action);
                    break;
                case WebhookActionTypes.CreateOrganization:
                    OnCreateOrganization?.Invoke(action);
                    break;
                case WebhookActionTypes.DeleteBoardInvitation:
                    OnDeleteBoardInvitation?.Invoke(action);
                    break;
                case WebhookActionTypes.DeleteCard:
                    OnDeleteCard?.Invoke(action);
                    break;
                case WebhookActionTypes.DeleteOrganizationInvitation:
                    OnDeleteOrganizationInvitation?.Invoke(action);
                    break;
                case WebhookActionTypes.DisableEnterprisePluginWhitelist:
                    OnDisableEnterprisePluginWhitelist?.Invoke(action);
                    break;
                case WebhookActionTypes.DisablePlugin:
                    OnDisablePlugin?.Invoke(action);
                    break;
                case WebhookActionTypes.DisablePowerUp:
                    OnDisablePowerUp?.Invoke(action);
                    break;
                case WebhookActionTypes.EmailCard:
                    OnEmailCard?.Invoke(action);
                    break;
                case WebhookActionTypes.EnableEnterprisePluginWhitelist:
                    OnEnableEnterprisePluginWhitelist?.Invoke(action);
                    break;
                case WebhookActionTypes.EnablePlugin:
                    OnEnablePlugin?.Invoke(action);
                    break;
                case WebhookActionTypes.EnablePowerUp:
                    OnEnablePowerUp?.Invoke(action);
                    break;
                case WebhookActionTypes.MakeAdminOfBoard:
                    OnMakeAdminOfBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.MakeNormalMemberOfBoard:
                    OnMakeNormalMemberOfBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.MakeNormalMemberOfOrganization:
                    OnMakeNormalMemberOfOrganization?.Invoke(action);
                    break;
                case WebhookActionTypes.MakeObserverOfBoard:
                    OnMakeObserverOfBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.MemberJoinedTrello:
                    OnMemberJoinedTrello?.Invoke(action);
                    break;
                case WebhookActionTypes.MoveCardFromBoard:
                    OnMoveCardFromBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.MoveCardToBoard:
                    OnMoveCardToBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.MoveListFromBoard:
                    OnMoveListFromBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.MoveListToBoard:
                    OnMoveListToBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.RemoveChecklistFromCard:
                    OnRemoveChecklistFromCard?.Invoke(action);
                    break;
                case WebhookActionTypes.RemoveFromEnterprisePluginWhitelist:
                    OnRemoveFromEnterprisePluginWhitelist?.Invoke(action);
                    break;
                case WebhookActionTypes.RemoveFromOrganizationBoard:
                    OnRemoveFromOrganizationBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.RemoveMemberFromCard:
                    OnRemoveMemberFromCard?.Invoke(action);
                    break;
                case WebhookActionTypes.RemoveOrganizationFromEnterprise:
                    OnRemoveOrganizationFromEnterprise?.Invoke(action);
                    break;
                case WebhookActionTypes.UnconfirmedBoardInvitation:
                    OnUnconfirmedBoardInvitation?.Invoke(action);
                    break;
                case WebhookActionTypes.UnconfirmedOrganizationInvitation:
                    OnUnconfirmedOrganizationInvitation?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateBoard:
                    OnUpdateBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateCard:
                    OnUpdateCard?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateCheckItemStateOnCard:
                    OnUpdateCheckItemStateOnCard?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateChecklist:
                    OnUpdateChecklist?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateList:
                    OnUpdateList?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateMember:
                    OnUpdateMember?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateOrganization:
                    OnUpdateOrganization?.Invoke(action);
                    break;
                case WebhookActionTypes.AddLabelToCard:
                    OnAddLabelToCard?.Invoke(action);
                    break;
                case WebhookActionTypes.CopyChecklist:
                    OnCopyChecklist?.Invoke(action);
                    break;
                case WebhookActionTypes.CreateBoardInvitation:
                    OnCreateBoardInvitation?.Invoke(action);
                    break;
                case WebhookActionTypes.CreateBoardPreference:
                    OnCreateBoardPreference?.Invoke(action);
                    break;
                case WebhookActionTypes.CreateCheckItem:
                    OnCreateCheckItem?.Invoke(action);
                    break;
                case WebhookActionTypes.CreateLabel:
                    OnCreateLabel?.Invoke(action);
                    break;
                case WebhookActionTypes.CreateOrganizationInvitation:
                    OnCreateOrganizationInvitation?.Invoke(action);
                    break;
                case WebhookActionTypes.DeleteAttachmentFromCard:
                    OnDeleteAttachmentFromCard?.Invoke(action);
                    break;
                case WebhookActionTypes.DeleteCheckItem:
                    OnDeleteCheckItem?.Invoke(action);
                    break;
                case WebhookActionTypes.DeleteComment:
                    OnDeleteComment?.Invoke(action);
                    break;
                case WebhookActionTypes.DeleteLabel:
                    OnDeleteLabel?.Invoke(action);
                    break;
                case WebhookActionTypes.MakeAdminOfOrganization:
                    OnMakeAdminOfOrganization?.Invoke(action);
                    break;
                case WebhookActionTypes.RemoveLabelFromCard:
                    OnRemoveLabelFromCard?.Invoke(action);
                    break;
                case WebhookActionTypes.RemoveMemberFromBoard:
                    OnRemoveMemberFromBoard?.Invoke(action);
                    break;
                case WebhookActionTypes.RemoveMemberFromOrganization:
                    OnRemoveMemberFromOrganization?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateCheckItem:
                    OnUpdateCheckItem?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateComment:
                    OnUpdateComment?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateLabel:
                    OnUpdateLabel?.Invoke(action);
                    break;
                case WebhookActionTypes.VoteOnCard:
                    OnVoteOnCard?.Invoke(action);
                    break;
                case WebhookActionTypes.DeleteCustomField:
                    OnDeleteCustomField?.Invoke(action);
                    break;
                case WebhookActionTypes.AddCustomField:
                    OnAddCustomField?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateCustomFieldItem:
                    OnUpdateCustomFieldItem?.Invoke(action);
                    break;
                case WebhookActionTypes.UpdateCustomField:
                    OnUpdateCustomField?.Invoke(action);
                    break;
                case WebhookActionTypes.MoveInboxCardToBoard:
                    OnMoveInboxCardToBoard?.Invoke(action);
                    break;
                default:
                    OnUnknownActionType?.Invoke(action);
                    break;
            }
        }
    }
}
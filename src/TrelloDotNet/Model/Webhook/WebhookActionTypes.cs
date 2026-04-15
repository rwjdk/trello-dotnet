// ReSharper disable UnusedMember.Global

namespace TrelloDotNet.Model.Webhook
{
    /// <summary>
    /// The various WebhookAction.Types possible
    /// </summary>
    public struct WebhookActionTypes
    {
        /// <summary>
        /// updateCard
        /// </summary>
        public const string UpdateCard = "updateCard";

        /// <summary>
        /// acceptEnterpriseJoinRequest
        /// </summary>
        public const string AcceptEnterpriseJoinRequest = "acceptEnterpriseJoinRequest";

        /// <summary>
        /// addAttachmentToCard
        /// </summary>
        public const string AddAttachmentToCard = "addAttachmentToCard";

        /// <summary>
        /// addChecklistToCard
        /// </summary>
        public const string AddChecklistToCard = "addChecklistToCard";

        /// <summary>
        /// addMemberToBoard
        /// </summary>
        public const string AddMemberToBoard = "addMemberToBoard";

        /// <summary>
        /// addMemberToCard
        /// </summary>
        public const string AddMemberToCard = "addMemberToCard";

        /// <summary>
        /// addMemberToOrganization
        /// </summary>
        public const string AddMemberToOrganization = "addMemberToOrganization";

        /// <summary>
        /// addOrganizationToEnterprise
        /// </summary>
        public const string AddOrganizationToEnterprise = "addOrganizationToEnterprise";

        /// <summary>
        /// addToEnterprisePluginWhitelist
        /// </summary>
        public const string AddToEnterprisePluginWhitelist = "addToEnterprisePluginWhitelist";

        /// <summary>
        /// addToOrganizationBoard
        /// </summary>
        public const string AddToOrganizationBoard = "addToOrganizationBoard";

        /// <summary>
        /// commentCard
        /// </summary>
        public const string CommentCard = "commentCard";

        /// <summary>
        /// convertToCardFromCheckItem
        /// </summary>
        public const string ConvertToCardFromCheckItem = "convertToCardFromCheckItem";

        /// <summary>
        /// copyBoard
        /// </summary>
        public const string CopyBoard = "copyBoard";

        /// <summary>
        /// copyCard
        /// </summary>
        public const string CopyCard = "copyCard";

        /// <summary>
        /// copyCommentCard
        /// </summary>
        public const string CopyCommentCard = "copyCommentCard";

        /// <summary>
        /// createBoard
        /// </summary>
        public const string CreateBoard = "createBoard";

        /// <summary>
        /// createCard
        /// </summary>
        public const string CreateCard = "createCard";

        /// <summary>
        /// createList
        /// </summary>
        public const string CreateList = "createList";

        /// <summary>
        /// createOrganization
        /// </summary>
        public const string CreateOrganization = "createOrganization";

        /// <summary>
        /// deleteBoardInvitation
        /// </summary>
        public const string DeleteBoardInvitation = "deleteBoardInvitation";

        /// <summary>
        /// deleteCard
        /// </summary>
        public const string DeleteCard = "deleteCard";

        /// <summary>
        /// deleteOrganizationInvitation
        /// </summary>
        public const string DeleteOrganizationInvitation = "deleteOrganizationInvitation";

        /// <summary>
        /// disableEnterprisePluginWhitelist
        /// </summary>
        public const string DisableEnterprisePluginWhitelist = "disableEnterprisePluginWhitelist";

        /// <summary>
        /// disablePlugin
        /// </summary>
        public const string DisablePlugin = "disablePlugin";

        /// <summary>
        /// disablePowerUp
        /// </summary>
        public const string DisablePowerUp = "disablePowerUp";

        /// <summary>
        /// emailCard
        /// </summary>
        public const string EmailCard = "emailCard";

        /// <summary>
        /// enableEnterprisePluginWhitelist
        /// </summary>
        public const string EnableEnterprisePluginWhitelist = "enableEnterprisePluginWhitelist";

        /// <summary>
        /// enablePlugin
        /// </summary>
        public const string EnablePlugin = "enablePlugin";

        /// <summary>
        /// enablePowerUp
        /// </summary>
        public const string EnablePowerUp = "enablePowerUp";

        /// <summary>
        /// makeAdminOfBoard
        /// </summary>
        public const string MakeAdminOfBoard = "makeAdminOfBoard";

        /// <summary>
        /// makeNormalMemberOfBoard
        /// </summary>
        public const string MakeNormalMemberOfBoard = "makeNormalMemberOfBoard";

        /// <summary>
        /// makeNormalMemberOfOrganization
        /// </summary>
        public const string MakeNormalMemberOfOrganization = "makeNormalMemberOfOrganization";

        /// <summary>
        /// makeObserverOfBoard
        /// </summary>
        public const string MakeObserverOfBoard = "makeObserverOfBoard";

        /// <summary>
        /// memberJoinedTrello
        /// </summary>
        public const string MemberJoinedTrello = "memberJoinedTrello";

        /// <summary>
        /// moveCardFromBoard
        /// </summary>
        public const string MoveCardFromBoard = "moveCardFromBoard";

        /// <summary>
        /// moveCardToBoard
        /// </summary>
        public const string MoveCardToBoard = "moveCardToBoard";

        /// <summary>
        /// moveListFromBoard
        /// </summary>
        public const string MoveListFromBoard = "moveListFromBoard";

        /// <summary>
        /// moveListToBoard
        /// </summary>
        public const string MoveListToBoard = "moveListToBoard";

        /// <summary>
        /// removeChecklistFromCard
        /// </summary>
        public const string RemoveChecklistFromCard = "removeChecklistFromCard";

        /// <summary>
        /// removeFromEnterprisePluginWhitelist
        /// </summary>
        public const string RemoveFromEnterprisePluginWhitelist = "removeFromEnterprisePluginWhitelist";

        /// <summary>
        /// removeFromOrganizationBoard
        /// </summary>
        public const string RemoveFromOrganizationBoard = "removeFromOrganizationBoard";

        /// <summary>
        /// removeMemberFromCard
        /// </summary>
        public const string RemoveMemberFromCard = "removeMemberFromCard";

        /// <summary>
        /// removeOrganizationFromEnterprise
        /// </summary>
        public const string RemoveOrganizationFromEnterprise = "removeOrganizationFromEnterprise";

        /// <summary>
        /// unconfirmedBoardInvitation
        /// </summary>
        public const string UnconfirmedBoardInvitation = "unconfirmedBoardInvitation";

        /// <summary>
        /// unconfirmedOrganizationInvitation
        /// </summary>
        public const string UnconfirmedOrganizationInvitation = "unconfirmedOrganizationInvitation";

        /// <summary>
        /// updateBoard
        /// </summary>
        public const string UpdateBoard = "updateBoard";

        /// <summary>
        /// updateCheckItemStateOnCard
        /// </summary>
        public const string UpdateCheckItemStateOnCard = "updateCheckItemStateOnCard";

        /// <summary>
        /// updateChecklist
        /// </summary>
        public const string UpdateChecklist = "updateChecklist";

        /// <summary>
        /// updateList
        /// </summary>
        public const string UpdateList = "updateList";

        /// <summary>
        /// updateMember
        /// </summary>
        public const string UpdateMember = "updateMember";

        /// <summary>
        /// updateOrganization
        /// </summary>
        public const string UpdateOrganization = "updateOrganization";

        /// <summary>
        /// addLabelToCard
        /// </summary>
        public const string AddLabelToCard = "addLabelToCard";

        /// <summary>
        /// copyChecklist
        /// </summary>
        public const string CopyChecklist = "copyChecklist";

        /// <summary>
        /// createBoardInvitation
        /// </summary>
        public const string CreateBoardInvitation = "createBoardInvitation";

        /// <summary>
        /// createBoardPreference
        /// </summary>
        public const string CreateBoardPreference = "createBoardPreference";

        /// <summary>
        /// createCheckItem
        /// </summary>
        public const string CreateCheckItem = "createCheckItem";

        /// <summary>
        /// createLabel
        /// </summary>
        public const string CreateLabel = "createLabel";

        /// <summary>
        /// createOrganizationInvitation
        /// </summary>
        public const string CreateOrganizationInvitation = "createOrganizationInvitation";

        /// <summary>
        /// deleteAttachmentFromCard
        /// </summary>
        public const string DeleteAttachmentFromCard = "deleteAttachmentFromCard";

        /// <summary>
        /// deleteCheckItem
        /// </summary>
        public const string DeleteCheckItem = "deleteCheckItem";

        /// <summary>
        /// deleteComment
        /// </summary>
        public const string DeleteComment = "deleteComment";

        /// <summary>
        /// deleteLabel
        /// </summary>
        public const string DeleteLabel = "deleteLabel";

        /// <summary>
        /// makeAdminOfOrganization
        /// </summary>
        public const string MakeAdminOfOrganization = "makeAdminOfOrganization";

        /// <summary>
        /// removeLabelFromCard
        /// </summary>
        public const string RemoveLabelFromCard = "removeLabelFromCard";

        /// <summary>
        /// removeMemberFromBoard
        /// </summary>
        public const string RemoveMemberFromBoard = "removeMemberFromBoard";

        /// <summary>
        /// removeMemberFromOrganization
        /// </summary>
        public const string RemoveMemberFromOrganization = "removeMemberFromOrganization";

        /// <summary>
        /// updateCheckItem
        /// </summary>
        public const string UpdateCheckItem = "updateCheckItem";

        /// <summary>
        /// updateComment
        /// </summary>
        public const string UpdateComment = "updateComment";

        /// <summary>
        /// updateLabel
        /// </summary>
        public const string UpdateLabel = "updateLabel";

        /// <summary>
        /// voteOnCard
        /// </summary>
        public const string VoteOnCard = "voteOnCard";

        /// <summary>
        /// addCustomField
        /// </summary>
        public const string AddCustomField = "addCustomField";

        /// <summary>
        /// deleteCustomField
        /// </summary>
        public const string DeleteCustomField = "deleteCustomField";

        /// <summary>
        /// updateCustomField
        /// </summary>
        public const string UpdateCustomField = "updateCustomField";

        /// <summary>
        /// updateCustomFieldItem
        /// </summary>
        public const string UpdateCustomFieldItem = "updateCustomFieldItem";

        /// <summary>
        /// updateCard:idList (This is sort of a special "event" that indicate a card was moved to a list despite the official type is "updateCard"). Use in GetAction... Methods
        /// </summary>
        public const string MoveCardToList = "updateCard:idList";

        /// <summary>
        /// moveInboxCardToBoard
        /// </summary>
        public const string MoveInboxCardToBoard = "moveInboxCardToBoard";
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.GetMemberOptions;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Retrieves all members (users) of a specific board.
        /// </summary>
        /// <param name="boardId">ID of the board </param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of members on the board</returns>
        public async Task<List<Member>> GetMembersOfBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfBoard(boardId), cancellationToken);
        }

        /// <summary>
        /// Retrieves all members (users) of a specific board, with additional options for member data selection.
        /// </summary>
        /// <param name="boardId">ID of the board</param>
        /// <param name="options">Options for what data to include on the member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of members on the board</returns>
        public async Task<List<Member>> GetMembersOfBoardAsync(string boardId, GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                return await GetMembersOfBoardAsync(boardId, cancellationToken);
            }

            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfBoard(boardId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Retrieves all members (users) who voted on a specific card.
        /// </summary>
        /// <param name="cardId">ID of the card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of members who voted on the card</returns>
        public async Task<List<Member>> GetMembersWhoVotedOnCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersWhoVotedOnOfCard(cardId), cancellationToken);
        }

        /// <summary>
        /// Retrieves all members (users) who voted on a specific card, with additional options for member data selection.
        /// </summary>
        /// <param name="cardId">ID of the card</param>
        /// <param name="options">Options for what data to include on the member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of members who voted on the card</returns>
        public async Task<List<Member>> GetMembersWhoVotedOnCardAsync(string cardId, GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                return await GetMembersWhoVotedOnCardAsync(cardId, cancellationToken);
            }

            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersWhoVotedOnOfCard(cardId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Retrieves all members (users) assigned to a specific card.
        /// </summary>
        /// <param name="cardId">ID of the card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of members assigned to the card</returns>
        public async Task<List<Member>> GetMembersOfCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfCard(cardId), cancellationToken);
        }

        /// <summary>
        /// Retrieves all members (users) assigned to a specific card, with additional options for member data selection.
        /// </summary>
        /// <param name="cardId">ID of the card</param>
        /// <param name="options">Options for what data to include on the member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of members assigned to the card</returns>
        public async Task<List<Member>> GetMembersOfCardAsync(string cardId, GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                return await GetMembersOfCardAsync(cardId, cancellationToken);
            }

            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfCard(cardId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Retrieves a member by their ID.
        /// </summary>
        /// <param name="memberId">ID of the member to retrieve</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The requested member</returns>
        public async Task<Member> GetMemberAsync(string memberId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Member>(GetUrlBuilder.GetMember(memberId), cancellationToken);
        }

        /// <summary>
        /// Adds one or more members to a card by their IDs.
        /// </summary>
        /// <param name="cardId">ID of the card to add members to</param>
        /// <param name="memberIdsToAdd">One or more IDs of members to add</param>
        /// <returns>The updated card with the added members</returns>
        public async Task<Card> AddMembersToCardAsync(string cardId, params string[] memberIdsToAdd)
        {
            return await AddMembersToCardAsync(cardId, CancellationToken.None, memberIdsToAdd);
        }

        /// <summary>
        /// Adds one or more members to a card by their IDs.
        /// </summary>
        /// <param name="cardId">ID of the card to add members to</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <param name="memberIdsToAdd">One or more IDs of members to add</param>
        /// <returns>The updated card with the added members</returns>
        public async Task<Card> AddMembersToCardAsync(string cardId, CancellationToken cancellationToken = default, params string[] memberIdsToAdd)
        {
            Card card = await GetCardAsync(cardId, cancellationToken);
            List<string> missing = memberIdsToAdd.Where(x => !card.MemberIds.Contains(x)).ToList();

            if (missing.Count == 0)
            {
                return card; //Everyone already There
            }

            //Need update
            card.MemberIds.AddRange(missing);
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Members(card.MemberIds.Distinct().ToList())
            }, cancellationToken);
        }

        /// <summary>
        /// Removes one or more members from a card by their IDs.
        /// </summary>
        /// <param name="cardId">ID of the card to remove members from</param>
        /// <param name="memberIdsToRemove">One or more IDs of members to remove</param>
        /// <returns>The updated card with the members removed</returns>
        public async Task<Card> RemoveMembersFromCardAsync(string cardId, params string[] memberIdsToRemove)
        {
            return await RemoveMembersFromCardAsync(cardId, CancellationToken.None, memberIdsToRemove);
        }

        /// <summary>
        /// Removes one or more members from a card by their IDs.
        /// </summary>
        /// <param name="cardId">ID of the card to remove members from</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <param name="memberIdsToRemove">One or more IDs of members to remove</param>
        /// <returns>The updated card with the members removed</returns>
        public async Task<Card> RemoveMembersFromCardAsync(string cardId, CancellationToken cancellationToken = default, params string[] memberIdsToRemove)
        {
            Card card = await GetCardAsync(cardId, cancellationToken);
            List<string> toRemove = memberIdsToRemove.Where(x => card.MemberIds.Contains(x)).ToList();
            if (toRemove.Count == 0)
            {
                return card; //Everyone not there
            }

            //Need update
            card.MemberIds = card.MemberIds.Except(toRemove).ToList();
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Members(card.MemberIds.Distinct().ToList())
            }, cancellationToken);
        }

        /// <summary>
        /// Removes all members from a card, leaving it without any assigned members.
        /// </summary>
        /// <param name="cardId">ID of the card to remove all members from</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated card with all members removed</returns>
        public async Task<Card> RemoveAllMembersFromCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Members(new List<string>())
            }, cancellationToken);
        }

        /// <summary>
        /// Adds a member to a board, granting them access with the specified membership type.
        /// </summary>
        /// <param name="boardId">ID of the board to grant access to</param>
        /// <param name="memberId">ID of the member to grant access</param>
        /// <param name="membershipType">The type of access to grant (admin, normal, observer, etc.)</param>
        /// <param name="allowBillableGuest">Optional parameter to allow organization admins to add multi-board guests to a board</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task AddMemberToBoardAsync(string boardId, string memberId, MembershipType membershipType, bool allowBillableGuest = false, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Put($"{UrlPaths.Boards}/{boardId}/{UrlPaths.Members}/{memberId}", cancellationToken, 0,
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Type, membershipType.GetJsonPropertyName()),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.AllowBillableGuest, allowBillableGuest));
        }

        /// <summary>
        /// Invites a member to a board via email, granting them access with the specified membership type.
        /// </summary>
        /// <param name="boardId">ID of the board to invite the member to</param>
        /// <param name="email">Email address to send the invitation to</param>
        /// <param name="membershipType">The type of access to grant (normal or observer)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task InviteMemberToBoardViaEmailAsync(string boardId, string email, MembershipType membershipType, CancellationToken cancellationToken = default)
        {
            if (membershipType == MembershipType.Admin)
            {
                throw new TrelloApiException($"It is not possible in the API to invite a member as 'Admin'. Instead invite as Normal and once they have accepted and email is confirmed, use '{nameof(UpdateMembershipTypeOfMemberOnBoardAsync)}' to promote them to Admin");
            }

            await _apiRequestController.Put($"{UrlPaths.Boards}/{boardId}/{UrlPaths.Members}", cancellationToken, 0,
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Type, membershipType.GetJsonPropertyName()),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Email, email));
        }

        /// <summary>
        /// Removes a member from a board, revoking their access.
        /// </summary>
        /// <param name="boardId">ID of the board to remove the member from</param>
        /// <param name="memberId">ID of the member to remove</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task RemoveMemberFromBoardAsync(string boardId, string memberId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Boards}/{boardId}/{UrlPaths.Members}/{memberId}", cancellationToken, 0);
        }

        /// <summary>
        /// Retrieves information about the member that owns the token used by this TrelloClient.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The member that owns the token</returns>
        public async Task<Member> GetTokenMemberAsync(CancellationToken cancellationToken = default)
        {
            if (_apiRequestController.UsesOAuth2)
            {
                return await _apiRequestController.Get<Member>($"{UrlPaths.Members}/me", cancellationToken);
            }

            return await _apiRequestController.Get<Member>(GetUrlBuilder.GetTokenMember(_apiRequestController.Token), cancellationToken);
        }

        /// <summary>
        /// Retrieves information about the member that owns the token used by this TrelloClient, with additional options for member data selection.
        /// </summary>
        /// <param name="options">Options for what data to include on the member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The member that owns the token</returns>
        public async Task<Member> GetTokenMemberAsync(GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                return await GetTokenMemberAsync(cancellationToken);
            }

            string suffix = _apiRequestController.UsesOAuth2
                ? $"{UrlPaths.Members}/me"
                : GetUrlBuilder.GetTokenMember(_apiRequestController.Token);
            return await _apiRequestController.Get<Member>(suffix, cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Retrieves all members (users) of a specific organization (workspace).
        /// </summary>
        /// <param name="organizationId">ID of the organization (workspace)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of members in the organization</returns>
        public async Task<List<Member>> GetMembersOfOrganizationAsync(string organizationId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfOrganization(organizationId), cancellationToken);
        }

        /// <summary>
        /// Retrieves all members (users) of a specific organization (workspace), with additional options for member data selection.
        /// </summary>
        /// <param name="organizationId">ID of the organization (workspace)</param>
        /// <param name="options">Options for what data to include on the member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of members in the organization</returns>
        public async Task<List<Member>> GetMembersOfOrganizationAsync(string organizationId, GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                return await GetMembersOfOrganizationAsync(organizationId, cancellationToken);
            }

            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfOrganization(organizationId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Adds a member vote to a card.
        /// </summary>
        /// <param name="cardId">ID of the card to add the vote to</param>
        /// <param name="memberId">ID of the member casting the vote</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task AddVoteToCardAsync(string cardId, string memberId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Post($"{UrlPaths.Cards}/{cardId}/membersVoted", cancellationToken, 0, new QueryParameter(Constants.TrelloIds.QueryParameterNames.Value, memberId));
        }

        /// <summary>
        /// Removes a member vote from a card.
        /// </summary>
        /// <param name="cardId">ID of the card to remove the vote from</param>
        /// <param name="memberId">ID of the member whose vote should be removed</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task RemoveVoteFromCardAsync(string cardId, string memberId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Cards}/{cardId}/membersVoted/{memberId}", cancellationToken, 0);
        }
    }
}




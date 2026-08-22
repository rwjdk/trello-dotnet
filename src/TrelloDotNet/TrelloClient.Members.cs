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
        /// Get the Members (users) of a board
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Members</returns>
        public async Task<List<Member>> GetMembersOfBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfBoard(boardId), cancellationToken);
        }

        /// <summary>
        /// Get the Members (users) of a board
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="options">Option for what data to include on the Member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Members</returns>
        public async Task<List<Member>> GetMembersOfBoardAsync(string boardId, GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfBoard(boardId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Get the Members (users) who voted on a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Members who voted</returns>
        public async Task<List<Member>> GetMembersWhoVotedOnCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersWhoVotedOnOfCard(cardId), cancellationToken);
        }

        /// <summary>
        /// Get the Members (users) who voted on a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="options">Option for what data to include on the Member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Members who voted</returns>
        public async Task<List<Member>> GetMembersWhoVotedOnCardAsync(string cardId, GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersWhoVotedOnOfCard(cardId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Get the Members (users) of a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Members</returns>
        public async Task<List<Member>> GetMembersOfCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfCard(cardId), cancellationToken);
        }

        /// <summary>
        /// Get the Members (users) of a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="options">Option for what data to include on the Member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Members</returns>
        public async Task<List<Member>> GetMembersOfCardAsync(string cardId, GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfCard(cardId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Get a Member with a specific Id
        /// </summary>
        /// <param name="memberId">Id of the Member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Member</returns>
        public async Task<Member> GetMemberAsync(string memberId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Member>(GetUrlBuilder.GetMember(memberId), cancellationToken);
        }

        /// <summary>
        /// Add one or more Members to a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="memberIdsToAdd">One or more Ids of Members to add</param>
        public async Task<Card> AddMembersToCardAsync(string cardId, params string[] memberIdsToAdd)
        {
            return await AddMembersToCardAsync(cardId, CancellationToken.None, memberIdsToAdd);
        }

        /// <summary>
        /// Add one or more Members to a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <param name="memberIdsToAdd">One or more Ids of Members to add</param>
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
                CardUpdate.Members(card.MemberIds)
            }, cancellationToken);
        }

        /// <summary>
        /// Remove a Member of a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="memberIdsToRemove">One or more Ids of Members to remove</param>
        public async Task<Card> RemoveMembersFromCardAsync(string cardId, params string[] memberIdsToRemove)
        {
            return await RemoveMembersFromCardAsync(cardId, CancellationToken.None, memberIdsToRemove);
        }

        /// <summary>
        /// Remove one or more Members from a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <param name="memberIdsToRemove">One or more Ids of Members to remove</param>
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
                CardUpdate.Members(card.MemberIds)
            }, cancellationToken);
        }

        /// <summary>
        /// Remove all Members from a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task<Card> RemoveAllMembersFromCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Members(new List<string>())
            }, cancellationToken);
        }

        /// <summary>
        /// Add a Member to a board (aka give them access)
        /// </summary>
        /// <param name="boardId">Id of the Board to give access to</param>
        /// <param name="memberId">Id of the Member that need access</param>
        /// <param name="membershipType">What type of access the member should be given</param>
        /// <param name="allowBillableGuest">Optional param that allows organization admins to add multi-board guests onto a board.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task AddMemberToBoardAsync(string boardId, string memberId, MembershipType membershipType, bool allowBillableGuest = false, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Put($"{UrlPaths.Boards}/{boardId}/{UrlPaths.Members}/{memberId}", cancellationToken, 0,
                new QueryParameter("type", membershipType.GetJsonPropertyName()),
                new QueryParameter("allowBillableGuest", allowBillableGuest));
        }

        /// <summary>
        /// Invite a Member to a board via email (aka give them access)
        /// </summary>
        /// <param name="boardId">Id of the Board to give access to</param>
        /// <param name="email">Email to invite</param>
        /// <param name="membershipType">What type of access the member should be given</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task InviteMemberToBoardViaEmailAsync(string boardId, string email, MembershipType membershipType, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Put($"{UrlPaths.Boards}/{boardId}/{UrlPaths.Members}", cancellationToken, 0,
                new QueryParameter("type", membershipType.GetJsonPropertyName()),
                new QueryParameter("email", email));
        }

        /// <summary>
        /// Remove a Member from a board (aka revoke access)
        /// </summary>
        /// <param name="boardId">Id of the Board the member should be removed from</param>
        /// <param name="memberId">Id of the Member that should be removed</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task RemoveMemberFromBoardAsync(string boardId, string memberId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Boards}/{boardId}/{UrlPaths.Members}/{memberId}", cancellationToken, 0);
        }

        /// <summary>
        /// Get information about the Member that owns the token used by this TrelloClient
        /// </summary>
        /// <returns>The Member</returns>
        public async Task<Member> GetTokenMemberAsync(CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Member>(GetUrlBuilder.GetTokenMember(_apiRequestController.Token), cancellationToken);
        }

        /// <summary>
        /// Get information about the Member that owns the token used by this TrelloClient
        /// <param name="options">Option for what data to include on the Member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// </summary>
        /// <returns>The Member</returns>
        public async Task<Member> GetTokenMemberAsync(GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Member>(GetUrlBuilder.GetTokenMember(_apiRequestController.Token), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Get the Members (users) of an Organization (aka Workspace)
        /// </summary>
        /// <param name="organizationId">Id of the Organization</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Members</returns>
        public async Task<List<Member>> GetMembersOfOrganizationAsync(string organizationId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfOrganization(organizationId), cancellationToken);
        }

        /// <summary>
        /// Get the Members (users) of an Organization (aka Workspace)
        /// </summary>
        /// <param name="organizationId">Id of the Organization</param>
        /// <param name="options">Option for what data to include on the Member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Members</returns>
        public async Task<List<Member>> GetMembersOfOrganizationAsync(string organizationId, GetMemberOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Member>>(GetUrlBuilder.GetMembersOfOrganization(organizationId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Add a member vote to a card
        /// </summary>
        /// <param name="cardId">The cardId to add the vote to</param>
        /// <param name="memberId">The id of the member that cast the vote</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task AddVoteToCardAsync(string cardId, string memberId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Post($"{UrlPaths.Cards}/{cardId}/membersVoted", cancellationToken, 0, new QueryParameter("value", memberId));
        }

        /// <summary>
        /// Remove a member vote from a card
        /// </summary>
        /// <param name="cardId">The cardId to add the vote to</param>
        /// <param name="memberId">The id of the member that cast the vote</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task RemoveVoteFromCardAsync(string cardId, string memberId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Cards}/{cardId}/membersVoted/{memberId}", cancellationToken, 0);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetBoardOptions;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Add a new Board
        /// </summary>
        /// <param name="board">The Board to Add</param>
        /// <param name="options">Options for the new board</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The New Board</returns>
        public async Task<Board> AddBoardAsync(Board board, AddBoardOptions options = null, CancellationToken cancellationToken = default)
        {
            List<QueryParameter> parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(board).ToList();
            if (options != null)
            {
                parameters.AddRange(_queryParametersBuilder.GetViaQueryParameterAttributes(options));
            }

            return await _apiRequestController.Post<Board>($"{UrlPaths.Boards}", cancellationToken, parameters.ToArray());
        }

        /// <summary>
        /// Close (Archive) a Board
        /// </summary>
        /// <param name="boardId">The id of board that should be closed</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Closed Board</returns>
        public async Task<Board> CloseBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Board>($"{UrlPaths.Boards}/{boardId}", cancellationToken, new QueryParameter("closed", true));
        }

        /// <summary>
        /// ReOpen a Board that was previously archived
        /// </summary>
        /// <param name="boardId">The id of board that should be reopened</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The ReOpened Board</returns>
        public async Task<Board> ReOpenBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Board>($"{UrlPaths.Boards}/{boardId}", cancellationToken, new QueryParameter("closed", false));
        }

        /// <summary>
        /// Update a Board
        /// </summary>
        /// <param name="boardWithChanges">The board with the changes</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Updated Card</returns>
        public async Task<Board> UpdateBoardAsync(Board boardWithChanges, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Board>($"{UrlPaths.Boards}/{boardWithChanges.Id}", cancellationToken, _queryParametersBuilder.GetViaQueryParameterAttributes(boardWithChanges));
        }

        /// <summary>
        /// Delete an entire board (WARNING: THERE IS NO WAY GOING BACK!!!). Alternative use CloseBoardAsync() for non-permanency
        /// </summary>
        /// <remarks>
        /// As this is a major thing, there is a secondary confirm needed by setting: Options.AllowDeleteOfBoards = true
        /// </remarks>
        /// <param name="boardId">The id of the Board to Delete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            if (Options.AllowDeleteOfBoards)
            {
                await _apiRequestController.Delete($"{UrlPaths.Boards}/{boardId}", cancellationToken, 0);
            }
            else
            {
                throw new SecurityException("Deletion of Boards are disabled via Options.AllowDeleteOfBoards (You need to enable this as a secondary confirmation that you REALLY wish to use that option as there is no going back: https://support.atlassian.com/trello/docs/deleting-a-board/)");
            }
        }

        /// <summary>
        /// Get a Board by its Id
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Board</returns>
        public async Task<Board> GetBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Board>(GetUrlBuilder.GetBoard(boardId), cancellationToken);
        }

        /// <summary>
        /// Get Plan Information for a specific board (Free, Standard, Premium, Enterprise) + what features are supported
        /// </summary>
        /// <param name="boardId">Id of the Board</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Plan Info</returns>
        [Obsolete("Use version with Async-suffix [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<TrelloPlanInformation> GetTrelloPlanInformationForBoard(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<TrelloPlanInformation>(GetUrlBuilder.GetBoard(boardId, new GetBoardOptions
            {
                BoardFields = new BoardFields("name", "premiumFeatures")
            }), cancellationToken);
        }

        /// <summary>
        /// Get Plan Information for a specific board (Free, Standard, Premium, Enterprise) + what features are supported
        /// </summary>
        /// <param name="boardId">Id of the Board</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Plan Info</returns>
        public async Task<TrelloPlanInformation> GetTrelloPlanInformationForBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<TrelloPlanInformation>(GetUrlBuilder.GetBoard(boardId, new GetBoardOptions
            {
                BoardFields = new BoardFields("name", "premiumFeatures")
            }), cancellationToken);
        }

        /// <summary>
        /// Get a Board by its Id
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="options">Options on what should be included on the board</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Board</returns>
        public async Task<Board> GetBoardAsync(string boardId, GetBoardOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Board>(GetUrlBuilder.GetBoard(boardId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Get the Boards that the specified member has access to
        /// </summary>
        /// <param name="memberId">Id of the Member to find boards for</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Active Boards there is access to</returns>
        public async Task<List<Board>> GetBoardsForMemberAsync(string memberId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Board>>(GetUrlBuilder.GetBoardsForMember(memberId), cancellationToken);
        }

        /// <summary>
        /// Get the Boards that the specified member has access to
        /// </summary>
        /// <param name="memberId">Id of the Member to find boards for</param>
        /// <param name="options">Options on what should be included on the board</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Active Boards there is access to</returns>
        public async Task<List<Board>> GetBoardsForMemberAsync(string memberId, GetBoardOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Board>>(GetUrlBuilder.GetBoardsForMember(memberId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Get the Boards that the token provided to the TrelloClient can Access
        /// </summary>
        /// <returns>The Active Boards there is access to</returns>
        public async Task<List<Board>> GetBoardsCurrentTokenCanAccessAsync(CancellationToken cancellationToken = default)
        {
            Member tokenMember = await GetTokenMemberAsync(cancellationToken);
            return await GetBoardsForMemberAsync(tokenMember.Id, cancellationToken);
        }

        /// <summary>
        /// Get the Boards that the token provided to the TrelloClient can Access
        /// <param name="options">Options on what should be included on the board</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// </summary>
        /// <returns>The Active Boards there is access to</returns>
        public async Task<List<Board>> GetBoardsCurrentTokenCanAccessAsync(GetBoardOptions options, CancellationToken cancellationToken = default)
        {
            Member tokenMember = await GetTokenMemberAsync(cancellationToken);
            return await GetBoardsForMemberAsync(tokenMember.Id, options, cancellationToken);
        }

        /// <summary>
        /// Get the Boards in an Organization
        /// </summary>
        /// <param name="organizationId">Id of the Organization</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Active Boards in the Organization</returns>
        [Obsolete("Use version with Async-suffix [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<Board>> GetBoardsInOrganization(string organizationId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Board>>(GetUrlBuilder.GetBoardsInOrganization(organizationId), cancellationToken);
        }

        /// <summary>
        /// Get the Boards in an Organization
        /// </summary>
        /// <param name="organizationId">Id of the Organization</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Active Boards in the Organization</returns>
        public async Task<List<Board>> GetBoardsInOrganizationAsync(string organizationId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Board>>(GetUrlBuilder.GetBoardsInOrganization(organizationId), cancellationToken);
        }

        /// <summary>
        /// Get the Boards in an Organization
        /// </summary>
        /// <param name="organizationId">Id of the Organization</param>
        /// <param name="options">Options on what should be included on the board</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Active Boards in the Organization</returns>
        [Obsolete("Use version with Async-suffix [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<Board>> GetBoardsInOrganization(string organizationId, GetBoardOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Board>>(GetUrlBuilder.GetBoardsInOrganization(organizationId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Get the Boards in an Organization
        /// </summary>
        /// <param name="organizationId">Id of the Organization</param>
        /// <param name="options">Options on what should be included on the board</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Active Boards in the Organization</returns>
        public async Task<List<Board>> GetBoardsInOrganizationAsync(string organizationId, GetBoardOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Board>>(GetUrlBuilder.GetBoardsInOrganization(organizationId), cancellationToken, options.GetParameters());
        }
    }
}
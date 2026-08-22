using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Actions;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Add a new Comment on a Card
        /// </summary>
        /// <paramref name="cardId">Id of the Card</paramref>
        /// <paramref name="comment">The Comment</paramref>
        /// <returns>The Comment Action</returns>
        public async Task<TrelloAction> AddCommentAsync(string cardId, Comment comment, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Post<TrelloAction>($"{UrlPaths.Cards}/{cardId}/actions/comments", cancellationToken, _queryParametersBuilder.GetViaQueryParameterAttributes(comment));
        }

        /// <summary>
        /// Update a comment Action (aka only way to update comments as they are not seen as their own objects)
        /// </summary>
        /// <param name="commentAction">The comment Action with the updated text</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task<TrelloAction> UpdateCommentActionAsync(TrelloAction commentAction, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<TrelloAction>($"{UrlPaths.Actions}/{commentAction.Id}", cancellationToken, new QueryParameter("text", commentAction.Data.Text));
        }

        /// <summary>
        /// Delete a Comment (WARNING: THERE IS NO WAY GOING BACK!!!).
        /// </summary>
        /// <param name="commentActionId">Id of Comment Action Id</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteCommentActionAsync(string commentActionId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Actions}/{commentActionId}", cancellationToken, 0);
        }

        /// <summary>
        /// Get All Comments on a Card
        /// </summary>
        /// <param name="cardId">Id of Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Comments</returns>
        public async Task<List<TrelloAction>> GetAllCommentsOnCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            List<TrelloAction> result = new List<TrelloAction>();
            int page = 0;
            bool moreComments = true;
            do
            {
                List<TrelloAction> comments = await GetPagedCommentsOnCardAsync(cardId, page, cancellationToken);
                page++;
                if (comments.Any())
                {
                    result.AddRange(comments);
                }
                else
                {
                    moreComments = false;
                }
            } while (moreComments);

            return result;
        }

        /// <summary>
        /// Get Paged Comments on a Card (Note: this method can max return up to 50 comments. For more use the page parameter [note: the API can't give you back how many there are in total so you need to try until non is returned])
        /// </summary>
        /// <param name="cardId">Id of Card</param>
        /// <param name="page">The page of results for actions. Each page of results has 50 actions. (Default: 0, Maximum: 19)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Comments</returns>
        public async Task<List<TrelloAction>> GetPagedCommentsOnCardAsync(string cardId, int page = 0, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<TrelloAction>>(GetUrlBuilder.GetActionsOnCard(cardId), cancellationToken,
                new QueryParameter("filter", "commentCard"),
                new QueryParameter("page", page));
        }

        /// <summary>
        /// The reactions to a comment
        /// </summary>
        /// <param name="commentActionId">Id of the Comment (ActionId)</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The Reactions</returns>
        [Obsolete("Use version with Async-suffix [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<CommentReaction>> GetCommentReactions(string commentActionId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<CommentReaction>>(GetUrlBuilder.GetCommentReactions(commentActionId), cancellationToken);
        }

        /// <summary>
        /// The reactions to a comment
        /// </summary>
        /// <param name="commentActionId">Id of the Comment (ActionId)</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The Reactions</returns>
        public async Task<List<CommentReaction>> GetCommentReactionsAsync(string commentActionId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<CommentReaction>>(GetUrlBuilder.GetCommentReactions(commentActionId), cancellationToken);
        }
    }
}
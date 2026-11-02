using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        /// Adds a new comment to a card.
        /// </summary>
        /// <param name="cardId">ID of the card to add the comment to</param>
        /// <param name="comment">The comment object containing the text to add</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The resulting comment action</returns>
        public async Task<TrelloAction> AddCommentAsync(string cardId, Comment comment, CancellationToken cancellationToken = default)
        {
            string payload = JsonSerializer.Serialize(new { text = comment.Text });
            return await _apiRequestController.PostWithJsonPayload<TrelloAction>($"{UrlPaths.Cards}/{cardId}/actions/comments", cancellationToken, payload);
        }

        /// <summary>
        /// Updates the text of an existing comment action (comments in Trello are represented as actions, so this is the only way to update a comment).
        /// </summary>
        /// <param name="commentAction">The comment action object with updated text</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated comment action</returns>
        public async Task<TrelloAction> UpdateCommentActionAsync(TrelloAction commentAction, CancellationToken cancellationToken = default)
        {
            string payload = JsonSerializer.Serialize(new { text = commentAction.Data.Text });
            return await _apiRequestController.PutWithJsonPayload<TrelloAction>($"{UrlPaths.Actions}/{commentAction.Id}", cancellationToken, payload);
        }

        /// <summary>
        /// Deletes a comment from a card. This operation is irreversible.
        /// </summary>
        /// <param name="commentActionId">ID of the comment action to delete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteCommentActionAsync(string commentActionId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Actions}/{commentActionId}", cancellationToken, 0);
        }

        /// <summary>
        /// Retrieves all comments on a specific card, handling pagination automatically to return the complete list.
        /// </summary>
        /// <param name="cardId">ID of the card to retrieve comments from</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of comment actions on the card</returns>
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
        /// Retrieves a single page of comments on a card. Each page contains up to 50 comments. Use the page parameter to paginate through results.
        /// </summary>
        /// <param name="cardId">ID of the card to retrieve comments from</param>
        /// <param name="page">The page number of results to retrieve (each page contains 50 comments, default is 0, maximum is 19)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of comment actions on the specified page</returns>
        public async Task<List<TrelloAction>> GetPagedCommentsOnCardAsync(string cardId, int page = 0, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<TrelloAction>>(GetUrlBuilder.GetActionsOnCard(cardId), cancellationToken,
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Filter, "commentCard"),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Page, page));
        }

        /// <summary>
        /// Retrieves all reactions associated with a specific comment action.
        /// </summary>
        /// <param name="commentActionId">ID of the comment action to get reactions for</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of reactions for the comment</returns>
        public async Task<List<CommentReaction>> GetCommentReactionsAsync(string commentActionId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<CommentReaction>>(GetUrlBuilder.GetCommentReactions(commentActionId), cancellationToken);
        }

        /// <summary>
        /// Adds a reaction (emoji) to a specific comment action.
        /// </summary>
        /// <param name="commentActionId">ID of the comment action to add the reaction to</param>
        /// <param name="reaction">The reaction object to add</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The created comment reaction object</returns>
        public async Task<CommentReaction> AddCommentReactionAsync(string commentActionId, Reaction reaction, CancellationToken cancellationToken = default)
        {
            string payload = JsonSerializer.Serialize(reaction);
            return await _apiRequestController.PostWithJsonPayload<CommentReaction>($"{UrlPaths.Actions}/{commentActionId}/reactions", cancellationToken, payload);
        }

        /// <summary>
        /// Deletes a reaction from a comment action. This operation is irreversible.
        /// </summary>
        /// <param name="commentActionId">ID of the comment action containing the reaction</param>
        /// <param name="commentReactionId">ID of the reaction to delete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteReactionFromCommentAsync(string commentActionId, string commentReactionId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Actions}/{commentActionId}/reactions/{commentReactionId}", cancellationToken, 0);
        }
    }
}



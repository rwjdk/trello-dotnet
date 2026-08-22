using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.GetLabelOptions;

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Delete a Label from the board and remove it from all cards it was added to (WARNING: THERE IS NO WAY GOING BACK!!!). If you are looking to remove a label from a Card then see 'RemoveLabelsFromCardAsync' and 'RemoveAllLabelsFromCardAsync'
        /// </summary>
        /// <param name="labelId">The id of the Label to Delete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteLabelAsync(string labelId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Labels}/{labelId}", cancellationToken, 0);
        }

        /// <summary>
        /// Get a list of Labels defined for a board
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Labels</returns>
        public async Task<List<Label>> GetLabelsOfBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Label>>(GetUrlBuilder.GetLabelsOfBoard(boardId), cancellationToken);
        }

        /// <summary>
        /// Get a list of Labels defined for a board
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="options">Option on how and what to get</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Labels</returns>
        public async Task<List<Label>> GetLabelsOfBoardAsync(string boardId, GetLabelOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Label>>(GetUrlBuilder.GetLabelsOfBoard(boardId), cancellationToken, options.GetParameters());
        }

        /// <summary>
        /// Add a Label to a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="labelIdsToAdd">One or more Ids of Labels to add</param>
        public async Task<Card> AddLabelsToCardAsync(string cardId, params string[] labelIdsToAdd)
        {
            return await AddLabelsToCardAsync(cardId, CancellationToken.None, labelIdsToAdd);
        }

        /// <summary>
        /// Add a Label to a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <param name="labelIdsToAdd">One or more Ids of Labels to add</param>
        public async Task<Card> AddLabelsToCardAsync(string cardId, CancellationToken cancellationToken = default, params string[] labelIdsToAdd)
        {
            Card card = await GetCardAsync(cardId, cancellationToken);
            List<string> missing = labelIdsToAdd.Where(x => !card.LabelIds.Contains(x)).ToList();

            if (missing.Count == 0)
            {
                return card; //All already There
            }

            //Need update
            card.LabelIds.AddRange(missing);
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Labels(card.LabelIds)
            }, cancellationToken);
        }

        /// <summary>
        /// Remove a Label of a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="labelIdsToRemove">One or more Ids of Labels to remove</param>
        public async Task<Card> RemoveLabelsFromCardAsync(string cardId, params string[] labelIdsToRemove)
        {
            return await RemoveLabelsFromCardAsync(cardId, CancellationToken.None, labelIdsToRemove);
        }

        /// <summary>
        /// Remove one or more Labels from a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <param name="labelIdsToRemove">One or more Ids of Labels to remove</param>
        public async Task<Card> RemoveLabelsFromCardAsync(string cardId, CancellationToken cancellationToken, params string[] labelIdsToRemove)
        {
            Card card = await GetCardAsync(cardId, cancellationToken);
            List<string> toRemove = labelIdsToRemove.Where(x => card.LabelIds.Contains(x)).ToList();
            if (toRemove.Count == 0)
            {
                return card; //All not there
            }

            //Need update
            card.LabelIds = card.LabelIds.Except(toRemove).ToList();
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Labels(card.LabelIds)
            }, cancellationToken);
        }

        /// <summary>
        /// Remove all Labels from a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task<Card> RemoveAllLabelsFromCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Labels(new List<string>())
            }, cancellationToken);
        }

        /// <summary>
        /// Update the definition of a label (Name and Color)
        /// </summary>
        /// <param name="labelWithUpdates">The label with updates</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task<Label> UpdateLabelAsync(Label labelWithUpdates, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Label>($"{UrlPaths.Labels}/{labelWithUpdates.Id}", cancellationToken, _queryParametersBuilder.GetViaQueryParameterAttributes(labelWithUpdates));
        }

        /// <summary>
        /// Add a new label to the Board (Not to be confused with 'AddLabelsToCardAsync' that assign labels to cards)
        /// </summary>
        /// <param name="label">Definition of the new label</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The newly created label</returns>
        public async Task<Label> AddLabelAsync(Label label, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Post<Label>($"{UrlPaths.Labels}", cancellationToken, _queryParametersBuilder.GetViaQueryParameterAttributes(label));
        }
    }
}
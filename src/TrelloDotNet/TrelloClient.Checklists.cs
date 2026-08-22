using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Add a Checklist to the card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="checklist">The Checklist to add</param>
        /// <param name="ignoreIfAChecklistWithThisNameAlreadyExist">If true the card will be checked if a checklist with the same name (case-sensitive) exists and if so return that instead of creating a new</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>New or Existing Checklist with same name</returns>
        public async Task<Checklist> AddChecklistAsync(string cardId, Checklist checklist, bool ignoreIfAChecklistWithThisNameAlreadyExist = false, CancellationToken cancellationToken = default)
        {
            Checklist template = checklist;
            if (ignoreIfAChecklistWithThisNameAlreadyExist)
            {
                //Check if card already have a list with same name
                List<Checklist> existingOnCard = await GetChecklistsOnCardAsync(cardId, cancellationToken);
                Checklist existing = existingOnCard.FirstOrDefault(x => x.Name == template.Name);
                if (existing != null)
                {
                    return existing;
                }
            }

            QueryParameter[] checklistParameters = _queryParametersBuilder.GetViaQueryParameterAttributes(template);
            _queryParametersBuilder.AdjustForNamedPosition(checklistParameters, checklist.NamedPosition);
            Checklist newChecklist = await _apiRequestController.Post<Checklist>($"{UrlPaths.Cards}/{cardId}/{UrlPaths.Checklists}", cancellationToken, checklistParameters);

            if (template.Items == null)
            {
                return newChecklist;
            }

            if (template.Items.TrueForAll(x => x.Position == 0)) //Give positions to have the system have same order as in list
            {
                int position = 1;
                foreach (ChecklistItem item in template.Items)
                {
                    item.Position = position;
                    position++;
                }
            }

            await AddCheckItemsAsync(newChecklist, template.Items.ToArray());

            return newChecklist;
        }

        /// <summary>
        /// Add a new Checklist item to an existing Checklist
        /// </summary>
        /// <param name="checklistId">Checklist Id</param>
        /// <param name="checkItemToAdd">New Checklist Item</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The new Checklist Item</returns>
        public async Task<ChecklistItem> AddChecklistItemAsync(string checklistId, ChecklistItem checkItemToAdd, CancellationToken cancellationToken = default)
        {
            QueryParameter[] parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(checkItemToAdd);
            _queryParametersBuilder.AdjustForNamedPosition(parameters, checkItemToAdd.NamedPosition);
            return await _apiRequestController.Post<ChecklistItem>($"{UrlPaths.Checklists}/{checklistId}/{UrlPaths.CheckItems}", cancellationToken, parameters);
        }

        internal async Task AddCheckItemsAsync(Checklist existingChecklist, params ChecklistItem[] checkItemsToAdd)
        {
            foreach (QueryParameter[] checkItemParameters in checkItemsToAdd.Select(item =>
                     {
                         QueryParameter[] parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(item);
                         _queryParametersBuilder.AdjustForNamedPosition(parameters, item.NamedPosition);
                         return parameters;
                     }))
            {
                existingChecklist.Items.Add(await _apiRequestController.Post<ChecklistItem>($"{UrlPaths.Checklists}/{existingChecklist.Id}/{UrlPaths.CheckItems}", CancellationToken.None, checkItemParameters));
            }
        }

        /// <summary>
        /// Add a Checklist to the card based on an existing checklist (as a copy)
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="existingChecklistIdToCopyFrom">Id of an existing Checklist that should be added to the card as a new copy</param>
        /// <param name="ignoreIfAChecklistWithThisNameAlreadyExist">If true the card will be checked if a checklist with same name (case-sensitive) exist and if so return that instead of creating a new</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>New Checklist</returns>
        public async Task<Checklist> AddChecklistAsync(string cardId, string existingChecklistIdToCopyFrom, bool ignoreIfAChecklistWithThisNameAlreadyExist = false, CancellationToken cancellationToken = default)
        {
            return await AddChecklistAsync(cardId, existingChecklistIdToCopyFrom, ignoreIfAChecklistWithThisNameAlreadyExist, null, cancellationToken);
        }

        /// <summary>
        /// Add a Checklist to the card based on an existing checklist (as a copy)
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="existingChecklistIdToCopyFrom">Id of an existing Checklist that should be added to the card as a new copy</param>
        /// <param name="ignoreIfAChecklistWithThisNameAlreadyExist">If true the card will be checked if a checklist with same name (case-sensitive) exist and if so return that instead of creating a new</param>
        /// <param name="namedPosition">Named position of the checklist (Top or Bottom)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>New Checklist</returns>
        public async Task<Checklist> AddChecklistAsync(string cardId, string existingChecklistIdToCopyFrom, bool ignoreIfAChecklistWithThisNameAlreadyExist = false, NamedPosition? namedPosition = null, CancellationToken cancellationToken = default)
        {
            if (ignoreIfAChecklistWithThisNameAlreadyExist)
            {
                //Find the name of the template (as we are only provided Id)
                Checklist existingChecklist = await GetChecklistAsync(existingChecklistIdToCopyFrom, cancellationToken);
                //Check if card already have a list with same name
                List<Checklist> existingOnCard = await GetChecklistsOnCardAsync(cardId, cancellationToken);
                Checklist existing = existingOnCard.FirstOrDefault(x => x.Name == existingChecklist.Name);
                if (existing != null)
                {
                    return existing;
                }
            }

            QueryParameter[] parameters =
            {
                new QueryParameter("idChecklistSource", existingChecklistIdToCopyFrom)
            };
            _queryParametersBuilder.AdjustForNamedPosition(parameters, namedPosition);
            return await _apiRequestController.Post<Checklist>($"{UrlPaths.Cards}/{cardId}/{UrlPaths.Checklists}", cancellationToken, parameters);
        }

        /// <summary>
        /// Update a checklists name and/or position
        /// </summary>
        /// <param name="checklistId">Id of the Checklist</param>
        /// <param name="newName">The new Name or null if you do not wish to change it</param>
        /// <param name="namedPosition">The new position or null if you do not wish to change it</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Checklist</returns>
        public async Task<Checklist> UpdateChecklistAsync(string checklistId, string newName = null, NamedPosition? namedPosition = null, CancellationToken cancellationToken = default)
        {
            List<QueryParameter> parameters = new List<QueryParameter>();
            if (!string.IsNullOrWhiteSpace(newName))
            {
                parameters.Add(new QueryParameter("name", newName));
            }

            if (namedPosition.HasValue)
            {
                switch (namedPosition.Value)
                {
                    case NamedPosition.Top:
                        parameters.Add(new QueryParameter("pos", "top"));
                        break;
                    case NamedPosition.Bottom:
                        parameters.Add(new QueryParameter("pos", "bottom"));
                        break;
                    default:
                        parameters.Add(new QueryParameter("pos", Convert.ToInt32(namedPosition.Value)));
                        break;
                }
            }

            return await _apiRequestController.Put<Checklist>($"{UrlPaths.Checklists}/{checklistId}", cancellationToken, parameters.ToArray());
        }

        /// <summary>
        /// Update a Check-item on a Card
        /// </summary>
        /// <param name="cardId">The Id of the Card the ChecklistItem is on</param>
        /// <param name="item">The updated Check-item</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The Updated Checklist Item</returns>
        public async Task<ChecklistItem> UpdateChecklistItemAsync(string cardId, ChecklistItem item, CancellationToken cancellationToken = default)
        {
            QueryParameter[] parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(item);
            _queryParametersBuilder.AdjustForNamedPosition(parameters, item.NamedPosition);
            return await _apiRequestController.Put<ChecklistItem>($"{UrlPaths.Cards}/{cardId}/checkItem/{item.Id}", cancellationToken, parameters);
        }

        /// <summary>
        /// Get a Checklist with a specific Id
        /// </summary>
        /// <param name="checkListId">Id of the Checklist</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Checklist</returns>
        public Task<Checklist> GetChecklistAsync(string checkListId, CancellationToken cancellationToken = default)
        {
            return _apiRequestController.Get<Checklist>(GetUrlBuilder.GetChecklist(checkListId), cancellationToken);
        }

        /// <summary>
        /// Get a list of Checklists that are used on cards on a specific Board
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Checklists</returns>
        public async Task<List<Checklist>> GetChecklistsOnBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Checklist>>(GetUrlBuilder.GetChecklistsOnBoard(boardId), cancellationToken);
        }

        /// <summary>
        /// Get a list of Checklists that are used on a specific card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Checklists</returns>
        public async Task<List<Checklist>> GetChecklistsOnCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Checklist>>(GetUrlBuilder.GetChecklistsOnCard(cardId), cancellationToken);
        }

        /// <summary>
        /// Delete a Checklist (WARNING: THERE IS NO WAY GOING BACK!!!)
        /// </summary>
        /// <param name="checklistId">The id of the Checklist to Delete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteChecklistAsync(string checklistId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Checklists}/{checklistId}", cancellationToken, 0);
        }

        /// <summary>
        /// Delete a Checklist Item from a checklist (WARNING: THERE IS NO WAY GOING BACK!!!)
        /// </summary>
        /// <param name="checklistId">The id of the Checklist</param>
        /// <param name="checklistItemId">The id of the Checklist Item to delete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteChecklistItemAsync(string checklistId, string checklistItemId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Checklists}/{checklistId}/{UrlPaths.CheckItems}/{checklistItemId}", cancellationToken, 0);
        }
    }
}
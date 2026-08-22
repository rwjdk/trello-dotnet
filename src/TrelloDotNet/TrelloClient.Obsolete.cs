using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Actions;
using TrelloDotNet.Model.Options.GetActionsOptions;
using TrelloDotNet.Model.Options.GetCardOptions;

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Add a Card
        /// </summary>
        /// <param name="card">The Card to Add</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Added Card</returns>
        [Obsolete("Use AddCard(AddCardOptions) instead for more options [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<Card> AddCardAsync(Card card, CancellationToken cancellationToken = default)
        {
            QueryParameter[] parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(card);
            _queryParametersBuilder.AdjustForNamedPosition(parameters, card.NamedPosition);
            Card result = await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters);
            if (card.Cover != null)
            {
                return await AddCoverToCardAsync(result.Id, card.Cover, cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Add a Template Card
        /// </summary>
        /// <param name="card">The Card Template to Add</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Added Template</returns>
        [Obsolete("Use AddCardTemplateAsync(AddCardOptions) instead [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<Card> AddCardTemplateAsync(Card card, CancellationToken cancellationToken = default)
        {
            card.IsTemplate = true;
            QueryParameter[] parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(card);

            _queryParametersBuilder.AdjustForNamedPosition(parameters, card.NamedPosition);
            Card result = await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters);
            if (card.Cover != null)
            {
                return await AddCoverToCardAsync(result.Id, card.Cover, cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Get the cards on board based on their status regardless if they are on archived lists
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="filter">The Selected Filter</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Cards</returns>
        [Obsolete("Use GetCardsOnBoardAsync with GetCardOptions.Filter [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<Card>> GetCardsOnBoardFilteredAsync(string boardId, CardsFilter filter, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Card>>($"{GetUrlBuilder.GetCardsOnBoard(boardId)}/{filter.GetJsonPropertyName()}", cancellationToken,
#pragma warning disable CS0618 // Type or member is obsolete
                new QueryParameter("customFieldItems", Options.IncludeCustomFieldsInCardGetMethods),
                new QueryParameter("attachments", Options.IncludeAttachmentsInCardGetMethods));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Get the cards on board based on their status regardless if they are on archived lists
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="filter">The Selected Filter</param>
        /// <param name="options">Options on how and what should be included on the cards (Example only a few fields to increase performance or more nested data to avoid more API calls)</param> 
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Cards</returns>
        [Obsolete("Use GetCardsOnBoardAsync with GetCardOptions.Filter [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<Card>> GetCardsOnBoardFilteredAsync(string boardId, CardsFilter filter, GetCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options.IncludeList)
            {
                if (options.CardFields != null && !options.CardFields.Fields.Contains("idList"))
                {
                    options.CardFields.Fields = options.CardFields.Fields.Union(new List<string>
                    {
                        "idList"
                    }).ToArray();
                }
            }

            List<Card> cards = await _apiRequestController.Get<List<Card>>($"{GetUrlBuilder.GetCardsOnBoard(boardId)}/{filter.GetJsonPropertyName()}", cancellationToken, options.GetParameters(true));
            if (options.IncludeList)
            {
                List<List> lists;
                switch (filter)
                {
                    case CardsFilter.All:
                    case CardsFilter.Closed:
                    case CardsFilter.Visible:
                        lists = await GetListsOnBoardFilteredAsync(boardId, ListFilter.All, cancellationToken);
                        break;
                    case CardsFilter.Open:
                        lists = await GetListsOnBoardAsync(boardId, cancellationToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
                }

                foreach (Card card in cards)
                {
                    card.List = lists.FirstOrDefault(x => x.Id == card.ListId);
                }
            }

            return cards;
        }

        /// <summary>
        /// Get the most recent Actions (Changelog Events) of a board
        /// </summary>
        /// <param name="boardId">The Id of the Board</param>
        /// <param name="filter">A set of event-types to filter by (You can see a list of event in TrelloDotNet.Model.Webhook.WebhookActionTypes)</param>
        /// <param name="limit">How many recent events to get back; Default 50, Max 1000</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <param name="page">The page of results for actions</param>
        /// <param name="before">An Action ID</param>
        /// <param name="since">An Action ID</param>
        /// <returns>List of most Recent Trello Actions</returns>
        [Obsolete("Use overload the take in the 'GetActionsOptions' instead [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<TrelloAction>> GetActionsOfBoardAsync(string boardId, List<string> filter = null, int limit = 50, CancellationToken cancellationToken = default, int page = 0, string before = null, string since = null)
        {
            return await GetActionsFromSuffix(GetUrlBuilder.GetActionsOnBoard(boardId), new GetActionsOptions
            {
                Limit = limit,
                Before = before,
                Filter = filter,
                Page = page,
                Since = since
            }, cancellationToken);
        }

        /// <summary>
        /// Get the most recent Actions (Changelog Events) on a Card
        /// </summary>
        /// <remarks>
        /// If no filter is given the default filter of Trello API is 'commentCard, updateCard:idList' (aka 'Move Card To List' and 'Add Comment')
        /// </remarks>
        /// <param name="cardId">The Id of the Card</param>
        /// <param name="filter">A set of event-types to filter by (You can see a list of event in TrelloDotNet.Model.Webhook.WebhookActionTypes). Default: 'commentCard, updateCard:idList' (aka 'Move Card To List' and 'Add Comment')</param>
        /// <param name="limit">How many recent events to get back; Default 50, Max 1000</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of most Recent Trello Actions</returns>
        [Obsolete("Use overload the take in the 'GetActionsOptions' instead [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<TrelloAction>> GetActionsOnCardAsync(string cardId, List<string> filter = null, int limit = 50, CancellationToken cancellationToken = default)
        {
            return await GetActionsFromSuffix(GetUrlBuilder.GetActionsOnCard(cardId), new GetActionsOptions
            {
                Limit = limit,
                Filter = filter,
            }, cancellationToken);
        }

        /// <summary>
        /// Get the most recent Actions (Changelog Events) for a List
        /// </summary>
        /// <param name="listId">The Id of the List</param>
        /// <param name="filter">A set of event-types to filter by (You can see a list of event in TrelloDotNet.Model.Webhook.WebhookActionTypes)</param>
        /// <param name="limit">How many recent events to get back; Default 50, Max 1000</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of most Recent Trello Actions</returns>
        [Obsolete("Use overload the take in the 'GetActionsOptions' instead [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<TrelloAction>> GetActionsForListAsync(string listId, List<string> filter = null, int limit = 50, CancellationToken cancellationToken = default)
        {
            return await GetActionsFromSuffix(GetUrlBuilder.GetActionsForList(listId), new GetActionsOptions
            {
                Limit = limit,
                Filter = filter,
            }, cancellationToken);
        }

        /// <summary>
        /// Get the most recent Actions (Changelog Events) for a Member
        /// </summary>
        /// <param name="memberId">The Id of the Member</param>
        /// <param name="filter">A set of event-types to filter by (You can see a list of event in TrelloDotNet.Model.Webhook.WebhookActionTypes)</param>
        /// <param name="limit">How many recent events to get back; Default 50, Max 1000</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of most Recent Trello Actions</returns>
        [Obsolete("Use overload the take in the 'GetActionsOptions' instead [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<TrelloAction>> GetActionsForMemberAsync(string memberId, List<string> filter = null, int limit = 50, CancellationToken cancellationToken = default)
        {
            return await GetActionsFromSuffix(GetUrlBuilder.GetActionsForMember(memberId), new GetActionsOptions
            {
                Limit = limit,
                Filter = filter,
            }, cancellationToken);
        }

        /// <summary>
        /// Get the most recent Actions (Changelog Events) for an Organization
        /// </summary>
        /// <remarks>
        /// Only organization-specific actions will be returned. For the actions on the boards, see GetActionsOfBoardAsync
        /// </remarks>
        /// <param name="organizationId">The Id of the Organization</param>
        /// <param name="filter">A set of event-types to filter by (You can see a list of event in TrelloDotNet.Model.Webhook.WebhookActionTypes)</param>
        /// <param name="limit">How many recent events to get back; Default 50, Max 1000</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of most Recent Trello Actions</returns>
        [Obsolete("Use overload the take in the 'GetActionsOptions' instead [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<TrelloAction>> GetActionsForOrganizationsAsync(string organizationId, List<string> filter = null, int limit = 50, CancellationToken cancellationToken = default)
        {
            return await GetActionsFromSuffix(GetUrlBuilder.GetActionsForOrganization(organizationId), new GetActionsOptions
            {
                Limit = limit,
                Filter = filter,
            }, cancellationToken);
        }

        /// <summary>
        /// Update a Card
        /// </summary>
        /// <param name="cardWithChanges">The card with the changes</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Updated Card</returns>
        [Obsolete("Use UpdateCard with 'List<CardUpdate> valuesToUpdate' as it have much better performance due to delta changes and avoid lost changes between get and update [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<Card> UpdateCardAsync(Card cardWithChanges, CancellationToken cancellationToken = default)
        {
            List<QueryParameter> parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(cardWithChanges).ToList();
            CardCover cardCover = cardWithChanges.Cover;
            _queryParametersBuilder.AdjustForNamedPosition(parameters, cardWithChanges.NamedPosition);
            string payload = GeneratePayloadForCoverUpdate(cardCover, parameters);
            return await _apiRequestController.PutWithJsonPayload<Card>($"{UrlPaths.Cards}/{cardWithChanges.Id}", cancellationToken, payload, parameters.ToArray());
        }

        /// <summary>
        /// Update one or more specific fields on a card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="parameters">The Specific Parameters to set (aka nerdy way; use 'valuesToUpdate' variant for more user-friendly way)</param>
        /// <param name="cancellationToken">CancellationToken</param>
        [Obsolete("Use UpdateCardAsync(string cardId, List<CardUpdate> valuesToUpdate...) variant (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)")]
        public async Task<Card> UpdateCardAsync(string cardId, List<QueryParameter> parameters, CancellationToken cancellationToken = default)
        {
            QueryParameter coverParameter = parameters.FirstOrDefault(x => x.Name == "cover");
            if (coverParameter != null && !string.IsNullOrWhiteSpace(coverParameter.GetRawStringValue()))
            {
                //Special Cover Card
                parameters.Remove(coverParameter);
                CardCover cover = JsonSerializer.Deserialize<CardCover>(coverParameter.GetRawStringValue());
                string payload = GeneratePayloadForCoverUpdate(cover, parameters);
                return await _apiRequestController.PutWithJsonPayload<Card>($"{UrlPaths.Cards}/{cardId}", cancellationToken, payload, parameters.ToArray());
            }

            return await _apiRequestController.Put<Card>($"{UrlPaths.Cards}/{cardId}", cancellationToken, parameters.ToArray());
        }

        /// <summary>
        /// Get Lists on board based on their status
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="filter">The Selected Filter</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Cards</returns>
        [Obsolete("Use GetCardsOnBoardAsync with GetListOptions.Filter instead [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<List<List>> GetListsOnBoardFilteredAsync(string boardId, ListFilter filter, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<List>>($"{GetUrlBuilder.GetListsOnBoard(boardId)}/{filter.GetJsonPropertyName()}", cancellationToken);
        }
    }
}
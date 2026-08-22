using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.AddCardFromTemplateOptions;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Options.AddCardToInboxOptions;
using TrelloDotNet.Model.Options.CopyCardOptions;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Options.GetInboxCardOptions;
using TrelloDotNet.Model.Options.MirrorCardOptions;
using TrelloDotNet.Model.Options.MoveCardToBoardOptions;
using TrelloDotNet.Model.Options.MoveCardToListOptions;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Add a Card
        /// </summary>
        /// <param name="options">Add Card Options (Name, Dates, Checklist, etc.)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Added Card</returns>
        public async Task<Card> AddCardAsync(AddCardOptions options, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(options.ListId))
            {
                throw new TrelloApiException("No ListId provided in options (Mandatory)", string.Empty);
            }

            Card input = new Card(options.ListId, options.Name, options.Description)
            {
                IsTemplate = options.IsTemplate
            };

            if (options.Start.HasValue)
            {
                input.Start = options.Start.Value;
            }

            if (options.Due.HasValue)
            {
                input.Due = options.Due.Value;
            }

            input.DueComplete = options.DueComplete;

            if (options.Position.HasValue)
            {
                input.Position = options.Position.Value;
            }
            else
            {
                input.NamedPosition = options.NamedPosition;
            }

            if (options.LabelIds != null)
            {
                input.LabelIds = options.LabelIds;
            }

            if (options.MemberIds != null)
            {
                input.MemberIds = options.MemberIds;
            }

            QueryParameter[] parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(input);
            _queryParametersBuilder.AdjustForNamedPosition(parameters, input.NamedPosition);
            Card addedCard = await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters);

            bool needGet = false;
            GetCardOptions getCardOptions = new GetCardOptions();
            if (options.Checklists != null)
            {
                needGet = true;
                getCardOptions.IncludeChecklists = true;
                foreach (Checklist checklist in options.Checklists)
                {
                    checklist.NamedPosition = NamedPosition.Bottom;
                    await AddChecklistAsync(addedCard.Id, checklist, cancellationToken: cancellationToken);
                }
            }

            if (options.AttachmentFileUploads != null)
            {
                needGet = true;
                getCardOptions.IncludeAttachments = GetCardOptionsIncludeAttachments.True;
                foreach (AttachmentFileUpload fileUpload in options.AttachmentFileUploads)
                {
                    await AddAttachmentToCardAsync(addedCard.Id, fileUpload, cancellationToken: cancellationToken);
                }
            }

            if (options.AttachmentUrlLinks != null)
            {
                needGet = true;
                getCardOptions.IncludeAttachments = GetCardOptionsIncludeAttachments.True;
                foreach (AttachmentUrlLink urlLink in options.AttachmentUrlLinks)
                {
                    await AddAttachmentToCardAsync(addedCard.Id, urlLink, cancellationToken: cancellationToken);
                }
            }

            if (options.Cover != null)
            {
                needGet = true;
                await AddCoverToCardAsync(addedCard.Id, options.Cover, cancellationToken);
            }

            // ReSharper disable once InvertIf
            if (options.CustomFields != null)
            {
                needGet = true;
                getCardOptions.IncludeCustomFieldItems = true;
                foreach (AddCardOptionsCustomField customField in options.CustomFields)
                {
                    switch (customField.Field.Type)
                    {
                        case CustomFieldType.Checkbox:
                            await UpdateCustomFieldValueOnCardAsync(addedCard.Id, customField.Field, (bool)customField.Value, cancellationToken);
                            break;
                        case CustomFieldType.Date:
                            await UpdateCustomFieldValueOnCardAsync(addedCard.Id, customField.Field, (DateTimeOffset)customField.Value, cancellationToken);
                            break;
                        case CustomFieldType.List:
                            await UpdateCustomFieldValueOnCardAsync(addedCard.Id, customField.Field, (CustomFieldOption)customField.Value, cancellationToken);
                            break;
                        case CustomFieldType.Number:
                            switch (customField.Value)
                            {
                                case int intValue:
                                    await UpdateCustomFieldValueOnCardAsync(addedCard.Id, customField.Field, intValue, cancellationToken);
                                    break;
                                case decimal decimalValue:
                                    await UpdateCustomFieldValueOnCardAsync(addedCard.Id, customField.Field, decimalValue, cancellationToken);
                                    break;
                            }

                            break;
                        case CustomFieldType.Text:
                        default:
                            await UpdateCustomFieldValueOnCardAsync(addedCard.Id, customField.Field, (string)customField.Value, cancellationToken);
                            break;
                    }
                }
            }

            return needGet ? await GetCardAsync(addedCard.Id, getCardOptions, cancellationToken) : addedCard;
        }


        /// <summary>
        /// Add a Card to the Inbox of the Member that own the Trello token
        /// </summary>
        /// <param name="options">Add Card options (Name, Dates, Checklists, etc.)</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The Added Card</returns>
        public async Task<Card> AddCardToInboxAsync(AddCardToInboxOptions options, CancellationToken cancellationToken = default)
        {
            TokenMemberInbox inbox = await GetTokenMemberInboxAsync(cancellationToken);
            if (inbox == null)
            {
                throw new TrelloApiException("Could not find your inbox", string.Empty);
            }

            Card input = new Card(inbox.ListId, options.Name, options.Description);
            if (options.Start.HasValue)
            {
                input.Start = options.Start.Value;
            }

            if (options.Due.HasValue)
            {
                input.Due = options.Due.Value;
            }

            input.DueComplete = options.DueComplete;

            if (options.Position.HasValue)
            {
                input.Position = options.Position.Value;
            }
            else
            {
                input.NamedPosition = options.NamedPosition;
            }

            QueryParameter[] parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(input);
            Card addedCard = await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters);

            if (options.Checklists != null)
            {
                foreach (Checklist checklist in options.Checklists)
                {
                    checklist.NamedPosition = NamedPosition.Bottom;
                    await AddChecklistAsync(addedCard.Id, checklist, cancellationToken: cancellationToken);
                }
            }

            if (options.AttachmentFileUploads != null)
            {
                foreach (AttachmentFileUpload fileUpload in options.AttachmentFileUploads)
                {
                    await AddAttachmentToCardAsync(addedCard.Id, fileUpload, cancellationToken: cancellationToken);
                }
            }

            if (options.AttachmentUrlLinks != null)
            {
                foreach (AttachmentUrlLink urlLink in options.AttachmentUrlLinks)
                {
                    await AddAttachmentToCardAsync(addedCard.Id, urlLink, cancellationToken: cancellationToken);
                }
            }

            if (options.Cover != null)
            {
                await AddCoverToCardAsync(addedCard.Id, options.Cover, cancellationToken);
            }

            return addedCard;
        }

        /// <summary>
        /// Add new card based on a Template Card
        /// </summary>
        /// <param name="options">Parameters for Adding the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The New Card</returns>
        public async Task<Card> AddCardFromTemplateAsync(AddCardFromTemplateOptions options, CancellationToken cancellationToken = default)
        {
            string nameOnNewCard = !string.IsNullOrWhiteSpace(options.Name)
                ? options.Name
                : (await GetCardAsync(options.SourceTemplateCardId, new GetCardOptions
                {
                    CardFields = new CardFields(CardFieldsType.Name)
                }, cancellationToken)).Name;

            QueryParameter[] parameters =
            {
                new QueryParameter("name", nameOnNewCard),
                new QueryParameter("idList", options.TargetListId),
                new QueryParameter("pos", "bottom"),
                new QueryParameter("idCardSource", options.SourceTemplateCardId)
            };
            return await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters);
        }

        /// <summary>
        /// Copy a Card
        /// </summary>
        /// <param name="options">Parameters for copying the card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Card Copy</returns>
        public async Task<Card> CopyCardAsync(CopyCardOptions options, CancellationToken cancellationToken = default)

        {
            string nameOnNewCard = !string.IsNullOrWhiteSpace(options.Name)
                ? options.Name
                : (await GetCardAsync(options.SourceCardId, new GetCardOptions
                {
                    CardFields = new CardFields(CardFieldsType.Name)
                }, cancellationToken)).Name;

            string position = "bottom";

            if (options.Position.HasValue)
            {
                position = options.Position.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (options.NamedPosition.HasValue)
            {
                position = options.NamedPosition.Value == NamedPosition.Bottom ? "bottom" : "top";
            }

            string keepFromSource = "all";
            if (options.Keep.HasValue)
            {
                CopyCardOptionsToKeep keep = options.Keep.Value;

                if (keep.HasFlag(CopyCardOptionsToKeep.All))
                {
                    keepFromSource = "all";
                }
                else
                {
                    List<string> keepStrings = new List<string>();
                    List<CopyCardOptionsToKeep> enumValues = Enum.GetValues(typeof(CopyCardOptionsToKeep)).Cast<CopyCardOptionsToKeep>().ToList();
                    foreach (CopyCardOptionsToKeep toKeep in enumValues.Where(x => x != CopyCardOptionsToKeep.All))
                    {
                        if (keep.HasFlag(toKeep))
                        {
                            keepStrings.Add(toKeep.GetJsonPropertyName());
                        }
                    }

                    keepFromSource = string.Join(",", keepStrings);
                }
            }

            QueryParameter[] parameters =
            {
                new QueryParameter("name", nameOnNewCard),
                new QueryParameter("idList", options.TargetListId),
                new QueryParameter("pos", position),
                new QueryParameter("idCardSource", options.SourceCardId),
                new QueryParameter("keepFromSource", keepFromSource)
            };
            return await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters);
        }

        /// <summary>
        /// Mirror a Card
        /// </summary>
        /// <param name="options">Parameters for create the mirror</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Mirror Card</returns>
        public async Task<Card> MirrorCardAsync(MirrorCardOptions options, CancellationToken cancellationToken = default)
        {
            //Get Source-Card as we need the ShortUrl to make the Card Mirror magic Happen
            Card sourceCard = await GetCardAsync(options.SourceCardId, new GetCardOptions
            {
                CardFields = new CardFields(CardFieldsType.ShortUrl)
            }, cancellationToken);

            string position = "bottom";

            if (options.Position.HasValue)
            {
                position = options.Position.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (options.NamedPosition.HasValue)
            {
                position = options.NamedPosition.Value == NamedPosition.Bottom ? "bottom" : "top";
            }

            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("idList", options.TargetListId),
                new QueryParameter("name", sourceCard.ShortUrl),
                new QueryParameter("isTemplate", false),
                new QueryParameter("closed", false),
                new QueryParameter("pos", position),
                new QueryParameter("cardRole", "mirror"),
            };

            Card result = await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters.ToArray());
            return result;
        }

        /// <summary>
        /// Archive (Close) a Card
        /// </summary>
        /// <param name="cardId">The id of card that should be archived</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Archived Card</returns>
        public async Task<Card> ArchiveCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Card>($"{UrlPaths.Cards}/{cardId}", cancellationToken, new QueryParameter("closed", true));
        }

        /// <summary>
        /// ReOpen (Send back to board) a Card
        /// </summary>
        /// <param name="cardId">The id of card that should be reopened</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The ReOpened Card</returns>
        public async Task<Card> ReOpenCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Card>($"{UrlPaths.Cards}/{cardId}", cancellationToken, new QueryParameter("closed", false));
        }

        /// <summary>
        /// Update one or more specific fields on a card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="valuesToUpdate">The Specific values to set</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public async Task<Card> UpdateCardAsync(string cardId, List<CardUpdate> valuesToUpdate, CancellationToken cancellationToken = default)
        {
            List<QueryParameter> parameters = valuesToUpdate.Select(x => x.ToQueryParameter()).ToList();
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
        /// Archive all cards on in a List
        /// </summary>
        /// <param name="listId">The id of the List that should have its cards archived</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task ArchiveAllCardsInListAsync(string listId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Post<List>($"{UrlPaths.Lists}/{listId}/archiveAllCards", cancellationToken);
        }

        /// <summary>
        /// Move all cards of a list to another list
        /// </summary>
        /// <param name="currentListId">The id of the List that should have its cards moved</param>
        /// <param name="newListId">The id of the new List that should receive the cards</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task MoveAllCardsInListAsync(string currentListId, string newListId, CancellationToken cancellationToken = default)
        {
            List newList = await GetListAsync(newListId, cancellationToken); //Get the new list's BoardId so the user do not need to provide it.
            await _apiRequestController.Post($"{UrlPaths.Lists}/{currentListId}/moveAllCards", cancellationToken,
                0,
                new QueryParameter("idBoard", newList.BoardId),
                new QueryParameter("idList", newListId)
            );
        }

        /// <summary>
        /// Delete a Card (WARNING: THERE IS NO WAY GOING BACK!!!). Alternative use ArchiveCardAsync() for non-permanency
        /// </summary>
        /// <param name="cardId">The id of the Card to Delete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Cards}/{cardId}", cancellationToken, 0);
        }

        /// <summary>
        /// Get a Card by its Id
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Card</returns>
        public async Task<Card> GetCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Card>(GetUrlBuilder.GetCard(cardId), cancellationToken,
#pragma warning disable CS0618 // Type or member is obsolete
                new QueryParameter("customFieldItems", Options.IncludeCustomFieldsInCardGetMethods),
                new QueryParameter("attachments", Options.IncludeAttachmentsInCardGetMethods)
#pragma warning restore CS0618 // Type or member is obsolete
            );
        }

        /// <summary>
        /// Get a Card by its Id
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="options">Options on how and what should be included on the cards (Example only a few fields to increase performance or more nested data to avoid more API calls)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Card</returns>
        public async Task<Card> GetCardAsync(string cardId, GetCardOptions options, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Card>(GetUrlBuilder.GetCard(cardId), cancellationToken, options.GetParameters(false));
        }

        /// <summary>
        /// Get all open cards on un-archived lists on a board
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>List of Cards</returns>
        public async Task<List<Card>> GetCardsOnBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Card>>(GetUrlBuilder.GetCardsOnBoard(boardId), cancellationToken,
#pragma warning disable CS0618 // Type or member is obsolete
                new QueryParameter("customFieldItems", Options.IncludeCustomFieldsInCardGetMethods),
                new QueryParameter("attachments", Options.IncludeAttachmentsInCardGetMethods)
#pragma warning restore CS0618 // Type or member is obsolete
            );
        }

        /// <summary>
        /// Get all open cards on un-archived lists on a board
        /// </summary>
        /// <param name="boardId">Id of the Board (in its long or short version)</param>
        /// <param name="options">Options on how and what should be included on the cards (Example only a few fields to increase performance or more nested data to avoid more API calls)</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>List of Cards</returns>
        public async Task<List<Card>> GetCardsOnBoardAsync(string boardId, GetCardOptions options, CancellationToken cancellationToken = default)
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

            if (options.IncludeBoard)
            {
                if (options.CardFields != null && !options.CardFields.Fields.Contains("idBoard"))
                {
                    options.CardFields.Fields = options.CardFields.Fields.Union(new List<string>
                    {
                        "idBoard"
                    }).ToArray();
                }
            }

            List<Card> cards;
            if (options.Filter.HasValue)
            {
                cards = await _apiRequestController.Get<List<Card>>($"{GetUrlBuilder.GetCardsOnBoard(boardId)}/{options.Filter.Value.GetJsonPropertyName()}", cancellationToken, options.GetParameters(true));
            }
            else
            {
                cards = await _apiRequestController.Get<List<Card>>(GetUrlBuilder.GetCardsOnBoard(boardId), cancellationToken, options.GetParameters(true));
            }

            if (options.IncludeList)
            {
                List<List> lists = await GetListsOnBoardAsync(boardId, cancellationToken);
                foreach (Card card in cards)
                {
                    card.List = lists.FirstOrDefault(x => x.Id == card.ListId);
                }
            }

            if (options.IncludeBoard)
            {
                Board board = await GetBoardAsync(boardId, cancellationToken);
                foreach (Card card in cards)
                {
                    card.Board = board;
                }
            }

            return cards;
        }

        /// <summary>
        /// Get all open cards on a specific list
        /// </summary>
        /// <param name="listId">Id of the List</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Cards</returns>
        public async Task<List<Card>> GetCardsInListAsync(string listId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Card>>(GetUrlBuilder.GetCardsInList(listId), cancellationToken,
#pragma warning disable CS0618 // Type or member is obsolete
                new QueryParameter("customFieldItems", Options.IncludeCustomFieldsInCardGetMethods),
                new QueryParameter("attachments", Options.IncludeAttachmentsInCardGetMethods));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Get all open cards on a specific list
        /// </summary>
        /// <param name="listId">Id of the List</param>
        /// <param name="options">Options on how and what should be included on the cards (Example only a few fields to increase performance or more nested data to avoid more API calls)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of Cards</returns>
        public async Task<List<Card>> GetCardsInListAsync(string listId, GetCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options.IncludeBoard)
            {
                if (options.CardFields != null && !options.CardFields.Fields.Contains("idBoard"))
                {
                    options.CardFields.Fields = options.CardFields.Fields.Union(new List<string>
                    {
                        "idBoard"
                    }).ToArray();
                }
            }

            List<Card> cards = await _apiRequestController.Get<List<Card>>(GetUrlBuilder.GetCardsInList(listId), cancellationToken, options.GetParameters(true));
            if (options.IncludeList)
            {
                List list = await GetListAsync(listId, cancellationToken);
                foreach (Card card in cards)
                {
                    card.ListId = listId;
                    card.List = list;
                }
            }

            if (options.IncludeBoard && cards.Count > 0)
            {
                Board board = await GetBoardAsync(cards[0].BoardId, cancellationToken);
                foreach (Card card in cards)
                {
                    card.Board = board;
                }
            }


            return cards;
        }

        /// <summary>
        /// Get the Cards in your Inbox
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The Cards</returns>
        public async Task<List<Card>> GetCardsInInboxAsync(CancellationToken cancellationToken = default)
        {
            TokenMemberInbox inbox = await GetTokenMemberInboxAsync(cancellationToken);
            if (inbox == null)
            {
                throw new TrelloApiException("Could not find your inbox", string.Empty);
            }

            return await GetCardsOnBoardAsync(inbox.BoardId, cancellationToken);
        }

        /// <summary>
        /// Get the Cards in your Inbox
        /// </summary>
        /// <param name="options">Options on what parts of the cards to get</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The Cards</returns>
        public async Task<List<Card>> GetCardsInInboxAsync(GetInboxCardOptions options, CancellationToken cancellationToken = default)
        {
            TokenMemberInbox inbox = await GetTokenMemberInboxAsync(cancellationToken);
            if (inbox == null)
            {
                throw new TrelloApiException("Could not find your inbox", string.Empty);
            }

            return await GetCardsOnBoardAsync(inbox.BoardId, options.ToCardOptions(), cancellationToken);
        }

        /// <summary>
        /// Get all Cards a Member is on (across multiple boards)
        /// </summary>
        /// <param name="memberId">Id of Member</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task<List<Card>> GetCardsForMemberAsync(string memberId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Card>>(GetUrlBuilder.GetCardsForMember(memberId), cancellationToken);
        }

        /// <summary>
        /// Get all Cards a Member is on (across multiple boards)
        /// </summary>
        /// <param name="memberId">Id of Member</param>
        /// <param name="options">Options on how and what should be included on the cards (Example only a few fields to increase performance or more nested data to avoid more API calls)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task<List<Card>> GetCardsForMemberAsync(string memberId, GetCardOptions options, CancellationToken cancellationToken = default)
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

                if (options.CardFields != null && !options.CardFields.Fields.Contains("idBoard"))
                {
                    options.CardFields.Fields = options.CardFields.Fields.Union(new List<string>
                    {
                        "idBoard"
                    }).ToArray();
                }
            }

            if (options.IncludeBoard)
            {
                if (options.CardFields != null && !options.CardFields.Fields.Contains("idBoard"))
                {
                    options.CardFields.Fields = options.CardFields.Fields.Union(new List<string>
                    {
                        "idBoard"
                    }).ToArray();
                }
            }

            List<Card> cards = await _apiRequestController.Get<List<Card>>(GetUrlBuilder.GetCardsForMember(memberId), cancellationToken, options.GetParameters(true));
            if (options.IncludeList)
            {
                string[] boardsToGetListsFor = cards.Select(x => x.BoardId).Distinct().ToArray();
                List<List> lists = new List<List>();
                foreach (string boardId in boardsToGetListsFor)
                {
                    lists.AddRange(await GetListsOnBoardAsync(boardId, cancellationToken));
                }

                foreach (Card card in cards)
                {
                    card.List = lists.FirstOrDefault(x => x.Id == card.ListId);
                }
            }

            if (options.IncludeBoard)
            {
                List<string> boardIds = cards.Select(x => x.BoardId).Distinct().ToList();
                List<Board> boards = await GetBoardsAsync(boardIds, cancellationToken);
                foreach (Card card in cards)
                {
                    card.Board = boards.FirstOrDefault(x => x.Id == card.BoardId);
                }
            }

            return cards;
        }

        /// <summary>
        /// Set Due Date on a card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="dueDate">The Due Date (In UTC Time)</param>
        /// <param name="dueComplete">If Due is complete</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task<Card> SetDueDateOnCardAsync(string cardId, DateTimeOffset dueDate, bool dueComplete = false, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.DueDate(dueDate),
                CardUpdate.DueComplete(dueComplete)
            }, cancellationToken);
        }

        /// <summary>
        /// Set Due Date on a card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="startDate">The Start Date (In UTC Time)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task<Card> SetStartDateOnCardAsync(string cardId, DateTimeOffset startDate, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.StartDate(startDate)
            }, cancellationToken);
        }

        /// <summary>
        /// Set Start and Due Date on a card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="startDate">The Start Date (In UTC Time)</param>
        /// <param name="dueDate">The Due Date (In UTC Time)</param>
        /// <param name="dueComplete">If Due is complete</param>
        /// <param name="cancellationToken">Cancellation Token</param> 
        public async Task<Card> SetStartDateAndDueDateOnCardAsync(string cardId, DateTimeOffset startDate, DateTimeOffset dueDate, bool dueComplete = false, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.StartDate(startDate),
                CardUpdate.DueDate(dueDate),
                CardUpdate.DueComplete(dueComplete)
            }, cancellationToken);
        }

        /// <summary>
        /// Move a Card to a new list on the same board
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="newListId">Id of the List you wish to move it to</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task<Card> MoveCardToListAsync(string cardId, string newListId, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.List(newListId)
            }, cancellationToken);
        }

        /// <summary>
        /// Move a Card to a new list on the same board
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="newListId">Id of the List you wish to move it to</param>
        /// <param name="options">Additional optional Options for the Move</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task<Card> MoveCardToListAsync(string cardId, string newListId, MoveCardToListOptions options, CancellationToken cancellationToken = default)
        {
            List<CardUpdate> parameters = new List<CardUpdate> { CardUpdate.List(newListId) };
            if (options.NamedPositionOnNewList.HasValue)
            {
                parameters.Add(CardUpdate.Position(options.NamedPositionOnNewList.Value));
            }
            else if (options.PositionOnNewList.HasValue)
            {
                parameters.Add(CardUpdate.Position(options.PositionOnNewList.Value));
            }

            return await UpdateCardAsync(cardId, parameters, cancellationToken);
        }

        /// <summary>
        /// Move the Card to the top of its current list
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Card</returns>
        public async Task<Card> MoveCardToTopOfCurrentListAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Position(NamedPosition.Top)
            }, cancellationToken);
        }

        /// <summary>
        /// Move the Card to the bottom of its current list
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Card</returns>
        public async Task<Card> MoveCardToBottomOfCurrentListAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Position(NamedPosition.Bottom)
            }, cancellationToken);
        }

        /// <summary>
        /// Move a Card to another board
        /// </summary>
        /// <param name="cardId">The Id of the Card to Move</param>
        /// <param name="newBoardId">The ID of the New Board that the card should be moved to</param>
        /// <param name="options">Additional Options for the move like what list the card should end up on the new board and what happens to labels and members</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        [Obsolete("Use version with Async-suffix [Will be removed in v2.0 of this nuGet Package (More info: https://github.com/rwjdk/TrelloDotNet/issues/51)]")]
        public async Task<Card> MoveCardToBoard(string cardId, string newBoardId, MoveCardToBoardOptions options, CancellationToken cancellationToken = default)
        {
            return await MoveCardToBoardAsync(cardId, newBoardId, options, cancellationToken);
        }

        /// <summary>
        /// Move a Card to another board
        /// </summary>
        /// <param name="cardId">The Id of the Card to Move</param>
        /// <param name="newBoardId">The ID of the New Board that the card should be moved to</param>
        /// <param name="options">Additional Options for the move like what list the card should end up on the new board and what happens to labels and members</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task<Card> MoveCardToBoardAsync(string cardId, string newBoardId, MoveCardToBoardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "You need to pass an options object to confirm the various options that are invovled with moving a card between boards");
            }

            List<CardUpdate> parameters = new List<CardUpdate> { CardUpdate.Board(newBoardId) };
            string newListId = options.NewListId;
            if (string.IsNullOrWhiteSpace(newListId))
            {
                //No list specified, so we need to find the first list on the board
                newListId = (await GetListsOnBoardAsync(newBoardId, cancellationToken)).OrderBy(x => x.Position).FirstOrDefault()?.Id;
            }

            parameters.Add(CardUpdate.List(newListId));

            if (options.NamedPositionOnNewList.HasValue)
            {
                parameters.Add(CardUpdate.Position((options.NamedPositionOnNewList.Value)));
            }
            else if (options.PositionOnNewList.HasValue)
            {
                parameters.Add(CardUpdate.Position(options.PositionOnNewList.Value));
            }

            Card card = await GetCardAsync(cardId, new GetCardOptions
            {
                CardFields = new CardFields(CardFieldsType.MemberIds, CardFieldsType.LabelIds, CardFieldsType.Labels)
            }, cancellationToken);

            switch (options.MemberOptions)
            {
                case MoveCardToBoardOptionsMemberOptions.KeepMembersAlsoOnNewBoardAndRemoveRest:
                    IEnumerable<string> existingMemberIdsOnNewBoard = (await GetMembersOfBoardAsync(newBoardId, cancellationToken)).Select(x => x.Id);
                    card.MemberIds = card.MemberIds.Intersect(existingMemberIdsOnNewBoard).ToList();
                    break;
                case MoveCardToBoardOptionsMemberOptions.RemoveAllMembersOnCard:
                    card.MemberIds.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (card.LabelIds.Any())
            {
                card.LabelIds.Clear();
                switch (options.LabelOptions)
                {
                    case MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndColorAndCreateMissing:
                    {
                        List<Label> existingLabels = await GetLabelsOfBoardAsync(newBoardId, cancellationToken);
                        foreach (Label cardLabel in card.Labels)
                        {
                            Label existingLabel = existingLabels.FirstOrDefault(x => x.Name == cardLabel.Name && x.Color == cardLabel.Color);
                            if (existingLabel != null)
                            {
                                card.LabelIds.Add(existingLabel.Id);
                            }
                            else
                            {
                                //Label need to be added
                                Label newLabel = await AddLabelAsync(new Label(newBoardId, cardLabel.Name, cardLabel.Color), cancellationToken);
                                card.LabelIds.Add(newLabel.Id);
                            }
                        }

                        break;
                    }
                    case MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndColorAndRemoveMissing:
                    {
                        List<Label> existingLabels = await GetLabelsOfBoardAsync(newBoardId, cancellationToken);
                        foreach (Label cardLabel in card.Labels)
                        {
                            Label existingLabel = existingLabels.FirstOrDefault(x => x.Name == cardLabel.Name && x.Color == cardLabel.Color);
                            if (existingLabel != null)
                            {
                                card.LabelIds.Add(existingLabel.Id);
                            }
                        }

                        break;
                    }
                    case MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndCreateMissing:
                    {
                        List<Label> existingLabels = await GetLabelsOfBoardAsync(newBoardId, cancellationToken);
                        foreach (Label cardLabel in card.Labels)
                        {
                            Label existingLabel = existingLabels.FirstOrDefault(x => x.Name == cardLabel.Name);
                            if (existingLabel != null)
                            {
                                card.LabelIds.Add(existingLabel.Id);
                            }
                            else
                            {
                                //Label need to be added
                                Label newLabel = await AddLabelAsync(new Label(newBoardId, cardLabel.Name, cardLabel.Color), cancellationToken);
                                card.LabelIds.Add(newLabel.Id);
                            }
                        }

                        break;
                    }
                    case MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndRemoveMissing:
                    {
                        List<Label> existingLabels = await GetLabelsOfBoardAsync(newBoardId, cancellationToken);
                        foreach (Label cardLabel in card.Labels)
                        {
                            Label existingLabel = existingLabels.FirstOrDefault(x => x.Name == cardLabel.Name);
                            if (existingLabel != null)
                            {
                                card.LabelIds.Add(existingLabel.Id);
                            }
                        }

                        break;
                    }
                    case MoveCardToBoardOptionsLabelOptions.RemoveAllLabelsOnCard:
                        //No more Work needed
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            parameters.Add(CardUpdate.Labels(card.LabelIds));
            parameters.Add(CardUpdate.Members(card.MemberIds));

            if (options.RemoveDueDate)
            {
                parameters.Add(CardUpdate.DueDate(null));
            }

            if (options.RemoveStartDate)
            {
                parameters.Add(CardUpdate.StartDate(null));
            }

            return await UpdateCardAsync(cardId, parameters, cancellationToken);
        }

        private static string GeneratePayloadForCoverUpdate(CardCover cardCover, List<QueryParameter> parameters)
        {
            //Special code for Cover
            string payload = string.Empty;
            if (cardCover == null)
            {
                //Remove cover
                parameters.Add(new QueryParameter("cover", ""));
            }
            else
            {
                cardCover.PrepareForAddUpdate();
                if (cardCover.Color != null || cardCover.BackgroundImageId != null)
                {
                    QueryParameter queryParameter = parameters.FirstOrDefault(x => x.Name == "idAttachmentCover");
                    if (queryParameter != null)
                    {
                        parameters.Remove(queryParameter); //This parameter can't be there while a cover is added
                    }
                }

                payload = $"{{\"cover\":{JsonSerializer.Serialize(cardCover)}}}";
            }

            return payload;
        }
    }
}
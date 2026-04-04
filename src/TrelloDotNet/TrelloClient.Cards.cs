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
using TrelloDotNet.Model.Options.DueRecurrenceOptions;
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
        private const int MaxCardDescriptionLength = 16_384;
        private const string DescriptionQueryParameterName = "desc";

        /// <summary>
        /// Creates a new card in a specified list with the provided options.
        /// </summary>
        /// <param name="options">Options for the new card, such as name, list ID, description, dates, checklists, attachments, labels, members, custom fields, and cover. <see cref="AddCardOptions"/></param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The created <see cref="Card"/> object with all requested properties set.</returns>
        public async Task<Card> AddCardAsync(AddCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(AddCardOptions)} cannot be null");
            }

            if (string.IsNullOrWhiteSpace(options.ListId))
            {
                throw new TrelloApiException("No ListId provided in options (Mandatory)", string.Empty);
            }

            Card input = new Card
            {
                ListId = options.ListId,
                Name = options.Name,
                Description = options.Description,
                IsTemplate = options.IsTemplate,
                DueComplete = options.DueComplete
            };

            if (options.Start.HasValue)
            {
                input.Start = options.Start.Value;
            }

            if (options.Due.HasValue)
            {
                input.Due = options.Due.Value;
            }

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
                input.LabelIds = options.LabelIds.Distinct().ToList();
            }

            if (options.MemberIds != null)
            {
                input.MemberIds = options.MemberIds.Distinct().ToList();
            }

            List<QueryParameter> parameters = _queryParametersBuilder.GetViaQueryParameterAttributes(input).ToList();
            _queryParametersBuilder.AdjustForNamedPosition(parameters, input.NamedPosition);

            QueryParameter descriptionParameter = parameters.FirstOrDefault(x => x.Name == DescriptionQueryParameterName);
            EnsureDescriptionIsWithinAllowedLimit(descriptionParameter?.GetRawStringValue());

            string payload = ExtractDescriptionFromQueryStringAndBuildPayload(parameters);
            Card addedCard = payload == null
                ? await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters.ToArray())
                : await _apiRequestController.PostWithJsonPayload<Card>($"{UrlPaths.Cards}", cancellationToken, payload, parameters.ToArray());

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
        /// Creates a new card in the inbox list of the member who owns the Trello token, using the provided options.
        /// </summary>
        /// <param name="options">Options for the new inbox card, such as name, description, dates, checklists, attachments, and cover. <see cref="AddCardToInboxOptions"/></param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The created <see cref="Card"/> object in the inbox list.</returns>
        public async Task<Card> AddCardToInboxAsync(AddCardToInboxOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(AddCardToInboxOptions)} cannot be null");
            }

            TokenMemberInbox inbox = await GetTokenMemberInboxAsync(cancellationToken);
            if (inbox == null)
            {
                throw new TrelloApiException("Could not find your inbox", string.Empty);
            }

            Card input = new Card
            {
                ListId = inbox.ListId,
                Name = options.Name,
                Description = options.Description,
                LabelIds = new List<string>(),
                MemberIds = new List<string>(),
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
                input.LabelIds = options.LabelIds.Distinct().ToList();
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
        /// Creates a new card based on a template card, copying selected properties and content from the template.
        /// </summary>
        /// <param name="options">Options for creating the card from a template, including template card ID, target list, name, position, and what to keep. <see cref="AddCardFromTemplateOptions"/></param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The newly created <see cref="Card"/> based on the template.</returns>
        public async Task<Card> AddCardFromTemplateAsync(AddCardFromTemplateOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(AddCardFromTemplateOptions)} cannot be null");
            }

            string nameOnNewCard = !string.IsNullOrWhiteSpace(options.Name)
                ? options.Name
                : (await GetCardAsync(options.SourceTemplateCardId, new GetCardOptions
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
                AddCardFromTemplateOptionsToKeep keep = options.Keep.Value;

                if (keep.HasFlag(AddCardFromTemplateOptionsToKeep.All))
                {
                    keepFromSource = "all";
                }
                else
                {
                    List<string> keepStrings = new List<string>();
                    List<AddCardFromTemplateOptionsToKeep> enumValues = Enum.GetValues(typeof(AddCardFromTemplateOptionsToKeep)).Cast<AddCardFromTemplateOptionsToKeep>().ToList();
                    foreach (AddCardFromTemplateOptionsToKeep toKeep in enumValues.Where(x => x != AddCardFromTemplateOptionsToKeep.All))
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
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Name, nameOnNewCard),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.IdList, options.TargetListId),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Pos, position),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.IdCardSource, options.SourceTemplateCardId),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.KeepFromSource, keepFromSource)
            };
            return await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters);
        }

        /// <summary>
        /// Adds a separator card (with name "---" and cardRole "separator") to the specified list at the given position.
        /// </summary>
        /// <param name="listId">The ID of the list to add the separator card to.</param>
        /// <param name="position">The position in the list to add the separator card (0 for top).</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The created separator <see cref="Card"/>.</returns>
        public async Task<Card> AddSeparatorCardAsync(string listId, decimal position, CancellationToken cancellationToken = default)
        {
            if (position == 0)
            {
                return await AddSeparatorCardAsync(listId, NamedPosition.Top, cancellationToken);
            }

            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter(CardFieldsType.ListId, listId),
                new QueryParameter(CardFieldsType.Name, "---"),
                new QueryParameter(CardFieldsType.Position, position),
                new QueryParameter(CardFieldsType.CardRole, "separator")
            };
            return await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters.ToArray());
        }

        /// <summary>
        /// Adds a separator card (with name "---" and cardRole "separator") to the specified list at the given position.
        /// </summary>
        /// <param name="listId">The ID of the list to add the separator card to.</param>
        /// <param name="position">The position in the list to add the separator card.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The created separator <see cref="Card"/>.</returns>
        public async Task<Card> AddSeparatorCardAsync(string listId, NamedPosition position, CancellationToken cancellationToken = default)
        {
            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter(CardFieldsType.ListId, listId),
                new QueryParameter(CardFieldsType.Name, "---"),
                new QueryParameter(CardFieldsType.Position, position == NamedPosition.Top ? "top" : "bottom"),
                new QueryParameter(CardFieldsType.CardRole, "separator")
            };
            return await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters.ToArray());
        }

        /// <summary>
        /// Creates a copy of an existing card, with options to specify which properties to keep or override.
        /// </summary>
        /// <param name="options">Options for copying the card, including source card ID, target list, name, position, and what to keep. <see cref="CopyCardOptions"/></param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The copied <see cref="Card"/>.</returns>
        public async Task<Card> CopyCardAsync(CopyCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(CopyCardOptions)} cannot be null");
            }

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
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Name, nameOnNewCard),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.IdList, options.TargetListId),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Pos, position),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.IdCardSource, options.SourceCardId),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.KeepFromSource, keepFromSource)
            };
            return await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters);
        }

        /// <summary>
        /// Creates a mirror card in a target list, linking it to the source card for synchronization.
        /// </summary>
        /// <param name="options">Options for mirroring the card, including source card ID, target list, and position. <see cref="MirrorCardOptions"/></param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The created mirror <see cref="Card"/>.</returns>
        public async Task<Card> MirrorCardAsync(MirrorCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(MirrorCardOptions)} cannot be null");
            }

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
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.IdList, options.TargetListId),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Name, sourceCard.ShortUrl),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.IsTemplate, false),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Closed, false),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.Pos, position),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.CardRole, "mirror"),
            };

            Card result = await _apiRequestController.Post<Card>($"{UrlPaths.Cards}", cancellationToken, parameters.ToArray());
            return result;
        }

        /// <summary>
        /// Archives (closes) the specified card, removing it from the active list but not deleting it permanently.
        /// </summary>
        /// <param name="cardId">The ID of the card to archive.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The archived <see cref="Card"/>.</returns>
        public async Task<Card> ArchiveCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Card>($"{UrlPaths.Cards}/{cardId}", cancellationToken, new QueryParameter(Constants.TrelloIds.QueryParameterNames.Closed, true));
        }

        /// <summary>
        /// Reopens an archived card, making it active and visible on the board again.
        /// </summary>
        /// <param name="cardId">The ID of the card to reopen.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The reopened <see cref="Card"/>.</returns>
        public async Task<Card> ReOpenCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Put<Card>($"{UrlPaths.Cards}/{cardId}", cancellationToken, new QueryParameter(Constants.TrelloIds.QueryParameterNames.Closed, false));
        }

        /// <summary>
        /// Updates one or more specific fields on a card, such as name, description, dates, members, labels, or cover.
        /// </summary>
        /// <param name="cardId">The ID of the card to update.</param>
        /// <param name="valuesToUpdate">A list of updates to apply to the card. <see cref="CardUpdate"/></param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The updated <see cref="Card"/>.</returns>
        public async Task<Card> UpdateCardAsync(string cardId, List<CardUpdate> valuesToUpdate, CancellationToken cancellationToken = default)
        {
            List<QueryParameter> parameters = valuesToUpdate.Select(x => x.ToQueryParameter()).ToList();
            QueryParameter descriptionParameter = parameters.FirstOrDefault(x => x.Name == DescriptionQueryParameterName);
            EnsureDescriptionIsWithinAllowedLimit(descriptionParameter?.GetRawStringValue());

            QueryParameter coverParameter = parameters.FirstOrDefault(x => x.Name == "cover");
            string payload = null;
            if (coverParameter != null && !string.IsNullOrWhiteSpace(coverParameter.GetRawStringValue()))
            {
                //Special Cover Card
                parameters.Remove(coverParameter);
                CardCover cover = JsonSerializer.Deserialize<CardCover>(coverParameter.GetRawStringValue());
                payload = GeneratePayloadForCoverUpdate(cover, parameters);
            }

            string descriptionPayload = ExtractDescriptionFromQueryStringAndBuildPayload(parameters);
            if (descriptionPayload != null)
            {
                payload = MergePayload(payload, descriptionPayload);
            }

            if (payload != null)
            {
                return await _apiRequestController.PutWithJsonPayload<Card>($"{UrlPaths.Cards}/{cardId}", cancellationToken, payload, parameters.ToArray());
            }

            return await _apiRequestController.Put<Card>($"{UrlPaths.Cards}/{cardId}", cancellationToken, parameters.ToArray());
        }

        /// <summary>
        /// Archives all cards in the specified list.
        /// </summary>
        /// <param name="listId">The ID of the list whose cards should be archived.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task ArchiveAllCardsInListAsync(string listId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Post<List>($"{UrlPaths.Lists}/{listId}/archiveAllCards", cancellationToken);
        }

        /// <summary>
        /// Moves all cards from one list to another list (on same or other Board)
        /// </summary>
        /// <param name="currentListId">The ID of the list whose cards should be moved.</param>
        /// <param name="newListId">The ID of the destination list.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task MoveAllCardsInListAsync(string currentListId, string newListId, CancellationToken cancellationToken = default)
        {
            List newList = await GetListAsync(newListId, cancellationToken); //Get the new list's BoardId so the user do not need to provide it.
            await _apiRequestController.Post($"{UrlPaths.Lists}/{currentListId}/moveAllCards", cancellationToken,
                0,
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.IdBoard, newList.BoardId),
                new QueryParameter(Constants.TrelloIds.QueryParameterNames.IdList, newListId)
            );
        }

        /// <summary>
        /// Permanently deletes a card. <b>Warning: This action cannot be undone. Use <see cref="ArchiveCardAsync"/> for non-permanent removal.</b>
        /// </summary>
        /// <param name="cardId">The ID of the card to delete.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task DeleteCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            await _apiRequestController.Delete($"{UrlPaths.Cards}/{cardId}", cancellationToken, 0);
        }

        /// <summary>
        /// Retrieves a card by its ID, including all default properties.
        /// </summary>
        /// <param name="cardId">The ID of the card to retrieve.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The requested <see cref="Card"/>.</returns>
        public async Task<Card> GetCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<Card>(GetUrlBuilder.GetCard(cardId), cancellationToken);
        }

        /// <summary>
        /// Retrieves a card by its ID, with options to include or exclude specific fields and nested data.
        /// </summary>
        /// <param name="cardId">The ID of the card to retrieve.</param>
        /// <param name="options">Options specifying which fields and nested data to include. <see cref="GetCardOptions"/></param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The requested <see cref="Card"/>.</returns>
        public async Task<Card> GetCardAsync(string cardId, GetCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                return await GetCardAsync(cardId, cancellationToken);
            }

            return await _apiRequestController.Get<Card>(GetUrlBuilder.GetCard(cardId), cancellationToken, options.GetParameters(false));
        }

        /// <summary>
        /// Retrieves all open cards on all un-archived lists on a board.
        /// </summary>
        /// <param name="boardId">The ID of the board (long or short form).</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>A list of <see cref="Card"/> objects on the board.</returns>
        public async Task<List<Card>> GetCardsOnBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Card>>(GetUrlBuilder.GetCardsOnBoard(boardId), cancellationToken);
        }

        /// <summary>
        /// Retrieves all cards on a board, with options to filter, include archived cards, and specify which fields to include.
        /// </summary>
        /// <param name="boardId">The ID of the board (long or short form).</param>
        /// <param name="options">Options specifying which cards and fields to include. <see cref="GetCardOptions"/></param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>A list of <see cref="Card"/> objects on the board.</returns>
        public async Task<List<Card>> GetCardsOnBoardAsync(string boardId, GetCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                return await GetCardsOnBoardAsync(boardId, cancellationToken);
            }

            options.AdjustFieldsBasedOnSelectedOptions();
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

            string cardsSuffix = options.Filter.HasValue ? $"{GetUrlBuilder.GetCardsOnBoard(boardId)}/{options.Filter.Value.GetJsonPropertyName()}" : GetUrlBuilder.GetCardsOnBoard(boardId);
            List<Card> cards = await GetCardsNormalOrViaPagination(options, cancellationToken, cardsSuffix);

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

            cards = FilterCards(cards, options.FilterConditions);
            return OrderCards(cards, options.OrderBy);
        }

        private async Task<List<Card>> GetCardsNormalOrViaPagination(GetCardOptions options, CancellationToken cancellationToken, string cardsSuffix)
        {
            List<Card> cards = new List<Card>();
            if (options.PageSize.HasValue)
            {
                string idOfOldestCardRetrieved = string.Empty;
                int? orgLimit = options.Limit;
                string orgBefore = options.Before;
                string orgSince = options.Since;
                List<QueryParameter> orgAdditionalParameters = options.AdditionalParameters;

                try
                {
                    List<Card> pageCards;
                    options.Limit = options.PageSize.Value;
                    if (options.AdditionalParameters == null)
                    {
                        options.AdditionalParameters = new List<QueryParameter>();
                    }
                    options.AdditionalParameters.Add(new QueryParameter(Constants.TrelloIds.QueryParameterNames.Sort, "-id"));
                    do
                    {
                        options.Before = idOfOldestCardRetrieved;
                        pageCards = await _apiRequestController.Get<List<Card>>(cardsSuffix, cancellationToken, options.GetParameters(true));
                        cards.AddRange(pageCards);
                        idOfOldestCardRetrieved = pageCards.LastOrDefault()?.Id;
                    }
                    while (pageCards.Count > 0 && (orgLimit ?? int.MaxValue) > cards.Count);

                    if (orgLimit.HasValue)
                    {
                        cards = cards.Take(orgLimit.Value).ToList();
                    }
                }
                finally
                {
                    options.Limit = orgLimit;
                    options.Before = orgBefore;
                    options.Since = orgSince;
                    options.AdditionalParameters = orgAdditionalParameters;
                }
            }
            else
            {
                cards = await _apiRequestController.Get<List<Card>>(cardsSuffix, cancellationToken, options.GetParameters(true));
            }

            return cards;
        }

        /// <summary>
        /// Retrieves all open cards on a specific list.
        /// </summary>
        /// <param name="listId">The ID of the list.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A list of <see cref="Card"/> objects in the list.</returns>
        public async Task<List<Card>> GetCardsInListAsync(string listId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Card>>(GetUrlBuilder.GetCardsInList(listId), cancellationToken);
        }

        /// <summary>
        /// Retrieves all open cards on a specific list, with options to include board and list details.
        /// </summary>
        /// <param name="listId">The ID of the list.</param>
        /// <param name="options">Options specifying which fields and nested data to include. <see cref="GetCardOptions"/></param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A list of <see cref="Card"/> objects in the list.</returns>
        public async Task<List<Card>> GetCardsInListAsync(string listId, GetCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                return await GetCardsInListAsync(listId, cancellationToken);
            }

            options.AdjustFieldsBasedOnSelectedOptions();
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

            string suffix = GetUrlBuilder.GetCardsInList(listId);
            List<Card> cards = await GetCardsNormalOrViaPagination(options, cancellationToken, suffix);
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

            cards = FilterCards(cards, options.FilterConditions);
            return OrderCards(cards, options.OrderBy);
        }

        /// <summary>
        /// Retrieves all cards in the inbox of the current member.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>A list of <see cref="Card"/> objects in the inbox.</returns>
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
        /// Retrieves all cards in the inbox of the current member, with options to include specific fields and nested data.
        /// </summary>
        /// <param name="options">Options specifying which fields and nested data to include. <see cref="GetInboxCardOptions"/></param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>A list of <see cref="Card"/> objects in the inbox.</returns>
        public async Task<List<Card>> GetCardsInInboxAsync(GetInboxCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(GetInboxCardOptions)} cannot be null");
            }

            TokenMemberInbox inbox = await GetTokenMemberInboxAsync(cancellationToken);
            if (inbox == null)
            {
                throw new TrelloApiException("Could not find your inbox", string.Empty);
            }

            GetCardOptions getCardOptions = options.ToCardOptions();
            List<Card> cards = await GetCardsOnBoardAsync(inbox.BoardId, getCardOptions, cancellationToken);
            cards = FilterCards(cards, getCardOptions.FilterConditions);
            return OrderCards(cards, getCardOptions.OrderBy);
        }

        /// <summary>
        /// Retrieves all cards assigned to a member across multiple boards.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A list of <see cref="Card"/> objects assigned to the member.</returns>
        public async Task<List<Card>> GetCardsForMemberAsync(string memberId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<Card>>(GetUrlBuilder.GetCardsForMember(memberId), cancellationToken);
        }

        /// <summary>
        /// Retrieves all cards assigned to a member across multiple boards, with options to include specific fields and nested data.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <param name="options">Options specifying which fields and nested data to include. <see cref="GetCardOptions"/></param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A list of <see cref="Card"/> objects assigned to the member.</returns>
        public async Task<List<Card>> GetCardsForMemberAsync(string memberId, GetCardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                return await GetCardsForMemberAsync(memberId, cancellationToken);
            }

            options.AdjustFieldsBasedOnSelectedOptions();
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

            string suffix = GetUrlBuilder.GetCardsForMember(memberId);
            List<Card> cards = await GetCardsNormalOrViaPagination(options, cancellationToken, suffix);

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

            cards = FilterCards(cards, options.FilterConditions);
            return OrderCards(cards, options.OrderBy);
        }

        /// <summary>
        /// Sets the due date and completion status on a card.
        /// </summary>
        /// <param name="cardId">The ID of the card.</param>
        /// <param name="dueDate">The due date to set (in UTC).</param>
        /// <param name="dueComplete">Whether the card is complete.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated <see cref="Card"/>.</returns>
        public async Task<Card> SetDueDateOnCardAsync(string cardId, DateTimeOffset dueDate, bool dueComplete = false, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.DueDate(dueDate),
                CardUpdate.DueComplete(dueComplete)
            }, cancellationToken);
        }

        /// <summary>
        /// Sets the start date on a card.
        /// </summary>
        /// <param name="cardId">The ID of the card.</param>
        /// <param name="startDate">The start date to set (in UTC).</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated <see cref="Card"/>.</returns>
        public async Task<Card> SetStartDateOnCardAsync(string cardId, DateTimeOffset startDate, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.StartDate(startDate)
            }, cancellationToken);
        }

        /// <summary>
        /// Sets both the start and due dates, and completion status, on a card.
        /// </summary>
        /// <param name="cardId">The ID of the card.</param>
        /// <param name="startDate">The start date to set (in UTC).</param>
        /// <param name="dueDate">The due date to set (in UTC).</param>
        /// <param name="dueComplete">Whether the card is complete.</param>
        /// <param name="cancellationToken">Cancellation Token</param> 
        /// <returns>The updated <see cref="Card"/>.</returns>
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
        /// Moves a card to a new list on the same board.
        /// </summary>
        /// <param name="cardId">The ID of the card to move.</param>
        /// <param name="newListId">The ID of the destination list.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated <see cref="Card"/> in the new list.</returns>
        public async Task<Card> MoveCardToListAsync(string cardId, string newListId, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.List(newListId)
            }, cancellationToken);
        }

        /// <summary>
        /// Moves a card to a new list on the same board, with additional options for position.
        /// </summary>
        /// <param name="cardId">The ID of the card to move.</param>
        /// <param name="newListId">The ID of the destination list.</param>
        /// <param name="options">Options for the move, such as position in the new list. <see cref="MoveCardToListOptions"/></param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated <see cref="Card"/> in the new list.</returns>
        public async Task<Card> MoveCardToListAsync(string cardId, string newListId, MoveCardToListOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(MoveCardToListOptions)} cannot be null");
            }

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
        /// Moves a card to the top of its current list.
        /// </summary>
        /// <param name="cardId">The ID of the card to move.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated <see cref="Card"/> at the top of the list.</returns>
        public async Task<Card> MoveCardToTopOfCurrentListAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Position(NamedPosition.Top)
            }, cancellationToken);
        }

        /// <summary>
        /// Moves a card to the bottom of its current list.
        /// </summary>
        /// <param name="cardId">The ID of the card to move.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated <see cref="Card"/> at the bottom of the list.</returns>
        public async Task<Card> MoveCardToBottomOfCurrentListAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await UpdateCardAsync(cardId, new List<CardUpdate>
            {
                CardUpdate.Position(NamedPosition.Bottom)
            }, cancellationToken);
        }

        /// <summary>
        /// Moves a card to another board, with options for list, members, labels, and removal of due/start dates.
        /// </summary>
        /// <param name="cardId">The ID of the card to move.</param>
        /// <param name="newBoardId">The ID of the destination board.</param>
        /// <param name="options">Additional Options for the move like what list the card should end up on the new board and what happens to labels and members</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The updated <see cref="Card"/> on the new board.</returns>
        public async Task<Card> MoveCardToBoardAsync(string cardId, string newBoardId, MoveCardToBoardOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "You need to pass an options object to confirm the various options that are involved with moving a card between boards");
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

            parameters.Add(CardUpdate.Labels(card.LabelIds.Distinct().ToList()));
            parameters.Add(CardUpdate.Members(card.MemberIds.Distinct().ToList()));

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
                parameters.Add(new QueryParameter(Constants.TrelloIds.QueryParameterNames.Cover, ""));
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

        private static void EnsureDescriptionIsWithinAllowedLimit(string description)
        {
            if (description == null)
            {
                return;
            }

            if (description.Length > MaxCardDescriptionLength)
            {
                throw new TrelloApiException($"Description is too long. Maximum allowed length is {MaxCardDescriptionLength} characters.", string.Empty);
            }
        }

        private string ExtractDescriptionFromQueryStringAndBuildPayload(List<QueryParameter> parameters)
        {
            QueryParameter descriptionParameter = parameters.FirstOrDefault(x => x.Name == DescriptionQueryParameterName);
            if (descriptionParameter == null)
            {
                return null;
            }

            parameters.Remove(descriptionParameter);
            string description = descriptionParameter.GetRawStringValue() ?? string.Empty;
            return $"{{\"{DescriptionQueryParameterName}\":{JsonSerializer.Serialize(description)}}}";
        }

        private static string MergePayload(string firstPayload, string secondPayload)
        {
            if (string.IsNullOrWhiteSpace(firstPayload))
            {
                return secondPayload;
            }

            if (string.IsNullOrWhiteSpace(secondPayload))
            {
                return firstPayload;
            }

            string firstPayloadTrimmed = firstPayload.Trim();
            string secondPayloadTrimmed = secondPayload.Trim();
            if (!firstPayloadTrimmed.EndsWith("}", StringComparison.Ordinal) || !secondPayloadTrimmed.StartsWith("{", StringComparison.Ordinal))
            {
                throw new TrelloApiException("Could not merge card update payload", string.Empty);
            }

            return $"{firstPayloadTrimmed.Substring(0, firstPayloadTrimmed.Length - 1)},{secondPayloadTrimmed.Substring(1)}";
        }

        /// <summary>
        /// Set a recurring Due Date
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="options">Options for recurrence</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task SetRecurringDueDateAsync(string cardId, DueRecurrenceOptions options, CancellationToken cancellationToken = default)
        {
            DateTimeOffset recurrenceStart;
            if (options.FirstDueDate.HasValue)
            {
                recurrenceStart = options.FirstDueDate.Value;
                await SetDueDateOnCardAsync(cardId, recurrenceStart, cancellationToken: cancellationToken);
            }
            else
            {
                Card card = await GetCardAsync(cardId, new GetCardOptions
                {
                    CardFields = new CardFields(CardFieldsType.Due)
                }, cancellationToken);

                if (card.Due.HasValue)
                {
                    recurrenceStart = card.Due.Value;
                }
                else
                {
                    recurrenceStart = DateTimeOffset.UtcNow;
                    await SetDueDateOnCardAsync(cardId, recurrenceStart, cancellationToken: cancellationToken);
                }
            }
            string startAsString = recurrenceStart.ToString("yyyyMMddTHHmmssZ");
            string suffix = $"{UrlPaths.Cards}/{cardId}/recurrenceRule";
            string json;
            switch (options.Recurrence)
            {
                case DueRecurrenceType.Daily:
                    json = $"{{\"rule\":\"DTSTART:{startAsString}\\nRRULE:FREQ=DAILY\",\"triggers\":[{{\"trigger\":\"dueComplete\",\"changes\":[\"dueDateChange\"]}}]}}";
                    break;
                case DueRecurrenceType.Weekdays:
                case DueRecurrenceType.Weekly:
                    string days = options.GetDays();
                    json = $"{{\"rule\":\"DTSTART:{startAsString}\\nRRULE:FREQ=WEEKLY;BYDAY={days}\",\"triggers\":[{{\"trigger\":\"dueComplete\",\"changes\":[\"dueDateChange\"]}}]}}";
                    break;
                case DueRecurrenceType.DayOfMonth:
                    json = $"{{\"rule\":\"DTSTART:{startAsString}\\nRRULE:FREQ=MONTHLY;BYMONTHDAY={recurrenceStart.Day}\",\"triggers\":[{{\"trigger\":\"dueComplete\",\"changes\":[\"dueDateChange\"]}}]}}";
                    break;
                case DueRecurrenceType.XthDayOfWeekOfTheMonth:
                    string day;
                    switch (recurrenceStart.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            day = "FR";
                            break;
                        case DayOfWeek.Monday:
                            day = "MO";
                            break;
                        case DayOfWeek.Saturday:
                            day = "SA";
                            break;
                        case DayOfWeek.Sunday:
                            day = "SU";
                            break;
                        case DayOfWeek.Thursday:
                            day = "TH";
                            break;
                        case DayOfWeek.Tuesday:
                            day = "TU";
                            break;
                        case DayOfWeek.Wednesday:
                            day = "WE";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    json = $"{{\"rule\":\"DTSTART;TZID={options.TimezoneId}:{startAsString}\\nRRULE:FREQ=MONTHLY;BYDAY={day};BYSETPOS={Convert.ToInt32(options.XthPosition)}\",\"triggers\":[{{\"trigger\":\"dueComplete\",\"changes\":[\"dueDateChange\"]}}]}}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await _apiRequestController.PostWithJsonPayloadToNonApi(suffix, cancellationToken, json, 0);
        }

        /// <summary>
        /// Remove recurrence of the due date (Note: This does not remove the current due date)
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task RemoveRecurringDueDateAsync(string cardId, CancellationToken cancellationToken = default)
        {
            string suffix = $"{UrlPaths.Cards}/{cardId}/recurrenceRule";
            await _apiRequestController.DeleteToNonApi(suffix, cancellationToken, 0);
        }
    }
}




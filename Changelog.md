# Changelog

## 2.8.2 (22nd of August 2026)
#### TrelloClient
- Added `GetAvailableEmojiAsync` to retrieve the Emoji available in Trello
- Special Note: Removed Trello Inbox Tests and make Inbox Methods as Obsolete as it seems like Atlassian have stopped supporting API access to the Inbox (or it is a bug) 
 
---

## 2.8.1 (17th of August 2026)
#### TrelloClient
- Fixed `DownloadAttachmentAsync` sending Trello credentials to untrusted attachment URLs
- Added more Tests

---

## 2.8.0 (15th of April 2026)
#### TrelloClient
- Added WebhookActionTypes.MoveInboxCardToBoard + WekhookReceiverEvent to support the event of a user move a card from their Inbox into a Board.

---

## 2.7.0 (4th of April 2026)
#### TrelloClient
- `TokenMemberInbox` now include the Inbox Specific Labels
- `AddCardToInboxAsync` now support Labels

---

## 2.6.0 (30th of March 2026)
#### TrelloClient
- Add option to crate, update and delete Custom Fields (`AddCustomFieldAsync`, `UpdateCustomFieldAsync`, `DeleteCustomFieldAsync`, `AddCustomFieldOptionAsync`, `DeleteCustomFieldOptionAsync`, `UpdateCustomFieldOptionAsync`)

---

## 2.5.0 (26th of March 2026)
#### TrelloClient
- Changed card descriptions on add/update requests to always be sent in the JSON body instead of only when the query string became too long

---

## 2.4.0 (24th of March 2026)
#### TrelloClient
- Added support for the new Recurring Due System (Warning: This seems to not be part of the official API (do not use the api.trello.com endpoint) but can still be set, but Atlassian might not promise backward compatibility / that this endpoint stay available to us)
  - `SetRecurringDueDateAsync` + `RemoveRecurringDueDateAsync`

---

## 2.3.0 (3rd of March 2026)
#### TrelloClient
- Added option to send the API Key and Token using Header instead of the QueryString (The `SendCredentialsMode` on `TrelloClientOptions` when creating the `TrelloClient`)
- Added HttpStatusCode to TrelloAPIExceptions to be able to see the underlying REST-API StatusCode
- Added Better Error Message if a Batch Request fail due to a TooManyRequests Rate Limiting
- Added Support for Adding/Updating Cards with Descriptions bigger than 2000 chars. Now the max is 16384 chars (The Trello upper limit)
- Added TrelloIds constants for the various technical fields

---

## 2.2.0 (3rd of February 2026)
#### TrelloClient
- Added option to set page size when getting multiple cards, allowing you to do pagination (to get around 'API_TOO_MANY_CARDS_REQUESTED' errors)
- Added `HasMember(string memberId)` and `HasMember(Member member)` on `Card` to make it easier to check if a specific Member is on the Card
- Added `HasLabel(string labelId)` and `HasLabel(Label label)` on `Card` to make it easier to check if a specific Label is on the Card

---

## 2.1.3 (28th of January 2026)
#### TrelloClient
- Allowed the various GetXyzOptions to be null (not resulting in null-ref exception but instead use method without options)
- Fix a few errors in the README.md code samples (thanks @AldeRoberge 👍)
- Added AGENTS.md for AI Agent Rules

---

## 2.1.2 (13th of October 2025)
#### TrelloClient
- Fixed that [GetTokenMemberInboxAsync](https://github.com/rwjdk/TrelloDotNet/wiki/GetTokenMemberInboxAsync) returned null as OrganizationId (due to breaking change from the Trello Team; they renamed the technical field)

---

## 2.1.1 (9th of July 2025)
#### Special
- Added [TrelloDotNet MCP Server](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloDotNet-MCP-Server) to let AI assist you and Vibe Code the whole thing 8-)

---

## 2.1.0 (6th of July 2025)
#### TrelloClient
- Added [AddSeparatorCardAsync](https://github.com/rwjdk/TrelloDotNet/wiki/AddSeparatorCardAsync) to create Separator Cards

---

## 2.0.9 (26th of June 2025)
#### TrelloClient
- Reversed experiment done in 2.0.8 as it did not resolve anything and meanwhile Trello have fixed their [202 - Accepted Bug](https://support.atlassian.com/requests/TRELLO-218085)

---

## 2.0.8 (24th of June 2025)
#### TrelloClient
- Added Experimental Support for Trello REST API now returning Status Code 202 Accepted sometimes instead of 200 OK

---

## 2.0.7 (24th of June 2025)
#### TrelloClient
- Added HTTP Error Code to end of TrelloApiExceptions messages to better understand error from the REST-API
- Detecting Http Error 429 - Too Many Requests as a reason to retry should error message not contain the normal error messages

---

## 2.0.6 (17th of June 2025)
#### Special
- Introduced copilot-instructions.md for AI Rules

#### TrelloClient
- Obsoleted `UpdateBoardAsync(Board boardWithChanges)` and introduced `UpdateBoardAsync(string boardId, List<BoardUpdate> valuesToUpdate)` that does delta updates similar to `UpdateCard`
- Obsoleted `UpdateListAsync(List listWithChanges)` and introduced `UpdateListAsync(string listId, List<ListUpdate> valuesToUpdate)` that does delta updates similar to `UpdateCard`
- Added `ListFields` in the `GetListOptions`
- Add option to see/update `Subscribed` (Watch) on Boards for the Token owner
- Updated various XML summaries
- Added guards on various options methods to check for null
- Added guard on InviteMemberToBoardViaEmailAsync to explain it is not possible to invite a user as Admin

---

## 2.0.5 (9th of April 2025)
#### TrelloClient
- Fixed the logic around setting `Start` and `DueDate` on Cards to correctly account for timezones

---

## 2.0.4 (1st of April 2025)
#### TrelloClient
- Added auto-fix for scenarios where duplicate LabelIds or MemberIds are provided to a Card in Add/Update operations (since the Trello REST API no longer supports duplicates, but instead throws error 'label not found')

---

## 2.0.3 (18th of March 2025)
#### Webhook Receiver
- Added missing `Text` property to `WebhookActionData` (Thanks to **[Liamth99](https://github.com/Liamth99)** for the contribution 💪) - [PR#57](https://github.com/rwjdk/TrelloDotNet/pull/57)

---

## 2.0.2 (12th of March 2025)
#### TrelloClient
- Fixed that `BoardPreferenceWhoCanAddAndRemoveMembers` `Admins` option was incorrectly named `Observers` [BREAKING CHANGE]

---

## 2.0.1 (11th of March 2025)
#### TrelloClient
- Added overload for [GetCurrentTokenMembershipsAsync](https://github.com/rwjdk/TrelloDotNet/wiki/GetCurrentTokenMembershipsAsync) where you can provide Board and `OrganizationOptions`

---

## 2.0.0 (5th of March 2025)
#### Special
- **This is the first v2.x release. It has a set of breaking changes to streamline the API and make it less confusing to use (fewer options to do the same thing). [See a list of breaking changes here](https://github.com/rwjdk/TrelloDotNet/issues/51)**
- Streamlined texts in ReadMe, Changelog, and Description of NuGet Package
- Added .editorconfig to help with naming of public methods (all receive the Async suffix)
- Significantly increased test coverage percentage

#### TrelloClient
- Added `FilterCondition` to `GetCardsOptions` that allows you to filter the cards returned in various ways (Example: _give me all cards on board that have the Red Label, 1-2 Members and the Description contains the word 'Urgent'_). NB: The filter is an in-memory filter as Trello does not allow server-side filtering. See more about the Filter Condition System [here](https://github.com/rwjdk/TrelloDotNet/wiki/Filter-Condition-System)
- Added options for `GetListAsync`, `GetListsAsync`, and `GetListsOnBoardAsync` to include the Board and the Cards on the list(s)
- Added `IncludeOrganization` to `GetBoardOptions`
- Added `GetOrganizationOptions` to the various GetOrganization methods
- Added option to see the Board Preferences of the `Board`
- Added [UpdateBoardPreferencesAsync](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateBoardPreferencesAsync) to update the various preferences of the board
- Added [GetCurrentTokenMembershipsAsync](https://github.com/rwjdk/TrelloDotNet/wiki/GetCurrentTokenMembershipsAsync) to more easily determine if the token-user is Admin on a board
- Added [GetPluginsOnBoardAsync](https://github.com/rwjdk/TrelloDotNet/wiki/GetPluginsOnBoardAsync)
- Added [GetPluginDataOnCardAsync\<T\>(cardId, pluginId)](https://github.com/rwjdk/TrelloDotNet/wiki/GetPluginDataOnCardAsync)
- Added [GetPluginDataOfBoardAsync\<T\>(boardId, pluginId)](https://github.com/rwjdk/TrelloDotNet/wiki/GetPluginDataOfBoardAsync)
- Added Extension Methods for `PlugInData` to cast their values to a Specific Model
- Fixed that a Custom Field of type Date could not be read if it included milliseconds
- Introduced an "Unknown" value for all enum-based values returned from the API to ensure that Trello can introduce new valid values without breaking this API (will revert to this Unknown value if value-parsing fails) [NB: You should never send this value to Add/Update methods as it will result in a failure]

---

## 1.11.13 (4th of March 2025)
#### TrelloClient
- Fixed that [Search](https://github.com/rwjdk/TrelloDotNet/wiki/Search) failed deserializing JSON

---

## 1.11.12 (2nd of March 2025)
- Obsoleted `GetListsOnBoardFilteredAsync`. Use GetListsOnBoardAsync with GetListOptions.Filter instead.

---

## 1.11.11 (21st of February 2025)
#### TrelloClient
- Marked incorrectly named methods as Obsolete and offered new versions with the correct names

---

## 1.11.10 (27th of January 2025)
#### TrelloClient
- Fixed that the new AddCard variant did not properly work with NamePosition

---

## Version 1.11.9 (25th of January 2025)
#### Special
- **This is the last planned 1.x release. Upcoming 2.x releases will have a set of breaking changes to streamline the API and make it less confusing to use (fewer options to do the same thing). [See a list of breaking changes here](https://github.com/rwjdk/TrelloDotNet/issues/51)**

#### TrelloClient
- Added a new version of [AddCardAsync](https://github.com/rwjdk/TrelloDotNet/wiki/AddCardAsync) that allows all features on a card to be set in a single command and obsoleted the old version.
- Added methods to interact with your Trello Inbox ([AddCardToInboxAsync](https://github.com/rwjdk/TrelloDotNet/wiki/AddCardToInboxAsync) + [GetCardsInInboxAsync](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsInInboxAsync))
- Added [GetTokenMemberInboxAsync](https://github.com/rwjdk/TrelloDotNet/wiki/GetTokenMemberInboxAsync) to get the IDs of the owner of the Token's Inbox
- Added `CardUpdate.Cover` to allow Cover updates and `CardUpdate.AdditionalParameter` to allow any additional fields that are not a known field (should something new come along)
- Change: `CardUpdate.DueDate` and `CardUpdate.StartDate` have been changed to be nullable in order to allow the removal of values
- Obsoleted `UpdateCardAsync(Card cardWithChanges...)` as it has performance issues and can cause data to be overwritten. Use one of the two other variants to update your cards as they are faster and safer.
- Obsoleted `UpdateCardAsync(string cardId, List<QueryParameter> parameters...)`. Use `UpdateCardAsync(string cardId, List<CardUpdate> valuesToUpdate)` version instead.
- Obsoleted `AddCardTemplateAsync()` (Use `AddCardAsync()` instead and in options set `IsTemplate = true`)
- Obsoleted `GetCardsOnBoardFilteredAsync()` (Use `GetCardsOnBoardAsync()` with `options.Filter = <your Filter>`)

<hr/>

## Version 1.11.8 (19th of January 2025)
#### TrelloClient
- Added [MirrorCardAsync](https://github.com/rwjdk/TrelloDotNet/wiki/MirrorCardAsync) to support the new Card Mirroring System
- Added [AddCardTemplateAsync](https://github.com/rwjdk/TrelloDotNet/wiki/AddCardTemplateAsync) to enable Template Card Creation
- Added [AddCardFromTemplateAsync](https://github.com/rwjdk/TrelloDotNet/wiki/AddCardFromTemplateAsync) to enable making new cards from templates
- Added [CopyCardAsync](https://github.com/rwjdk/TrelloDotNet/wiki/CopyCardAsync)
- Added properties `CardRole`, `MirrorSourceId`, `IsCardMirror` and `IsTemplate` to `Card` objects

<hr/>

## Version 1.11.7 (10th of January 2025)
#### TrelloClient
- Fixed that [GetCardsOnBoardFilteredAsync](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsOnBoardFilteredAsync) would not return Lists for Archived Cards that were on an Archived List but instead returned null - [Issue #47](https://github.com/rwjdk/TrelloDotNet/issues/47)

<hr/>

## Version 1.11.6 (5th of January 2025)
#### TrelloClient
- Added [DeleteListAsync](https://github.com/rwjdk/TrelloDotNet/wiki/DeleteListAsync) (Thanks to **[cmoski](https://github.com/cmoski)** for mentioning that this is even possible as I did not know that) - [Issue #46](https://github.com/rwjdk/TrelloDotNet/issues/46)

<hr/>

## Version 1.11.5 (29th of November 2024)
#### TrelloClient
- Fixed that [AddChecklistItemAsync](https://github.com/rwjdk/TrelloDotNet/wiki/AddChecklistItemAsync) did not use the NamedPosition property

<hr/>

## Version 1.11.4 (25th of November 2024)
#### TrelloClient
- Added [AddChecklistItemAsync](https://github.com/rwjdk/TrelloDotNet/wiki/AddChecklistItemAsync)

<hr/>

## Version 1.11.3 (3rd of November 2024)
#### TrelloClient
- Added [GetAttachmentOnCardAsync](https://github.com/rwjdk/TrelloDotNet/wiki/GetAttachmentOnCardAsync)
- Added [DownloadAttachmentAsync](https://github.com/rwjdk/TrelloDotNet/wiki/DownloadAttachmentAsync) [Via IDs or URL]

<hr/>

## Version 1.11.2 (9th of October 2024)
#### TrelloClient
- `GetBoardOptions` now has filter options (All, Open, Closed, or Starred boards)
- Added [GetTrelloPlanInformationForOrganization](https://github.com/rwjdk/TrelloDotNet/wiki/GetTrelloPlanInformationForOrganization) and [GetTrelloPlanInformationForBoard](https://github.com/rwjdk/TrelloDotNet/wiki/GetTrelloPlanInformationForBoard) to get information about what features the Workspace/Board supports
- Bumped System.Text.Json dependency to version 8.0.5 due to [Security Vulnerability CVE-2024-43485](https://github.com/rwjdk/TrelloDotNet/security/dependabot/3) in previous version

<hr/>

## Version 1.11.1 (5th of October 2024)
#### Special
- Started long-term preparation for v2.0 by obsoleting various things

#### TrelloClient
- Added overload [GetLabelsOfBoardAsync](https://github.com/rwjdk/TrelloDotNet/wiki/GetLabelsOfBoardAsync) that lets you specify `GetLabelOptions` in order to control fields returned and how many labels are returned (Default: 50, Max: 1000)
- All Get`<object>`Options now have an `AdditionalParameters` property where you can inject additional QueryParameters if the out-of-the-box framework does not support it
- Added `Limit`, and `Before`/`Since` (pagination options) to `GetCardOptions`

<hr/>

## Version 1.11.0 (17th of September 2024)
#### TrelloClient
- Added options on `CustomFieldItemValue` to get the raw Custom Field Values as String/DateTimeOffset?/Decimal?/Bool (so you do not need to parse them yourself)

<hr/>

## Version 1.10.9 (17th of September 2024)
#### TrelloClient
- Added option to add/update `Color` on `List` (Only paid Trello plans support this feature)
- Added overload of [UpdateCard](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateCard) that is more simple and intuitive to use (no need to know magic strings) ([Issue #35](https://github.com/rwjdk/TrelloDotNet/issues/35))

<hr/>

## Version 1.10.8 (3rd of September 2024)
#### TrelloClient
- Added `LastLogin` and `LastActivity` to `Member` object
<hr/>

## Version 1.10.7 (28th of August 2024)
#### Automation Engine
- Added optional `SubType` to [UpdateCardTrigger](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateCardTrigger)

<hr/>

## Version 1.10.6 (30th of July 2024)
#### General
- Added Source link to NuGet Package (so you can debug source code directly)

<hr/>

## Version 1.10.5 (10th of July 2024)
#### General
- Bumped System.Text.Json dependency to version 8.0.4 due to [Security Vulnerability CVE-2024-30105](https://github.com/rwjdk/TrelloDotNet/security/dependabot/1) in previous version

<hr/>

## Version 1.10.4 (22nd of June 2024)
#### TrelloClient
- Added pagination options `Page`, `Before` and `Since` to [`GetActionsOfBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetActionsOfBoardAsync) (Thanks to **[mashbrno](https://github.com/mashbrno)** for the contribution 💪) - [PR#37](https://github.com/rwjdk/TrelloDotNet/pull/37)

<hr/>

## Version 1.10.3 (29th of May 2024)
#### TrelloClient
- Added options `TypeOfBoardsToInclude` and `IncludeLists` to `GetBoardOptions`

<hr/>

## Version 1.10.2 (22nd of May 2024)
#### TrelloClient
- Using GetCardOptions on methods [`GetCardsOnBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsOnBoardAsync), [`GetCardsInListAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/) and [`GetCardsForMember`](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsForMember) now correctly gets the Board if `IncludeBoard` option is used

<hr/>

## Version 1.10.1 (15th of May 2024)
#### Automation Engine
- Exposed Text as TrelloAction old value

<hr/>

## Version 1.10.0 (30th of April 2024)
#### TrelloClient
- Fixed [Issue #30](https://github.com/rwjdk/TrelloDotNet/issues/30) where providing a Cover to [`AddCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/AddCardAsync) would not add the cover

---

## Version 1.9.9 (16th of February 2024)
#### TrelloClient
- Methods [`GetCardsOnBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsOnBoardAsync), [`GetCardsOnBoardFilteredAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsOnBoardFilteredAsync), [`GetCardsInListAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsInListAsync), [`GetCardsForMemberAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsForMemberAsync) will now correctly include the `List` a card is on if specified in the `GetCardOptions`
- Fixed that member type 'ghost' was not supported

---

## Version 1.9.8 (21st of January 2024)
#### TrelloClient
- Added option to get Label Colors as Enum (`LabelColor`) value and `ColorInfo` that explains the color of the labels in RGB and #Hex value

---

## 1.9.7 (23rd of December 2023)
#### TrelloClient
- Added a set of handy Checklist extensions on single and collection of checklists (`GetNumberOfItems`, `GetNumberOfCompletedItems`, `GetNumberOfIncompleteItems`, `IsAllComplete`, `IsAnyIncomplete`)
- Added advanced version of [`MoveCardToListAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/MoveCardToListAsync) that accepts additional options for the move (Position and NamedPosition)
- Added [`MoveCardToBoard`](https://github.com/rwjdk/TrelloDotNet/wiki/MoveCardToBoard)
- Added `GetMemberOption` overloads to the various member-get methods
- Added member properties `Email` and `MemberType`
- Added properties on Member for the various Avatar URLs (30x30, 50x50, 170x170 pixels and the original image)

---

## Version 1.9.6 (22nd of December 2023)
#### General
- Improved NuGet description and tags

---

## Version 1.9.5 (13th of November 2023)
#### TrelloClient
- Added option to Add/Update with named positions (Top or Bottom) on `Cards`, `Lists`, `Attachments`, `Checklists`, and `Checklist Items`
- Added [`UpdateChecklistAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateChecklistAsync)

---

## Version 1.9.4 (1st of November 2023)
#### TrelloClient
- Added option to get `Stickers` on a card via `GetCardOptions`
- Added option to get `CustomFieldItems` on a card via `GetCardOptions`
- Added option to get `MemberVotes` on a card via `GetCardOptions`
- Added [`AddVoteToCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/AddVoteToCardAsync)
- Added [`RemoveVoteFromCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/RemoveVoteFromCardAsync)
- Added [`GetMembersWhoVotedOnCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetMembersWhoVotedOnCardAsync)

---

## Version 1.9.3 (26th of October 2023)
#### TrelloClient
- Added option to get `PluginData` on a card via `GetCardOptions`
- Added option to get `PluginData` of a board via `GetBoardOptions`
- Added [`GetPluginDataOnCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetPluginDataOnCardAsync)
- Added [`GetPluginDataOfBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetPluginDataOfBoardAsync)

---

## Version 1.9.2 (12th of October 2023)
#### General
- All internal usage of `UpdateCardAsync` now uses the new 'partial update' variant described below for better performance and more secure async usage in the Automation Engine

#### TrelloClient
- Added overload to [`UpdateCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateCardAsync) that instead of a full update with a card can do partial updates with only the parameters you provide

---

## Version 1.9.1 (11th of October 2023)
#### Automation Engine
- Added Automation Trigger [`AddChecklistToCardTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/AddChecklistToCardTrigger)
- Added Automation Trigger [`CreateCheckItemTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/CreateCheckItemTrigger)
- Added Automation Trigger [`DeleteCheckItemTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/DeleteCheckItemTrigger)
- Added Automation Trigger [`RemoveChecklistFromCardTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/RemoveChecklistFromCardTrigger)
- Added Automation Trigger [`UpdateCheckItemTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateCheckItemTrigger)

---

## Version 1.9.0 (3rd of October 2023)
#### General
- Tweaked the format of the README
- Tweaked the description, release notes, and tags of the NuGet Package
- The changelog now has sub-sections "General", "TrelloClient", "Automation Engine" and "Webhook Receiver" to better allow you to focus on the parts you use of the package

#### TrelloClient
- Added GetBoardOptions to all get-methods that return Boards. This allows more advanced control of what should be included on the Board (for example, only a few fields to increase performance or more nested data to avoid more API calls).
- Added `IncludeChecklists` and `ChecklistFields` to GetCardOptions
- Added more properties on the `Card` object
- Added more properties on the `Board` object
- Added batch get operations: [`ExecuteBatchedRequestAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/ExecuteBatchedRequestAsync), [`GetListsAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetListsAsync), [`GetCardsAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsAsync), [`GetBoardsAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetBoardsAsync), [`GetMembersAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetMembersAsync), [`GetOrganizationsAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetOrganizationsAsync) (more work but better performance)
- Added [`GetUrlBuilder`](https://github.com/rwjdk/TrelloDotNet/wiki/GetUrlBuilder) to make it easier to build Trello REST API URLs (for batch and generic requests)
- Fields in `GetCardOptions` and `GetBoardOptions` can now alternatively be set via enums instead of strings
- Fixed that you could not make a custom request if the suffix contained a ? (i.e., you used some of the optional API features)

#### Automation Engine
- Added option to use [Webhook Signature Validation](https://developer.atlassian.com/cloud/trello/guides/rest-api/webhooks/#webhook-signatures) (Thanks to **[compujuckel](https://github.com/compujuckel)** for the contribution 💪) - [PR#26](https://github.com/rwjdk/TrelloDotNet/pull/26)
- Added Automation Trigger [`CardNameUpdatedTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/CardNameUpdatedTrigger)
- Added Automation Trigger [`ConvertToCardFromCheckItemTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/ConvertToCardFromCheckItemTrigger)
- Added generic Automation [`Trigger`](https://github.com/rwjdk/TrelloDotNet/wiki/GenericTrigger), [`Condition`](https://github.com/rwjdk/TrelloDotNet/wiki/GenericCondition) and [`Action`](https://github.com/rwjdk/TrelloDotNet/wiki/GenericAction) that can use Func instead of needing to make custom implementations
- Automation Engine can now make automations with multiple triggers (for example, do something when a 'Card is Created' OR 'Card is emailed')
- Performance optimized various `AutomationEngine` conditions and one of the actions by internally using the GetCardOptions system to only retrieve the absolute minimum needed data
- `AddChecklistToCardIfLabelMatchAction` now supports both include and exclude matching (example: If this label is present and this is not, then add checklist)
- Added that Automation Action [`AddChecklistToCardAction`](https://github.com/rwjdk/TrelloDotNet/wiki/AddChecklistToCardAction) can use `**ID**` and `**NAME**` in the name of checklist and checklist items to on the fly get them to replace by card's name and id

#### Webhook Receiver
- Added option to use [Webhook Signature Validation](https://developer.atlassian.com/cloud/trello/guides/rest-api/webhooks/#webhook-signatures) (Thanks to **[compujuckel](https://github.com/compujuckel)** for the contribution 💪) - [PR#26](https://github.com/rwjdk/TrelloDotNet/pull/26)

---

## Version 1.8.0 (27th of August 2023)
#### TrelloClient
- Added [`SearchAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/SearchAsync)
- Added [`SearchMembersAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/SearchMembersAsync)
- Added [`GetCommentReactions`](https://github.com/rwjdk/TrelloDotNet/wiki/GetCommentReactions)
- Added GetCardOptions to all get-methods that return Cards. This allows more advanced control of what should be included on the cards (for example, only a few fields to increase performance or more nested data to avoid more API calls).
- Added more properties on the `Member` object
- Added more properties on the `Card` object

---

## Version 1.7.2 (23rd of August 2023)
#### General
- New logo
- Increased test coverage

#### Automation Engine
- Fixed that [`AddCommentToCardAction`](https://github.com/rwjdk/TrelloDotNet/wiki/AddCommentToCardAction) did not increment property `ActionsExecuted` in `ProcessingResult`
- Fixed incorrect spelling of property `Position` in `WebhookActionDataList` [COMPILE TIME BREAKING CHANGE]

---

## Version 1.7.0 (5th of August 2023)
#### General
- API will automatically retry failed requests that get the 'API_TOKEN_LIMIT_EXCEEDED' error (retry up to 3 times with 1 second between). You can change/disable this behavior via TrelloClientOptions if you like.
- Rewritten all tests to be faster and easier to maintain, while also being able to run on any Trello account
- Greatly increased test coverage (+ coverage can be inspected via the new [README.md](https://github.com/rwjdk/TrelloDotNet#readme) badge)

#### TrelloClient
- Added [`GetOrganizationsForMemberAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetOrganizationsForMemberAsync)
- Added [`GetOrganizationsCurrentTokenCanAccessAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetOrganizationsCurrentTokenCanAccessAsync)
- Added DebuggerDisplay to `Organization`
- Added option to set `OrganizationId` when creating a `Board`

---

## Version 1.6.9 (8th of July 2023)
#### Automation Engine
- Fixed that if you used option 'AddCheckItemsToExistingChecklist' in an `AddChecklistToCardAction` and a checklist existed but had no items, the automation failed.
- Added better error context to the AutomationException (what Board, List, and Card was involved in the event that caused the Exception)

---

## Version 1.6.8 (7th of July 2023)
#### TrelloClient
- Added [`DeleteChecklistItemAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/DeleteChecklistItemAsync)

---

## Version 1.6.7 (5th of July 2023)
#### General
- Changed the Project URL to point at the Wiki
- Restructured the ReadMe

---

## Version 1.6.6 (12th of June 2023)
#### General
- Added badges in [ReadMe](https://github.com/rwjdk/TrelloDotNet#readme) for easier discovery of additional resources

---

## Version 1.6.4 (9th of June 2023)
#### Automation Engine
- Added Automation Trigger [`CardMovedAwayFromListTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/CardMovedAwayFromListTrigger)

---

## Version 1.6.3 (7th of June 2023)
#### General
- Tweaked various documentation for spelling errors

#### TrelloClient
- Added [`MoveCardToListAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/MoveCardToListAsync) for greater discoverability of how to do it (was already possible via `UpdateCardAsync`)

#### Automation Engine
- Added Automation Action [`AddLabelsToCardAction`](https://github.com/rwjdk/TrelloDotNet/wiki/AddLabelsToCardAction)
- Added Automation Action [`AddMembersToCardAction`](https://github.com/rwjdk/TrelloDotNet/wiki/AddMembersToCardAction)
- Added Automation Action [`RemoveMembersFromCardAction`](https://github.com/rwjdk/TrelloDotNet/wiki/RemoveMembersFromCardAction)
- Added the Webhook object to the `WebhookNotification` (Trello recently exposed this feature)

---

## Version 1.6.2 (4th of June 2023)
#### General
- Tweaked various documentation for spelling errors

#### Automation Engine
- Added Automation Action [`RemoveCardDataAction`](https://github.com/rwjdk/TrelloDotNet/wiki/RemoveCardDataAction)

---

## Version 1.6.1 (31st of May 2023)
#### General
- Tweaked various documentation for spelling errors
- This changelog now links to the [Wiki](https://github.com/rwjdk/TrelloDotNet/wiki)

#### Automation Engine
- Added Automation Trigger [`CheckItemStateUpdatedOnCardTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/CheckItemStateUpdatedOnCardTrigger)
- Added Automation Condition [`ChecklistItemsCompleteCondition`](https://github.com/rwjdk/TrelloDotNet/wiki/ChecklistItemsCompleteCondition)

---

## Version 1.6.0 (12th of May 2023)
#### General
- Added option to pass [`CancellationToken`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken) to the API

#### TrelloClient
- Added [`GetActionsOfBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetActionsOfBoardAsync)
- Added [`GetActionsOnCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetActionsOnCardAsync)
- Added [`GetActionsForListAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetActionsForListAsync)
- Added [`GetActionsForMemberAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetActionsForMemberAsync)
- Added [`GetActionsForOrganizationAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetActionsForOrganizationsAsync)
- Added [`GetCardsForMemberAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetCardsForMemberAsync)
- Added [`AddMemberToBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/AddMemberToBoardAsync)
- Added [`RemoveMemberFromBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/RemoveMemberFromBoardAsync)
- Added [`UpdateMembershipTypeOfMemberOnBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateMembershipTypeOfMemberOnBoardAsync)
- Added [`GetBoardsInOrganization`](https://github.com/rwjdk/TrelloDotNet/wiki/GetBoardsInOrganization)
- Added [`GetMembersOfOrganizationAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetMembersOfOrganizationAsync)
- Added support for [`Organizations`](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloClient#organization-features) (also known as Workspaces)

#### Automation Engine
- Added Automation Action [`AddCommentToCardAction`](https://github.com/rwjdk/TrelloDotNet/wiki/AddCommentToCardAction)
- Added Automation Action [`StopProcessingFurtherAction`](https://github.com/rwjdk/TrelloDotNet/wiki/StopProcessingFurtherAction) that allows you to conditionally stop any further processing for the specific webhook call
- Updated Automation Action [`AddChecklistToCardAction`](https://github.com/rwjdk/TrelloDotNet/wiki/AddChecklistToCardAction) to now have the option to add items to existing checklists with the same name (Example: two Definition of Done automations for two different labels add their items to a single checklist)

---

## Version 1.5.2 (30th of March 2023)
#### Automation Engine
- Added Automation Action [`AddChecklistToCardIfLabelMatchAction`](https://github.com/rwjdk/TrelloDotNet/wiki/AddChecklistToCardIfLabelMatchAction) to make combining creation of checklists easier to maintain

---

## Version 1.5.1 (27th of March 2023)
#### General
- Fixed that strings were not properly URL encoded if they contained an '&'

---

## Version 1.5.0 (27th of March 2023)
#### TrelloClient
- Added support for managing [`Label` definitions](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloClient#label-features) on the Board
- Added support for managing [`Attachments`](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloClient#attachment-features)
- Added support to set/remove Attachment Cover on a Card
- Added [`UpdateChecklistItemAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateChecklistItemAsync)
- Added [`GetBoardsForMemberAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetBoardsForMemberAsync)
- Added [`GetBoardsCurrentTokenCanAccessAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetBoardsCurrentTokenCanAccessAsync)
- Added [`GetMembershipsOfBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetMembershipsOfBoardAsync)

#### Automation Engine
- Added Automation Action [`RemoveLabelsFromCardAction`](https://github.com/rwjdk/TrelloDotNet/wiki/RemoveLabelsFromCardAction)
- It is now legal to have a null as a Condition in Automation Engine (indicating there are no further conditions)
- Fixed that 'Constraint' was spelled incorrectly (missing an 's') in 2 places (`ListConditionConstraint` and `CardMovedToListTriggerConstraint`) [COMPILE TIME BREAKING CHANGE] (sorry but better to change now than later)
- Fixed that [`CardCoverCondition`](https://github.com/rwjdk/TrelloDotNet/wiki/CardCoverCondition) was not evaluated correctly in all scenarios
- Fixed that [`SetFieldsOnCardAction`](https://github.com/rwjdk/TrelloDotNet/wiki/SetFieldsOnCardAction) did not update processing result (Executed and Skipped Actions counts)

---

## Version 1.4.0 (3rd of April 2023)
#### TrelloClient
- Added [`AddCoverToCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/AddCoverToCardAsync)
- Added [`UpdateCoverOnCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateCoverOnCardAsync)
- Added [`RemoveCoverOnCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/RemoveCoverOnCardAsync)
- Updated [`UpdateCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateCardAsync) so it now has the ability to Add/Update/Remove the Cover of a card
- Added quality-of-life methods to maintain [`Webhooks`](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloClient#webhook-features) by callback URL instead of ID

#### Automation Engine
- Added RegEx option for string-comparison in Triggers and Conditions
- Added Automation Trigger [`CardUpdatedTrigger`](https://github.com/rwjdk/TrelloDotNet/wiki/CardUpdatedTrigger)

---

## Version 1.3.1 (30th of March 2023)
#### Automation Engine
- Updated Automation Condition [`ChecklistIncompleteCondition`](https://github.com/rwjdk/TrelloDotNet/wiki/ChecklistIncompleteCondition) to now have the option to define `ChecklistNameMatchCriteria` if, for example, you want to check lists with a certain name prefix

---

## Version 1.3.0 (27th of March 2023)
#### General
- Added [Webhook Automation Engine](https://github.com/rwjdk/TrelloDotNet/wiki/Automation-Engine) that makes it even easier to consume Webhooks (just define your automation rules and give the engine the Webhook JSON and it does the rest)
- Fixed various incorrect XML summaries and parameter names (all non-breaking changes)

#### TrelloClient
- Added support for retrieving, setting, and removing [`Custom Field`](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloClient#custom-field-features) values of cards
- Added [`DeleteChecklistAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/DeleteChecklistAsync)
- Added [`GetTokenInformationAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetTokenInformationAsync) to get information about the Trello Token used for connecting
- Added [`GetTokenMemberAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetTokenMemberAsync) that returns the user that owns the Trello Token used for connecting
- Fixed that null values in strings for an update of objects are now considered empty strings so you do not end up with 'null' values (the word 'null' as a string)

#### Webhook Receiver
- `WebhookAction` now has reference to the TrelloClient and the sub-objects can get their full objects
- Added struct [`WebhookActionTypes`](https://github.com/rwjdk/TrelloDotNet/blob/main/TrelloDotNet/TrelloDotNet/Model/Webhook/WebhookActionTypes.cs) that lists all types of Webhook events
- Added support for basic events `OnDeleteCustomField`, `OnAddCustomField`, `OnUpdateCustomField` and `OnUpdateCustomFieldItem`
- Added `ListBefore` and `ListAfter` to `TrelloActionData`

---

## Version 1.2.1 (25th of Feb. 2023)
#### TrelloClient
- Added [`UpdateCommentActionAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/UpdateCommentActionAsync) to update comments
- Added [`GetAllCommentsOnCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetAllCommentsOnCardAsync) and [`GetPagedCommentsOnCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/GetPagedCommentsOnCardAsync) to get existing comments on a card
- Updated [`AddChecklistAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/AddChecklistAsync) to now add positions of check items automatically in the same order as the list if none is specified
- Fixed that [`AddStickerToCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/AddStickerToCardAsync) did not work as intended (You got an 'Invalid ImageId' error) due to a late refactoring and poor testing on my part (last time I skimp on integration-tests!)
- Fixed that [`AddCommentAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/AddCommentAsync) had a wrong return object (Trello API is just weird and not consistent!)

#### Webhook Receiver
- Added generic WebHookNotification that does not care if Webhook returned from a Board, List, Card, etc

---

## Version 1.2.0 (23rd of Feb. 2023)
#### General
- Overridden `ToString()` methods on models are removed and `DebuggerDisplay` attributes are now used instead

#### TrelloClient
- Fixed that various methods were missing the `Async` suffix so it was added (Sorry for this breaking change (oversight by me) but better now than later)
- Added properties `OrganizationId`, `EnterpriseId`, and `Pinned` to the Board object
- Added support for [`Card Stickers`](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloClient#sticker-features)
- Added support to add [`Comments`](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloClient#comments-features) on cards

---

## Version 1.1.1 (17th of Feb. 2023)
#### TrelloClient
- Method `SetStartDateOnCardAsync` was renamed to [`SetStartDateAndDueDateOnCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/SetStartDateAndDueDateOnCardAsync) [COMPILE TIME BREAKING CHANGE]
- Added property `Closed` on Board so you can see if a board is closed or not
- Fixed that methods [`ArchiveListAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/ArchiveListAsync) and [`ReOpenListAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/ReOpenListAsync) incorrectly returned a Card and not a List [COMPILE TIME BREAKING CHANGE]
- Fixed that methods [`CloseBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/CloseBoardAsync) and [`ReOpenBoardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/ReOpenBoardAsync) incorrectly returned a Card and not a Board [COMPILE TIME BREAKING CHANGE]
- Fixed that method [`RemoveLabelsFromCardAsync`](https://github.com/rwjdk/TrelloDotNet/wiki/) did not work (wrongly implemented)

#### Webhook Receiver
- Fixed that Smart-events did not do a proper internal await so could technically be delayed

---

## Version 1.1.0 (8th of Feb. 2023)
#### General
- Added [Webhook System](https://github.com/rwjdk/TrelloDotNet/wiki/WebHook-Data-Receiver) (See video on how to get going [here](https://youtu.be/A3_B-SLBm_0))
- Fixed the Trello icon edges were white
- Added more detailed `README.md`

#### TrelloClient
- Added various 'ease of use' methods to do common actions (for example, add or remove Members/Labels from Cards)
- Made class `EnumViaJsonPropertyConverter` internal (incorrectly public) [COMPILE TIME BREAKING CHANGE]

---

## Version 1.0.0 (6th of Feb. 2023)
#### General
- First official version :-)

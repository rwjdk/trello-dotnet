This is the Main Client to communicate with the Trello API (aka everything is done via this)

## Creating a new TrelloClient
In order to instantiate the client you need to provide:
| Option | Description | 
|:---|:---|
| `ApiKey` (Required) | The Trello API Key you get on https://trello.com/power-ups/admin/ |
| `Token` (Required) | Your Authorization Token you generate get on https://trello.com/power-ups/admin/ |
| `Options` (Optional) | Various options for the client (if null default options will be used) See below for details |
| `HttpClient` (Optional) | Optional HTTP Client if you wish to specify it on your own (else an internal static HttpClient will be used for re-use) |

**Example**

- [E-Learning video](https://youtu.be/0SfAmTeVLwY)

```cs
TrelloClient client = new TrelloDotNet.TrelloClient("APIKEY", "TOKEN"); //IMPORTANT: Remember to NOT leave Key and Token in clear text!

//Get all boards that Token Owner can Access
List<Board> boards = await client.GetBoardsCurrentTokenCanAccessAsync();

//Get a specific board
Board board = await client.GetBoardAsync("<boardId>");

//Get Lists on a board
List<List> lists = await client.GetListsOnBoardAsync("<boardId>");

//Get Cards on Board
List<Card> cardsOnBoard = await trelloClient.GetCardsOnBoardAsync("<boardId>");

//Get Cards in a specific List
List<Card> cardsInList = await trelloClient.GetCardsInListAsync("<listId>");

//Get a specific card
Card card = await client.GetCardAsync("<cardId>");

//Add a card (Simple)
AddCardOptions newCardOptions = new AddCardOptions("<listId>", "My Card", "My Card description");
Card newCard = await client.AddCardAsync(newCardOptions);

//Add a Card (Advanced with all options set)
Card newAdvancedCard = await client.AddCardAsync(new AddCardOptions
{
    //Mandatory options
    ListId = "<listId>",
    Name = "My Card",

    //Optional options
    Description = "Description of My Card",
    Start = DateTimeOffset.Now,
    Due = DateTimeOffset.Now.AddDays(3),
    Cover = new CardCover(CardCoverColor.Blue, CardCoverSize.Normal),
    LabelIds = new List<string>
    {
        "<labelId1>",
        "<labelId2>",
    },
    MemberIds = new List<string>
    {
        "<memberId1>",
        "<memberId2>"
    },
    Checklists = new List<Checklist>
    {
        new Checklist("Checklist 1", new List<ChecklistItem>
        {
            new ChecklistItem("Item 1"),
            new ChecklistItem("Item 2"),
            new ChecklistItem("Item 3")
        }),
        new Checklist("Checklist 2", new List<ChecklistItem>
        {
            new ChecklistItem("Item A"),
            new ChecklistItem("Item B"),
            new ChecklistItem("Item C")
        }),
    },
    AttachmentUrlLinks = new List<AttachmentUrlLink>
    {
        new AttachmentUrlLink("https://www.google.com", "Google")
    },
    AttachmentFileUploads = new List<AttachmentFileUpload>
    {
        new AttachmentFileUpload(File.OpenRead(@"<pathToFile>"), "<Filename>", "<FileDescription>")
    },
    CustomFields = new List<AddCardOptionsCustomField>
    {
        new AddCardOptionsCustomField(customField1OnBoard, "ABC"),
        new AddCardOptionsCustomField(customField2OnBoard, 123),
    }
});

//Update a Card (with new name and description and removal of Due Date)
var updateCard = await TrelloClient.UpdateCardAsync("<cardId>", [
    CardUpdate.Name("New Name"),
    CardUpdate.Description("New Description"),
    CardUpdate.DueDate(null),
]);

//Add a Checklist to a card
var checklistItems = new List<ChecklistItem>
{
    new("ItemA"),
    new("ItemB"),
    new("ItemC")
};
Checklist newChecklist = new Checklist("Sample Checklist", checklistItems);
Checklist addedChecklist = await client.AddChecklistAsync("<cardId>", newChecklist);

```
### TrelloClientOptions
The TrelloClientOptions can be passed optionally to the TrelloClient Constructor with the following options:
| Option | Description | 
|:---|:---|
| `ApiCallExceptionOption` | Control level of URL Details are shown in Exceptions from calls to the API |
| `AllowDeleteOfBoards` | Controls if it is allowed to delete Boards (secondary confirmation) |
|`AllowDeleteOfOrganizations`| Controls if it is allowed to delete Organizations (secondary confirmation) |
|`MaxRetryCountForTokenLimitExceeded` | Controls how many automated Retries the API should try in case if get an 'API_TOKEN_LIMIT_EXCEEDED' error from Trello (Default 3) set to -1 to disable the system |
|`DelayInSecondsToWaitInTokenLimitExceededRetry` | Controls how long in seconds system should wait between retries, should it receive an 'API_TOKEN_LIMIT_EXCEEDED' error from Trello (Default 1 sec) |
|`SendCredentialsMode` | Controls if credentials are sent as query string (`QueryString`, default) or via `Authorization` header (`Header`) |

**Example of sending credentials via headers**
```cs
var options = new TrelloClientOptions
{
    SendCredentialsMode = SendCredentialsMode.Header
};
TrelloClient client = new TrelloClient("APIKEY", "TOKEN", options);
```

## Features of the API
- [Action Features](TrelloClient#action-features)
- [Attachment Features](TrelloClient#attachment-features) 
- [Board Features](TrelloClient#board-features) 
- [Card Features](TrelloClient#card-features) 
- [Checklist Features](TrelloClient#checklist-features)
- [Comments Features](TrelloClient#comments-features)
- [Cover Features](TrelloClient#cover-features)
- [Custom Field Features](TrelloClient#custom-field-features)
- [Generic Features](TrelloClient#generic-features)
- [Label Features](TrelloClient#label-features)
- [List Features](TrelloClient#list-features)
- [Member Features](TrelloClient#member-features)
- [Membership Features](TrelloClient#membership-features)
- [Organization Features](TrelloClient#organization-features)
- [PluginData Features](TrelloClient#plugindata-features)
- [Search Features](TrelloClient#search-features)
- [Sticker Features](TrelloClient#sticker-features)
- [Webhook Features](TrelloClient#webhook-features)


### Action Features
`Actions` in Trello are the 'things' the `Members` do on a `Board`... There are common things like 'Create a Card' or 'Add a Label to a Card', and more obscure things like example 'Accept Enterprise Join Request'. In total, there are over 75 different types of `Actions`. You can retrieve these events via the API to example get a list of things that happened on a specific card. 

> Tip: There is a list of different events in struct [`TrelloDotNet.Model.Webhook.WebhookActionTypes`](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/Model/Webhook/WebhookActionTypes.cs) for you to consume. 

> NB: You can max get the last 1000 Actions that have happened.

| Feature | Description |
|:---|:---|
| [GetActionsOfBoardAsync](GetActionsOfBoardAsync) | Get the most recent Actions (Changelog Events) of a board |
| [GetActionsOnCardAsync](GetActionsOnCardAsync) | Get the most recent Actions (Changelog Events) on a Card |
| [GetActionsForListAsync](GetActionsForListAsync) | Get the most recent Actions (Changelog Events) for a List |
| [GetActionsForMemberAsync](GetActionsForMemberAsync) | Get the most recent Actions (Changelog Events) for a Member |
| [GetActionsForOrganizationsAsync](GetActionsForOrganizationsAsync) | Get the most recent Actions (Changelog Events) for an Organization |

### Attachment Features
`Cards` can have `Attachments` of various types (Links and Files) and these methods allow you to add, get and delete the attachments.
| Feature| Description |
|:---|:---|
| [AddAttachmentToCardAsync](AddAttachmentToCardAsync) | Add an Attachment to a Card |
| [GetAttachmentOnCardAsync](GetAttachmentOnCardAsync) | Get a Specific Attachment on a Card |
| [GetAttachmentsOnCardAsync](GetAttachmentsOnCardAsync) | Get Attachments on a card |
| [DeleteAttachmentOnCardAsync](DeleteAttachmentOnCardAsync) | Delete an Attachments on a card |
| [DownloadAttachmentAsync](DownloadAttachmentAsync) | Download an Attachment on a card |

### Board Features
`Boards` are where you have your `Lists` (and `Cards` on those lists). The API gives standard CRUD features (good if you programmatically wish to spin up template `Boards` in large organizations). A Board's 'parent' is an `Organization` (Workspace)
| Feature| Description |
|:---|:---|
| [AddBoardAsync](AddBoardAsync) | Add a new Board |
| [GetBoardAsync](GetBoardAsync) | Get a Board by its Id |
| [GetBoardsForMemberAsync](GetBoardsForMemberAsync) | Get the Boards that the specified member has access to |
| [GetBoardsCurrentTokenCanAccessAsync](GetBoardsCurrentTokenCanAccessAsync) | Get the Boards that the token provided to the TrelloClient can Access |
| [GetBoardsInOrganization](GetBoardsInOrganization) | Get the Boards in an Organization |
| [UpdateBoardAsync](UpdateBoardAsync) | Update a Board |
| [CloseBoardAsync](CloseBoardAsync) | Close (Archive) a Board |
| [ReOpenBoardAsync](ReOpenBoardAsync) | ReOpen a Board |
| [DeleteBoardAsync](DeleteBoardAsync) | Delete an entire board |

### Card Features
`Cards` are the main feature of any `Trello Board`. There are associated with a List and have various core fields (`Name`, `Description`, `Dates`) and Links to other Trello artifacts (`Labels`, `Members`, `Checklists`, `Attachments`, `Stickers`, and `Covers`). Via the API you can do standard CRUD operations and link/unlink the other artifacts.

> Tip: On the various methods that get Cards you can optionally specify [GetCardOptions](GetCardOptions) to control what parts of a card is returned and what cards are returned when multiple is returned.

| Feature| Description |
|:---|:---|
| [AddCardAsync](AddCardAsync) | Add a Card |
| [GetCardAsync](GetCardAsync) | Get Card by its Id |
| [GetCardsOnBoardAsync](GetCardsOnBoardAsync) | Get all open cards on un-archived lists |
| [GetCardsInInboxAsync](GetCardsInInboxAsync) | Get Cards in the users inbox | 
| [GetCardsInListAsync](GetCardsInListAsync) | Get all open cards on a specific list |
| [GetCardsForMemberAsync](GetCardsForMemberAsync) | Get all Cards a Member is on (across multiple boards) |
| [MirrorCardAsync](MirrorCardASync) | Mirror a Card |
| [UpdateCardAsync](UpdateCardAsync) | Update a Card |
| [ArchiveCardAsync](ArchiveCardAsync) | Archive (Close) a Card |
| [ReOpenCardAsync](ReOpenCardAsync) | ReOpen (Send back to board) a Card|
| [ArchiveAllCardsInListAsync](ArchiveAllCardsInListAsync) | Archive all cards on in a List |
| [MoveAllCardsInListAsync](MoveAllCardsInListAsync) | Move all cards of a list to another list |
| [MoveCardToListAsync](MoveCardToListAsync) | Move card to another list on the board |
| [DeleteCardAsync](DeleteCardAsync) | Delete a Card |
| [SetDueDateOnCardAsync](SetDueDateOnCardAsync) | Set Due Date on a card |
| [SetStartDateOnCardAsync](SetStartDateOnCardAsync) | Set Due Date on a card |
| [SetStartDateAndDueDateOnCardAsync](SetStartDateAndDueDateOnCardAsync) | Set Start and Due Date on a card |

### Checklist Features
On a `Card`, you can have one or more `Checklists` that are essential 'SubTasks' for the card. If you are a free user they are simple checklists, while on a premium account, you can also assign members and due dates to each `Check-item`. Via the API you can do standard CRUD operations.
| Feature| Description |
|:---|:---|
| [AddChecklistAsync](AddChecklistAsync) | Add a Checklist to the card or Add a Checklist to the card based on an existing checklist (as a copy) |
| [GetChecklistAsync](GetChecklistAsync) | Get a Checklist with a specific Id |
| [GetChecklistsOnBoardAsync](GetChecklistsOnBoardAsync) | Get a list of Checklists that are used on cards on a specific Board |
| [GetChecklistsOnCardAsync](GetChecklistsOnCardAsync) | Get a list of Checklists that are used on a specific card |
| [UpdateChecklistItemAsync](UpdateChecklistItemAsync) | Update a Check-item on a Card |
| [DeleteChecklistAsync](DeleteChecklistAsync) | Delete a Checklist |
| [DeleteChecklistItemAsync](DeleteChecklistItemAsync) | Delete a Checklist Item from a checklist |

### Comments Features
On a `Card`, the `Members` of a `Board` can add Comments (Comments in Trello are essentially special `Actions` so they will also appear there). Via the API you can do standard CRUD operations.
| Feature| Description |
|:---|:---|
| [AddCommentAsync](AddCommentAsync) | Add a new Comment on a Card |
| [GetAllCommentsOnCardAsync](GetAllCommentsOnCardAsync) | Get All Comments on a Card |
| [GetCommentReactions](GetCommentReactions) | The reactions of a comment |
| [GetPagedCommentsOnCardAsync](GetPagedCommentsOnCardAsync) | Get Paged Comments on a Card |
| [UpdateCommentActionAsync](UpdateCommentActionAsync) | Update a comment Action (aka only way to update comments as they are not seen as their own objects) |
| [DeleteCommentActionAsync](DeleteCommentActionAsync) | Delete a Comment |

### Cover Features
`Covers` are a special visual feature on `Cards` that can help the card stand out (color at, the top, the full coloring of the card, or have an image at the top of the card). Via the API you can do standard CRUD operations for covers.
| Feature| Description |
|:---|:---|
| [AddCoverToCardAsync](AddCoverToCardAsync) | Add a Cover to a card. Tip: It is also possible to update the cover via [UpdateCardAsync](UpdateCardAsync) |
| [UpdateCoverOnCardAsync](UpdateCoverOnCardAsync) | Update a Cover to a card (this is equivalent to AddCoverToCardAsync, but here for discover-ability. Tip: It is also possible to update the cover via [`UpdateCardAsync`](UpdateCardAsync)) |
| [RemoveCoverFromCardAsync](RemoveCoverFromCardAsync) | Remove a cover from a Card |

### Custom Field Features
NB: Custom Fields are a Paid Trello Feature only :-/
`Custom Fields` are as the name says, fields that you can make yourself in order to add custom values to `Cards`. As an example, Trello does not have a default `Priority` field, but with a custom field, you could make one.

| Feature| Description |
|:---|:---|
| [GetCustomFieldsOnBoardAsync](GetCustomFieldsOnBoardAsync) | Get Custom Fields of a Board |
| [UpdateCustomFieldValueOnCardAsync](UpdateCustomFieldValueOnCardAsync) | Update a Custom field on a Card |
| [ClearCustomFieldValueOnCardAsync](ClearCustomFieldValueOnCardAsync) | Clear a Custom field on a Card |
| [GetCustomFieldItemsForCardAsync](GetCustomFieldItemsForCardAsync) | Get Custom Fields for a Card (Tip: Use Extension methods GetCustomFieldValueAsXYZ for a handy way to get values) |

### Generic Features
This API does not cover every single little or obscure feature the Trello API has to offer, but it could be that you wish to use something that is not exposed anyway. For that reason, Generic `Post`, `Put`, `Get`, and `Delete` methods exist in the API where you can provide the endpoint, parameters and the API will take care of all the core stuff of the call
>Tip: If you feel it should be in the product then submit it on the [Issues](https://github.com/rwjdk/trello-dotnet/issues) page). 

| Feature| Description |
|:---|:---|
| [PostAsync](PostAsync) | Custom Post Method to be used on unexposed features of the API |
| [GetAsync](GetAsync) | Custom Get Method to be used on unexposed features of the API |
| [PutAsync](PutAsync) | Custom Put Method to be used on unexposed features of the API |
| [DeleteAsync](DeleteAsync) | Custom Delete Method to be used on unexposed features of the API |

### Label Features
`Labels` (or Tags as they are called in other systems) can be assigned to `Cards` to categorize them. The API provides both CRUD operations for the management of Labels (they belong to a board) and the add/removal of labels on Cards.

| Feature| Description |
|:---|:---|
| [AddLabelsToCardAsync](AddLabelsToCardAsync) | Add a Label to a Card |
| [RemoveLabelsFromCardAsync](RemoveLabelsFromCardAsync) | Remove a Label of a Card |
| [RemoveAllLabelsFromCardAsync](RemoveAllLabelsFromCardAsync) | Remove all Labels of a Card |
| [GetLabelsOfBoardAsync](GetLabelsOfBoardAsync) | Get List of Labels defined for a board |
| [AddLabelAsync](AddLabelAsync) | Add a new label to the Board (Not to be confused with [`AddLabelsToCardAsync`](AddLabelsToCardAsync) that assign labels to cards) |
| [UpdateLabelAsync](UpdateLabelAsync) | Update the definition of a label (Name and Color) |
| [DeleteLabelAsync](DeleteLabelAsync) | Delete a Label from the board and remove it from all cards it was added to |

### List Features
`Lists` are the 'Columns' you see on your Board and hold your Cards. They are pretty simple structures with just an `Id` and a `Name`. Via the API you can do standard CRUD operations.
| Feature| Description |
|:---|:---|
| [AddListAsync](AddListAsync) |  Add a List to a Board |
| [GetListAsync](GetListAsync) | Get a specific List (Column) based on its Id |
| [GetListsOnBoardAsync](GetListsOnBoardAsync) | Get Lists (Columns) on a Board |
| [UpdateListAsync](UpdateListAsync) | Update a List |
| [MoveListToBoardAsync](MoveListToBoardAsync) | Move an entire list to another board |
| [ArchiveListAsync](ArchiveListAsync) | Archive a List |
| [ReOpenListAsync](ReOpenListAsync) | Reopen a List (Send back to the board) |

### Member Features
`Members` are the users of Trello, and via the API you can manage invites and access to `Boards` and `Organizations`, as well as the assignment of them to `Cards`
| Feature| Description |
|:---|:---|
| [AddMembersToCardAsync](AddMembersToCardAsync) | Add one or more Members to a Card |
| [AddMemberToBoardAsync](AddMemberToBoardAsync) | Add a Member to a board (aka give them access) |
| [GetMemberAsync](GetMemberAsync) | Get a Member with a specific Id |
| [GetMembersOfCardAsync](GetMembersOfCardAsync) | Get the Members (users) of a Card |
| [GetMembersOfBoardAsync](GetMembersOfBoardAsync) | Get the Members (users) of a board |
| [GetMembersOfOrganizationAsync](GetMembersOfOrganizationAsync) | Get the Members (users) of an Organization |
| [RemoveMembersFromCardAsync](RemoveMembersFromCardAsync) | Remove a Member of a Card |
| [RemoveAllMembersFromCardAsync](RemoveAllMembersFromCardAsync) | Remove all Members of a Card |
| [RemoveMemberFromBoardAsync](RemoveMemberFromBoardAsync) | Remove a Member from a board (aka revoke access) |
| [InviteMemberToBoardViaEmailAsync](InviteMemberToBoardViaEmailAsync) | Invite a Member to a board via email (aka give them access) |
| [GetTokenMemberAsync](GetTokenMemberAsync) | Get information about the Member that owns the token used by this TrelloClient |
| [AddVoteToCardAsync](AddVoteToCardAsync) | Add a Vote from a member to a card |
| [RemoveVoteFromCardAsync](RemoveVoteFromCardAsync) | Remove a Member Vote from a card |
| [GetMembersWhoVotedOnCardAsync](GetMembersWhoVotedOnCardAsync) | Get a list of Members who have voted for a Card |

### Membership Features
`Memberships` are information about a `Member` access to a thing (Example: if the Member is `Admin` or `Normal User` on a `Board`). Via the API you can get and update the memberships

| Feature| Description |
|:---|:---|
| [GetMembershipsOfBoardAsync](GetMembershipsOfBoardAsync)  |The Membership Information for a board (aka if Users are Admin, Normal, or Observer) |
| [UpdateMembershipTypeOfMemberOnBoardAsync](UpdateMembershipTypeOfMemberOnBoardAsync) | Change the membership type of a member Member on a board (Example make them Admin) |

### Organization Features
`Organizations` are in Trello called `Workspaces` but in the API the `Organization` name is kept to better align with official documentation. The API provides basic CRUD operations

| Feature| Description |
|:---|:---|
| [AddOrganizationAsync](AddOrganizationAsync) | Create a new Organization (Workspace) |
| [GetOrganizationAsync](GetOrganizationAsync) | Get an Organization (also known as Workspace) |
| [GetOrganizationsForMemberAsync](GetOrganizationsForMemberAsync) | Get the Organizations that the specified member has access to |
| [GetOrganizationsCurrentTokenCanAccessAsync](GetOrganizationsCurrentTokenCanAccessAsync) | Get the Organizations that the token provided to the TrelloClient can Access |
| [UpdateOrganizationAsync](UpdateOrganizationAsync) | Update an Organization (Workspace) |
| [DeleteOrganizationAsync](DeleteOrganizationAsync) | Delete an entire Organization including all Boards it contains |

### PluginData Features
`PluginData` are in Trello data from PowerUps that are bound to cards or boards

| Feature| Description |
|:---|:---|
| [GetPluginDataOnCardAsync](GetPluginDataOnCardAsync) | Get PluginData on a Card |
| [GetPluginDataOfBoardAsync](GetPluginDataOfBoardAsync) | Get PluginData of a Board |

### Search Features
`Search` makes it possible to search Organizations, Boards, Cards and Members.

| Feature | Description |
|:---|:---|
| [SearchAsync](SearchAsync) | Search Trello for Cards, Boards, and/or Organizations |
| [SearchMembersAsync](SearchMembersAsync) | Search for Trello members. |

### Sticker Features
`Stickers` are visuals you can 'attach' at the top of `Cards` to indicate something special (for example a warning sticker about something is wrong with the card) or someone did a good/bad job with a thumbs-up/thumbs-down sticker. The API provides operations to add and remove stickers to cards.

| Feature | Description |
|:---|:---|
| [AddStickerToCardAsync](AddStickerToCardAsync) | Add a sticker to a card |
| [GetStickerAsync](GetStickerAsync) | Get a Stickers with a specific Id |
| [GetStickersOnCardAsync](GetStickersOnCardAsync) | Get List of Stickers on a card |
| [UpdateStickerAsync](UpdateStickerAsync) | Update a sticker |
| [DeleteStickerAsync](DeleteStickerAsync) | Delete a sticker |

### Webhook Features
Trello has the option to create `Webhooks` that can, on each `Action` event you users do on a `board`, send the information to a `Callback` URL. The raw API provides CRUD operations for managing these Webhook subscriptions. For the reaction of event see the [Automation Engine](Automation-Engine) system [recommended] and/or the [Webhook Data Receiver](Webhook-Data-Receiver)

| Feature | Description |
|:---|:---|
| [AddWebhookAsync](AddWebhookAsync) | Add a new Webhook |
| [GetWebhooksForCurrentTokenAsync](GetWebhooksForCurrentTokenAsync) | Get Webhooks linked with the current Token used to authenticate with the API|
| [GetWebhookAsync](GetWebhookAsync) | Get a Webhook from its Id |
| [UpdateWebhookAsync](UpdateWebhookAsync) | Update a webhook |
| [UpdateWebhookByCallbackUrlAsync](UpdateWebhookByCallbackUrlAsync) | Replace callback URL for one or more Webhooks |
| [DeleteWebhookAsync](DeleteWebhookAsync) | Delete a Webhook |
| [DeleteWebhooksByCallbackUrlAsync](DeleteWebhooksByCallbackUrlAsync) | Delete Webhooks using indicated Callback URL |
| [DeleteWebhooksByTargetModelIdAsync](DeleteWebhooksByTargetModelIdAsync) | Delete Webhooks using indicated target ModelId |

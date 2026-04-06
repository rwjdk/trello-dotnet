[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/rwjdk/TrelloDotNet/Build.yml?style=for-the-badge)](https://github.com/rwjdk/TrelloDotNet/actions)
[![GitHub Issues or Pull Requests by label](https://img.shields.io/github/issues/rwjdk/TrelloDotNet/bug?style=for-the-badge&label=Bugs)](https://github.com/rwjdk/TrelloDotNet/issues?q=is%3Aissue%20state%3Aopen%20label%3Abug)
[![Libraries.io dependency status for GitHub repo](https://img.shields.io/librariesio/github/rwjdk/TrelloDotNet?style=for-the-badge)](https://github.com/rwjdk/TrelloDotNet/network/dependencies)
[![Coveralls](https://img.shields.io/coverallsCoverage/github/rwjdk/TrelloDotNet?style=for-the-badge)](https://coveralls.io/github/rwjdk/TrelloDotNet)

# TrelloDotNet
_Welcome to TrelloDotNet - A .NET implementation of the [Trello REST API](https://developer.atlassian.com/cloud/trello/rest)_

[![NuGet](https://img.shields.io/badge/NuGet-blue?style=for-the-badge)](https://www.nuget.org/packages/TrelloDotNet)
[![WIKI](https://img.shields.io/badge/Wiki-brown?style=for-the-badge)](https://github.com/rwjdk/TrelloDotNet/wiki)
[![Changelog](https://img.shields.io/badge/-Changelog-darkgreen?style=for-the-badge)](https://github.com/rwjdk/TrelloDotNet/blob/main/Changelog.md)
[![YouTube](https://img.shields.io/badge/-YouTube-darkred?style=for-the-badge)](https://www.youtube.com/playlist?list=PLhGl0l5La4saguVChJ3jmlAXqFDkmYjdC)
[![Rest API](https://img.shields.io/badge/API_Reference-gray?style=for-the-badge)](https://developer.atlassian.com/cloud/trello/rest/)
[![API Keys](https://img.shields.io/badge/Power--Ups_Admin-purple?style=for-the-badge)](https://trello.com/power-ups/admin/)
[![MCP Server](https://img.shields.io/badge/MCP--Server-green?style=for-the-badge)](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloDotNet-MCP-Server)


## Features
- A [TrelloClient](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloClient) for CRUD operations on Trello features
- An [Automation Engine](https://github.com/rwjdk/TrelloDotNet/wiki/Automation-Engine) and [Webhook Data Receiver](https://github.com/rwjdk/TrelloDotNet/wiki/Webhook-Data-Receiver) for handling webhook events

## Getting Started
*To get started you can either follow the steps below or use the [TrelloDotNet MCP Server](https://github.com/rwjdk/TrelloDotNet/wiki/TrelloDotNet-MCP-Server) to let AI assist you and Vibe Code the whole thing* 8-)

1. Install the '[TrelloDotNet](https://www.nuget.org/packages/TrelloDotNet)' NuGet Package (`dotnet add package TrelloDotNet`)
2. Retrieve your [API-Key and Token](https://youtu.be/ndLSAD3StH8) from the [PowerUps Administration](https://trello.com/power-ups/admin)
3. Create a new instance of the `TrelloClient` _(located in the namespace 'TrelloDotNet')_
4. Locate your IDs of your Boards, Lists, and Cards (see video [here](https://youtu.be/es84INLIiKI) or at the end of this ReadMe)
5. Use the TrelloClient based on the examples below and/or the [Wiki](https://github.com/rwjdk/TrelloDotNet/wiki).

### Examples of Usage:

```cs
TrelloClient client = new TrelloDotNet.TrelloClient("APIKEY", "TOKEN"); //IMPORTANT: Remember to NOT leave Key and Token in clear text!

//Get all boards that the Token Owner can access
List<Board> boards = await client.GetBoardsCurrentTokenCanAccessAsync();

//Get a specific board
Board board = await client.GetBoardAsync("<boardId>");

//Get lists on a board
List<List> lists = await client.GetListsOnBoardAsync("<boardId>");

//Get cards on a board
List<Card> cardsOnBoard = await client.GetCardsOnBoardAsync("<boardId>");

//Get cards in a specific list
List<Card> cardsInList = await client.GetCardsInListAsync("<listId>");

//Get a specific card
Card card = await client.GetCardAsync("<cardId>");

//Add a card (simple)
AddCardOptions newCardOptions = new AddCardOptions("<listId>", "My Card", "My Card description");
Card newCard = await client.AddCardAsync(newCardOptions);

//Add a card (advanced, with all options set)
Card newAdvancedCard = await client.AddCardAsync(new AddCardOptions
{
    //Required options
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

//Update a card (with new name and description and removal of Due Date)
var updateCard = await client.UpdateCardAsync("<cardId>", [
    CardUpdate.Name("New Name"),
    CardUpdate.Description("New Description"),
    CardUpdate.DueDate(null),
]);

//Add a checklist to a card
var checklistItems = new List<ChecklistItem>
{
    new("ItemA"),
    new("ItemB"),
    new("ItemC")
};
Checklist newChecklist = new Checklist("Sample Checklist", checklistItems);
Checklist addedChecklist = await client.AddChecklistAsync("<cardId>", newChecklist);

```

## Video Guides
- Trello Developer Fundamentals
  - [How to get your API-Key and Token](https://youtu.be/ndLSAD3StH8)
  - [How to Find IDs on a Trello Board](https://youtu.be/es84INLIiKI)
- TrelloDotNet
  - [How to use the TrelloDotNet NuGet Package](https://youtu.be/tf47BCkieus)
  - [E-Learning Playlist](https://www.youtube.com/playlist?list=PLhGl0l5La4sZJxSCNYl0AfCagdRB_c8CD)
  - Webhook
    - [How to work with Webhooks (Part 1: Setup)](https://youtu.be/A3_B-SLBm_0)
    - [How to work with Webhooks (Part 2: Receiving Events)](https://youtu.be/GsGKDDvuq40)

## Handy links
- [Udemy Course](https://www.udemy.com/course/7120835)
- [Wiki](https://github.com/rwjdk/TrelloDotNet/wiki)
- [Changelog](https://github.com/rwjdk/TrelloDotNet/blob/main/Changelog.md)
  - [Version 2.0 Migration Guide](https://github.com/rwjdk/TrelloDotNet/issues/51)
- [Report an issue](https://github.com/rwjdk/TrelloDotNet/issues)
- [Report a security concern](https://github.com/rwjdk/TrelloDotNet/security)
- [TrelloDotNet NuGet Package](https://www.nuget.org/packages/TrelloDotNet) 
- [Developers LinkedIn Group](https://www.linkedin.com/groups/12847286/) 
- [Power-Up to locate IDs](https://trello.com/power-ups/646cc3622176aebf713bb7f8/api-developer-id-helper)
- [Trello API YouTube Playlist](https://www.youtube.com/playlist?list=PLhGl0l5La4saguVChJ3jmlAXqFDkmYjdC)
- [Power-Ups Admin Center for API Keys and Tokens](https://trello.com/power-ups/admin/)
- [Trello API Documentation](https://developer.atlassian.com/cloud/trello/rest)
- [Trello Changelog](https://developer.atlassian.com/cloud/trello/changelog/)
- [How to build your first Power-Up](https://www.youtube.com/watch?v=dLCkcQnwAQk&ab_channel=TrelloDevelopers)

## On the subject of getting IDs from Trello
The easiest way to get IDs in Trello is to use this [Power-Up](https://trello.com/power-ups/646cc3622176aebf713bb7f8/api-developer-id-helper) to copy/paste them (recommended).

![API Developer ID Helper Power-Up](https://i.imgur.com/4FR6K2t.gif)

Alternatively, use the share buttons in the project (requires no Power-Up but is more cumbersome).

![Trello Board](https://i.imgur.com/D6vxkrm.png)

The export looks like this (search for ID or use a tool to pretty-print the JSON to get a better view):

![JSON Example](https://i.imgur.com/qDJgzNz.png)

*Have fun!*

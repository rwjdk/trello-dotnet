The TrelloDotNet Automation Engine, is a system that leverages [`Trello Webhooks`](TrelloClient#webhook-features), in order to automate processes on a Trello Board when users trigger specific scenarios.

> A more manual alternative to the Automation Engine is the [Webhook data receiver](WebHook-Data-Receiver). For comparison see [this page](Automation-Engine-vs-WebHook-Data-Receiver-Comparison)

You can break any automation down to 3 parts; <`Trigger(s)`>, optional <`Condition(s)`>, and <`Action(s)`>
#### One or More <`Trigger(s)`>:
The user-action on a Trello Board (Examples: 'Move a Card to another list', 'Add a Member to a Card', 'Remove a Label from a Card', 'Create a Card', etc.)

#### Zero, one or more <`Condition(s)`>:
The Condition of an object, most often a card, have at the time of the trigger (Examples: 'Card has no members', 'Card is in List 'In Progress', 'There is an incomplete checklist on the Card')

#### One or More <`Action(s)`>: 
If the trigger happens and Condition(s) are met, it is time to do the Automation Action. The Action will often be something affecting the Trello Board (often on a card) or some 3rd party system with a [Custom Action](Custom-Automation-Actions) (Examples: 'Add a Label', 'Add a Warning sticker', 'Add a Checklist', 'Remove a member')

## Examples of Automations the system supports (for inspiration):
1. When a Card is moved to the In Progress Column <`Trigger`> and it has Label 'Product Feature' <`Condition`> then add a [Definition of Done](https://www.agilealliance.org/glossary/definition-of-done/) Checklist <`Action`>.
2. When a Card is moved to the 'Done' List <`Trigger`>, check if there are incomplete checklists <`Condition`> and if so add a warning sticker <`Action`>
3. When the 'Bug' label is added to a Card <`Trigger`>, and there are no members on the Card <`Condition`>, Send an e-mail to warn Trello Board Administrators <`Custom Action`>

_(There is no fixed set of scenarios the system can handle, only your imagination)_

## How to set up (Building Blocks)
In order to get the Automation Engine working you need to do the following steps:
1. Get an [API Key and Token](https://youtu.be/ndLSAD3StH8)
2. Create a C# Based API Endpoint ([Azure Function](https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview?pivots=programming-language-csharp), [ASP.NET Core API](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-7.0), [Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-7.0), or similar) that can accept HTTP POST and HEAD messages and run on HTTPS
> TIP: For Local development use [NGROK](https://ngrok.com/) or Visual Studio [DevTunnels](https://learn.microsoft.com/en-us/aspnet/core/test/dev-tunnels?view=aspnetcore-7.0)
3. Add NuGet package [TrelloDotNet](https://www.nuget.org/packages/TrelloDotNet) to the project
4. Add the following basic code for now to the Endpoint (Code is based on Azure Function but adjust accordingly to get the JSON)

```cs
public class WebhookReceiver
{
    [Function("WebhookReceiver")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", "head")] HttpRequestData req)
    {
        try
        {
            //Get a configured Trello client
            var trelloClient = new TrelloClient("MY_API_KEY", "MY_TOKEN");

            //Create the configuration 
            List<Automation> automations = new List<Automation>
            {
                //todo - Added you needed automations. Here is a dummy Automation as example:

                new Automation("Name of Automation: Example When Card is moved to In Progress Column, Add Definition of Done Checklist",
                    new CardMovedToListTrigger(CardMovedToListTriggerConstraint.AnyOfTheseListsAreMovedTo, "In Progress")
                    {
                        TreatListNameAsId = true
                    },
                    new List<IAutomationCondition>
                    {
                        new LabelCondition(LabelConditionConstraint.AnyOfThesePresent, "Bug")
                        {
                            TreatLabelNameAsId = true
                        }
                    },
                    new List<IAutomationAction>
                    {
                        new AddChecklistToCardAction(new Checklist("Bug Definition of Done Checklist", new List<ChecklistItem>
                        {
                            new ChecklistItem("Reproduce error in local environment"),
                            new ChecklistItem("Add Unit-test to cover bug"),
                            new ChecklistItem("Do Root cause analysis")
                        }))

                    })

            };
            var configuration = new Configuration(trelloClient, automations.ToArray());

            //Create the Automation controller
            var automationController = new TrelloDotNet.AutomationEngine.AutomationController(configuration);

            //Get The raw JSON from the webhook and process it
            using var streamReader = new StreamReader(req.Body);
            var json = await streamReader.ReadToEndAsync();
            var result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(json));
            //todo - Optional: use result for any sort of post-processing of stats
        }
        catch (Exception e)
        {
            //todo - deal with exceptions (error mail or the like) and potential retry
        }

        return req.CreateResponse(HttpStatusCode.OK);//This is still OK as Trello should not see an exception as the receiver 
    }
}
```

5. Run the Code so it is publically accessible to the internet
6. Make a Console App or similar with the TrelloDotNet Nuget to [register the Webhook](AddWebhookAsync) (alternative use Postman as it is one-time code)
7. Add the following code and run it:
```cs
var trelloClient = new TrelloClient("MY_API_KEY", "MY_TOKEN");
string description = "My first Webhook";
string callbackUrl = "https://myUrlThatCanReceiveTheWebhook";
var idOfTypeYouWishToMonitor = boardId;
var webhook = new Webhook(description, callbackUrl, idOfTypeYouWishToMonitor);
var addedWebhook = trelloClient.AddWebhookAsync(webhook);
```

**You should now see the dummy automation trigger if the trigger and condition is met, and you are ready to use the below <`Trigger(s)`>, <`Condition(s)`>, and <`Action(s)`> to create your real Automations.**

Here is the full setup in overview form:

![image](https://github.com/rwjdk/trello-dotnet/assets/7032102/0d5eb770-4c36-42e8-bac6-734d23e7bc23)

## Triggers
The Automation engine has the following out-of-the-box Triggers

> Note: In the examples of Triggers below we over-use the `TreatListNameAsId` and `TreatLabelNameAsId` feature more than in 'real code' (as it is easier to understand). In real code using IDs is better as the code is more resilient to List/Label name changes on the board.

| Trigger | Description |
|:---|:---|
| [CardCreatedTrigger](CardCreatedTrigger) | Trigger when a card is added |
| [CardEmailedTrigger](CardEmailedTrigger) | Trigger when a card is emailed to the board |
| [CardMovedAwayFromListTrigger](CardMovedAwayFromListTrigger) | Trigger of an event that is a Card is Moved away from a List |
| [CardMovedToBoardTrigger](CardMovedToBoardTrigger) | Trigger that happens when a card is moved to the board |
| [CardMovedToListTrigger](CardMovedToListTrigger) | Trigger of an event that is a Card is Moved to a List |
| [CardUpdatedTrigger](CardUpdatedTrigger) | Trigger when a card is updated |
| [CheckItemStateUpdatedOnCardTrigger](CheckItemStateUpdatedOnCardTrigger) | Trigger that occurs when a Check-item on a Card Change State |
| [LabelAddedToCardTrigger](LabelAddedToCardTrigger) | Trigger that occurs when a Card gets a new label Added |
| [LabelRemovedFromCardTrigger](LabelRemovedFromCardTrigger) | Trigger that occurs when a Card has a label removed |
| [MemberAddedToCardTrigger](MemberAddedToCardTrigger) | Trigger that occurs when a Card gets a new Member Added |
| [MemberRemovedFromCardTrigger](MemberRemovedFromCardTrigger) | Trigger that occurs when a Card has a Member removed |

_If you need other/more advanced triggers then please submit a [feature request issue](https://github.com/rwjdk/trello-dotnet/issues) or make your own [Custom Trigger](Custom-Automation-Triggers)_

## Conditions
The Automation engine has the following out-of-the-box Conditions

> Note: In the examples of Conditions below we over-use the `TreatListNameAsId` and `TreatLabelNameAsId` feature more than in 'real code' (as it is easier to understand). In real code using IDs is better as the code is more resilient to List/Label name changes on the board.

| Condition | Description |
|:---|:---|
| [CardCoverCondition](CardCoverCondition) | Check if a Card has a specific Cover or not |
| [CardFieldCondition](CardFieldCondition) | Check if a Card Field has a particular value |
| [ChecklistItemsCompleteCondition](ChecklistItemsCompleteCondition) | Condition that checks if a Card has all its check-items complete |
| [ChecklistItemsIncompleteCondition](ChecklistItemsIncompleteCondition) | Condition that checks if a Card has one or more incomplete check items on any of its checklists |
| [ChecklistIncompleteCondition](ChecklistIncompleteCondition) | Condition that checks if a Card has a checklist with a specific name and that checklist has one or more incomplete items | Condition that checks if a Card has one or more incomplete check items on any of its checklists |
| [ChecklistNotStartedCondition](ChecklistNotStartedCondition) | Condition that checks if a Card has a checklist with a specific name and that none of the items on that list is completed (aka the Checklist has not been started) |
| [LabelCondition](LabelCondition) | Condition that checks labels on a Card is present/not present |
| [ListCondition](ListCondition) | Condition that checks if a card is on a specific list or the event involved a specific list |
| [MemberCondition](MemberCondition) | Condition that checks whether Members on a Card is present/not present |

_If you need other/more advanced conditions then please submit a [feature request issue](https://github.com/rwjdk/trello-dotnet/issues) or make your own [Custom Condition](Custom-Automation-Conditions)_

## Actions
The Automation engine has the following out-of-the-box Actions

> Note: In the examples of Actions below we over-use the `TreatListNameAsId` and `TreatLabelNameAsId` feature more than in 'real code' (as it is easier to understand). In real code using IDs is better as the code is more resilient to List/Label name changes on the board.

| Action | Description |
|:---|:---|
| [AddChecklistToCardAction](AddChecklistToCardAction) | This Automation Action adds a specific Checklist to a card if it is not already present. (This is often used for automation of 'Definition of Done') |
| [AddChecklistToCardIfLabelMatchAction](AddChecklistToCardIfLabelMatchAction) | This Automation Action adds Checklists to cards if it is not already present based on what Labels are present on the card. |
| [AddCommentToCardAction](AddCommentToCardAction) | Add a Comment to the Card |
| [AddCoverOnCardAction](AddCoverOnCardAction) | This Automation Action adds/updates a Cover on a card (This is often used to warn about something irregular on a Card, for example when it is moved to 'Done') |
| [AddLabelsToCardAction](AddLabelsToCardAction) | This Automation Action adds a set of Labels to a card if they are not already present. |
| [AddMembersToCardAction](AddMembersToCardAction) | This Automation Action adds a set of Members to a card if they are not already present. |
| [AddStickerToCardAction](AddStickerToCardAction) | This Automation Action adds a specific Sticker to a card if it is not already present. (This is often used to warn about something irregular on a Card, for example when it is moved to 'Done') |
| [RemoveCardDataAction](RemoveCardDataAction) | Action to remove one or more field values on a card
| [RemoveChecklistFromCardAction](RemoveChecklistFromCardAction) | This Automation Action Removes a Checklist from a Card if it is present |
| [RemoveCoverFromCardAction](RemoveCoverFromCardAction) | This Automation Action Removes a Cover from a card |
| [RemoveLabelsFromCardAction](RemoveLabelsFromCardAction) | Remove one or more Labels from a card |
| [RemoveMembersFromCardAction](RemoveMembersFromCardAction) | Remove one or more Members from a card |
| [RemoveStickerFromCardAction](RemoveStickerFromCardAction) | This Automation Action removes a specific Sticker from a card based on imageId |
| [SetFieldsOnCardAction](SetFieldsOnCardAction) | Action to set one or more fields on a card |
| [StopProcessingFurtherAction](StopProcessingFurtherAction) | An action that will stop any further processing of automations after this one for the given Webhook Receive Request |

_If you need other/more advanced Actions then please submit a [feature request issue](https://github.com/rwjdk/trello-dotnet/issues) or make your own [Custom Action](Custom-Automation-Actions)_
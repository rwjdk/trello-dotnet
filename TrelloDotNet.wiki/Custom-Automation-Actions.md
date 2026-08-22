The [Automation Engine](Automation-Engine) has various [out-of-the box Actions](Automation-Engine#actions), but there can be cases where you wish to make your own custom Actions 

> If you think the Action you need so be an out-of-the-box Action feel free to [open an issue](https://github.com/rwjdk/trello-dotnet/issues) or [create a Pull Request](https://github.com/rwjdk/trello-dotnet/pulls) with your custom Action

### Steps to make a custom Action

1. Make a new C# class and Call it 'MyCustomCondition' (or what you like it to be called but it is recommended to suffix it 'Condition')
2. Let the class implement interface `IAutomationCondition`
3. Let the class implement the mandatory member `IsConditionMetAsync`

#### Basic Example
``` cs
public class MyCustomAction : IAutomationAction
{
    public Task PerformActionAsync(WebhookAction webhookAction, ProcessingResult processingResult)
    {
        //Your Logic if the Action goes here

        TrelloClient trelloClient = webhookAction.TrelloClient; //You have full access to the TrelloClient

        WebhookActionData webhookActionData = webhookAction.Data; //This object has data about the event so you example can get the CardId of the Card for the event

        //The 'processingResult' instance can be used to communicate back what happened and if the action was executed or skipped + Any log messages back
        bool doneSomeWork = true;
        if (doneSomeWork)
        {
            processingResult.Log.Add(new ProcessingResultLogEntry("We did stuff"));
            processingResult.ActionsExecuted++;
        }
        else
        {
            processingResult.Log.Add(new ProcessingResultLogEntry("We skipped stuff (because xyz)"));
            processingResult.ActionsSkipped++;
        }
    }
}
```

Inside the `PerformActionAsync` you have access to the WebhookAction (aka the JSON given by the Webhook) + The TrelloClient for further data lookups and manipulation if needed

#### Real Examples
To better get a sense of how `Actions` look are here links to the various real-time implementations for inspiration:
- Source Code: [AddChecklistToCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/AddChecklistToCardAction.cs)
- Source Code: [AddChecklistToCardIfLabelMatchAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/AddChecklistToCardIfLabelMatchAction.cs)
- Source Code: [AddCommentToCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/AddCommentToCardAction.cs)
- Source Code: [AddCoverOnCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/AddCoverOnCardAction.cs)
- Source Code: [AddLabelsToCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/AddLabelsToCardAction.cs)
- Source Code: [AddMembersToCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/AddMembersToCardAction.cs)
- Source Code: [AddStickerToCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/AddStickerToCardAction.cs)
- Source Code: [RemoveCardDataAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/RemoveCardDataAction.cs)
- Source Code: [RemoveChecklistFromCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/RemoveChecklistFromCardAction.cs)
- Source Code: [RemoveCoverFromCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/RemoveCoverFromCardAction.cs)
- Source Code: [RemoveLabelsFromCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/RemoveLabelsFromCardAction.cs)
- Source Code: [RemoveMembersFromCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/RemoveMembersFromCardAction.cs)
- Source Code: [RemoveStickerFromCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/RemoveStickerFromCardAction.cs)
- Source Code: [SetFieldsOnCardAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/SetFieldsOnCardAction.cs)
- Source Code: [StopProcessingFurtherAction](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Actions/StopProcessingFurtherAction.cs)

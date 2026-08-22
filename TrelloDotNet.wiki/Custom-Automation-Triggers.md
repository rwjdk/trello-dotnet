The [Automation Engine](Automation-Engine) has various [out-of-the box Triggers](Automation-Engine#triggers), but there can be rare cases where you wish to make your own trigger 

> It is from experience more common to make [Custom Conditions](CustomAutomationConditions) and [Custom Actions](CustomAutomationActions)), but it is still doable of cause

> If you think the Trigger you need so be an out-of-the-box trigger feel free to [open an issue](https://github.com/rwjdk/trello-dotnet/issues) or [create a Pull Request](https://github.com/rwjdk/trello-dotnet/pulls) with your custom Trigger

### Steps to make a custom Trigger

1. Make a new C# class and Call it 'MyCustomTrigger' (or what you like it to be called but it is recommended to suffix it 'Trigger')
2. Let the class implement interface `IAutomationTrigger`
3. Let the class implement the mandatory member `IsTriggerMetAsync`

#### Basic Example
``` cs

//Your custom trigger
public class MyCustomTrigger : IAutomationTrigger
{
    public async Task<bool> IsTriggerMetAsync(WebhookAction webhookAction)
    {
        await Task.CompletedTask; //This method supports async/await for completeness' sake but try to avoid it being a too expensive check as triggers are evaluated a lot!

        //Your Logic if the trigger is met

        var eventType = webhookAction.Type; //This is the event of the webhook
        WebhookActionData webhookActionData = webhookAction.Data; //This object has data about the event
        TrelloClient trelloClient = webhookAction.TrelloClient; //If you need the TrelloClient to access data from Trello to evaluate your event you have access to it here
        WebhookActionDisplay webhookActionDisplay = webhookAction.Display; //This one often can help you better understand what event happened more easier

        return false; //Return true if the trigger is met (aka continue to condition) or False if not met (stop processing)
    }
}
```

Inside the `IsTriggerMetAsync` you have access to the WebhookAction (aka the JSON given by the Webhook) + The Trello Client in the rare cases you need further data from the board (not normal or recommended for Triggers)

#### Real Examples
To better get a sense of how `Triggers` look are here links to the various real-time implementations for inspiration:
- Source Code: [CardCreatedTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/CardCreatedTrigger.cs)
- Source Code: [CardEmailedTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/CardEmailedTrigger.cs)
- Source Code: [CardMovedToBoardTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/CardMovedToListTrigger.cs)
- Source Code: [CardMovedToListTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/CardMovedToListTrigger.cs)
- Source Code: [CardUpdatedTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/CardUpdatedTrigger.cs)
- Source Code: [CheckItemStateUpdatedOnCardTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/CheckItemStateUpdatedOnCardTrigger.cs)
- Source Code: [LabelAddedToCardTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/LabelAddedToCardTrigger.cs) |
- Source Code: [LabelRemovedFromCardTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/LabelRemovedFromCardTrigger.cs) |
- Source Code: [MemberAddedToCardTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/MemberAddedToCardTrigger.cs) |
- Source Code: [MemberRemovedFromCardTrigger](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Triggers/MemberRemovedFromCardTrigger.cs)



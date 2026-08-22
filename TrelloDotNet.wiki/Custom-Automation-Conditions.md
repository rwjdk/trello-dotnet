The [Automation Engine](Automation-Engine) has various [out-of-the box Conditions](Automation-Engine#conditions), but there can be  cases where you wish to make your own custom condition 

> If you think the Condition you need so be an out-of-the-box Condition feel free to [open an issue](https://github.com/rwjdk/trello-dotnet/issues) or [create a Pull Request](https://github.com/rwjdk/trello-dotnet/pulls) with your custom Condition

### Steps to make a custom Condition

1. Make a new C# class and Call it 'MyCustomCondition' (or what you like it to be called but it is recommended to suffix it 'Condition')
2. Let the class implement interface `IAutomationCondition`
3. Let the class implement the mandatory member `IsConditionMetAsync`

#### Basic Example
``` cs

//Your custom Condition
public class MyCustomCondition: IAutomationCondition
{
    public async Task<bool> IsConditionMetAsync(WebhookAction webhookAction)
    {
        //Your Logic if the Condition is met

        WebhookActionData webhookActionData = webhookAction.Data; //This object has data about the event
        TrelloClient trelloClient = webhookAction.TrelloClient; //If you need the TrelloClient to access data from Trello to evaluate your event you have access to it here

        return false; //Return true if the Condition is met (aka continue to Actions) or False if not met (stop processing)
    }
}
```

Inside the `IsConditionMetAsync` you have access to the WebhookAction (aka the JSON given by the Webhook) + The TrelloClient for further data lookups if needed

#### Real Examples
To better get a sense of how `Conditions` look are here links to the various real-time implementations for inspiration:
- Source Code: [CardCoverCondition](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Conditions/CardCoverCondition.cs)
- Source Code: [CardFieldCondition](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Conditions/CardFieldCondition.cs)
- Source Code: [ChecklistItemsCompleteCondition](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Conditions/ChecklistItemsCompleteCondition.cs)
- Source Code: [ChecklistItemsIncompleteCondition](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Conditions/ChecklistItemsIncompleteCondition.cs)
- Source Code: [ChecklistIncompleteCondition](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Conditions/ChecklistIncompleteCondition.cs)
- Source Code: [ChecklistNotStartedCondition](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Conditions/ChecklistNotStartedCondition.cs)
- Source Code: [LabelCondition](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Conditions/LabelCondition.cs)
- Source Code: [ListCondition](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Conditions/ListCondition.cs)
- Source Code: [MemberCondition](https://github.com/rwjdk/trello-dotnet/blob/main/TrelloDotNet/TrelloDotNet/AutomationEngine/Model/Conditions/MemberCondition.cs)




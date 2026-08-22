With the Webhook Data Receiver, you are able to easily turn the JSON received from a Webhook into C# Objects and Events so you can process them.

![image](https://github.com/rwjdk/trello-dotnet/assets/7032102/7db2f1a6-b59f-43a9-b0cb-e945908bea1e)

> A more automated alternative to the Webhook Data Receiver is the [Automation Engine](Automation-Engine). For comparison see [this page](Automation-Engine-vs-WebHook-Data-Receiver-Comparison)

## How to set up
In order to get the Webhook Data Receiver working you need to do the following steps:
1. Get an [API Key and Token](https://youtu.be/ndLSAD3StH8)
2. Create a C# Based API Endpoint ([Azure Function](https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview?pivots=programming-language-csharp), [ASP.NET Core API](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-7.0), [Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-7.0), or similar) that can accept HTTP POST and HEAD messages and run on HTTPS
> TIP: For Local development use [NGROK](https://ngrok.com/) or Visual Studio [DevTunnels](https://learn.microsoft.com/en-us/connectors/custom-connectors/port-tunneling) for local development
3. Add NuGet package [TrelloDotNet](https://www.nuget.org/packages/TrelloDotNet) to the project
4. Add the following basic code for now to the Endpoint (Code is based on Azure Function but adjust accordingly to get the JSON)

``` cs
public class WebhookReceiversample
{
    [Function("WebhookReceiverSample")]
    // ReSharper disable once UnusedMember.Global
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", "head")] HttpRequestData req)
    {
        var trelloClientHelper = new TrelloClientController();
        try
        {
            //Get The raw JSON from the webhook and process it
            using var streamReader = new StreamReader(req.Body);
            var json = await streamReader.ReadToEndAsync(); //JSON from a Board Webhook

            //Get a configured Trello client
            var trelloClient = trelloClientHelper.GetTrelloClient();

            var webhookDataReceiver = new TrelloDotNet.WebhookDataReceiver(trelloClient);

            //Option 1: Use code to parse the event yourself
            WebhookNotificationBoard webhookBoard = webhookDataReceiver.ConvertJsonToWebhookNotificationBoard(json); //Alternative you can get data from a list of a card if that is what you subscribed to in your Webhook

            Board board = webhookBoard.Board; //Info on the source-board
            WebhookAction action = webhookBoard.Action; //The Webhook-action (generic if webhook is Board, List or Card)
            string actionType = action.Type; //The type of event (Example: 'UpdateCard' or 'AddLabelToCard')
            WebhookActionData webhookActionData = action.Data; //Data about the event (aka what board, list, card, etc. was involved)
            WebhookActionDisplay webhookActionDisplay = action.Display; //Display name of the Action (can sometime help better understand the event)
            Member actionMemberCreator = action.MemberCreator; //The member who did the action

            //Todo: Use above to react and do what you need to do

            //--------------------------------------------------------------------------------------------------------------
            //Option 2: Let the Webhook Receiver send you C# Event based on the incoming data

            //You can subscribe to basic raw events (there are over 70 of these)
            webhookDataReceiver.BasicEvents.OnUpdateCard += BasicEvents_OnUpdateCard;
            webhookDataReceiver.BasicEvents.OnAddLabelToCard += BasicEvents_OnAddLabelToCard;

            //Or you can subscribe to more curated events (Few but common events people need)
            webhookDataReceiver.SmartEvents.OnCardMovedToNewList += SmartEvents_OnCardMovedToNewList;
            webhookDataReceiver.SmartEvents.OnLabelAddedToCard += SmartEvents_OnLabelAddedToCard;

        }
        catch (Exception e)
        {
            //todo - deal with exceptions (error mail or the like) and potential retry
        }

        return req.CreateResponse(HttpStatusCode.OK);//This is still OK as Trello should not see exception as the receiver 
    }

    private void SmartEvents_OnLabelAddedToCard(WebhookSmartEventLabelAdded args)
    {
        var labelId = args.AddedLabelId;
        var labelName = args.AddedLabelName;
        var cardId = args.CardId;

        //Todo - React and implement Logic
    }

    private void SmartEvents_OnCardMovedToNewList(WebhookSmartEventCardMovedToNewList args)
    {
        var newListId = args.NewListId;
        var newListName = args.NewListName;
        var oldListId = args.OldListId;
        var oldListName = args.OldListName;
        var cardId = args.CardId;

        //Todo - React and implement Logic
    }

    private void BasicEvents_OnAddLabelToCard(WebhookAction args)
    {
        WebhookActionData actionData = args.Data; //Data about event
        //Todo - React and implement Logic
    }

    private void BasicEvents_OnUpdateCard(WebhookAction args)
    {
        WebhookActionData actionData = args.Data; //Data about event
        //Todo - React and implement Logic
    }
}
```
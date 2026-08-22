using System;
using System.Text;
using System.Text.Json.Serialization;

namespace TrelloDotNet.Model.Webhook
{
    /// <summary>
    /// The Action of the Webhook
    /// </summary>
    public class WebhookAction
    {
        /// <summary>
        /// Id of the Action
        /// </summary>
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; private set; }

        /// <summary>
        /// The Id of the Member (User) that did the action
        /// </summary>
        [JsonPropertyName("idMemberCreator")]
        [JsonInclude]
        public string MemberCreatorId { get; private set; }

        /// <summary>
        /// Type of the Action (example Update of a Card)
        /// </summary>
        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; private set; }

        /// <summary>
        /// Date of the Action
        /// </summary>
        [JsonPropertyName("date")]
        [JsonInclude]
        public DateTimeOffset Date { get; private set; }

        /// <summary>
        /// The Id of the Member (User) that did the action
        /// </summary>
        [JsonPropertyName("memberCreator")]
        [JsonInclude]
        public Member MemberCreator { get; private set; }

        /// <summary>
        /// Data about the Action
        /// </summary>
        [JsonPropertyName("data")]
        [JsonInclude]
        public WebhookActionData Data { get; private set; }

        /// <summary>
        /// Display of the Action
        /// </summary>
        [JsonPropertyName("display")]
        [JsonInclude]
        public WebhookActionDisplay Display { get; private set; }

        /// <summary>
        /// Trello Client
        /// </summary>
        public TrelloClient TrelloClient { get; internal set; }

        internal static WebhookAction CreateDummy(TrelloClient trelloClient, WebhookActionDummyCreationScenario scenario, Board boardToSimulate = null, List listToSimulate = null, Card cardToSimulate = null)
        {
            WebhookAction webhookAction = new WebhookAction()
            {
                TrelloClient = trelloClient,
                Data = WebhookActionData.CreateDummy(scenario, cardToSimulate, listToSimulate, boardToSimulate),
                Date = new DateTimeOffset(new DateTime(2000, 12, 1)),
                Display = WebhookActionDisplay.CreateDummy(scenario),
                Id = "63d128787441d05619f44dbe",
                MemberCreator = Member.CreateDummy(),
                MemberCreatorId = "63d1239e857afaa8b003c633",
                Type = GetTypeFromScenario(scenario),
            };
            webhookAction.Data.Parent = webhookAction;
            return webhookAction;
        }

        private static string GetTypeFromScenario(WebhookActionDummyCreationScenario scenario)
        {
            switch (scenario)
            {
                case WebhookActionDummyCreationScenario.MoveCardToList:
                case WebhookActionDummyCreationScenario.MoveCardAwayFromList:
                case WebhookActionDummyCreationScenario.CardUpdated:
                    return WebhookActionTypes.UpdateCard;
                case WebhookActionDummyCreationScenario.CardCreated:
                    return WebhookActionTypes.CreateCard;
                case WebhookActionDummyCreationScenario.CardEmailed:
                    return WebhookActionTypes.EmailCard;
                case WebhookActionDummyCreationScenario.CheckItemStateUpdated:
                    return WebhookActionTypes.UpdateCheckItemStateOnCard;
                case WebhookActionDummyCreationScenario.CardMovedToBoard:
                    return WebhookActionTypes.MoveCardToBoard;
                case WebhookActionDummyCreationScenario.LabelAddedToCard:
                    return WebhookActionTypes.AddLabelToCard;
                case WebhookActionDummyCreationScenario.MemberAddedToCard:
                    return WebhookActionTypes.AddMemberToCard;
                case WebhookActionDummyCreationScenario.LabelRemovedFromCard:
                    return WebhookActionTypes.RemoveLabelFromCard;
                case WebhookActionDummyCreationScenario.MemberRemovedFromCard:
                    return WebhookActionTypes.RemoveMemberFromCard;
                case WebhookActionDummyCreationScenario.BoardUpdated:
                    return WebhookActionTypes.UpdateBoard;
                case WebhookActionDummyCreationScenario.ListUpdated:
                    return WebhookActionTypes.UpdateList;
            }

            return WebhookActionTypes.UpdateCard;
        }

        internal enum WebhookActionDummyCreationScenario
        {
            MoveCardToList,
            NoListAfter,
            NoListBefore,
            CardCreated,
            CardEmailed,
            CardMovedToBoard,
            CardUpdated,
            LabelAddedToCard,
            MemberAddedToCard,
            LabelRemovedFromCard,
            MemberRemovedFromCard,
            BoardUpdated,
            ListUpdated,
            MoveCardAwayFromList,
            CheckItemStateUpdated
        }

        internal string SummarizeEvent()
        {
            StringBuilder context = new StringBuilder();
            context.AppendLine();
            context.AppendLine("Event-Context:");
            context.AppendLine($"- Type: '{Type}'");
            context.AppendLine($"- TranslationKey: '{Display?.TranslationKey}'");
            if (Data?.Board != null)
            {
                context.AppendLine($"- Data > Board: '{Data.Board.Id} - {Data.Board.Name}'");
            }

            if (Data?.List != null)
            {
                context.AppendLine($"- Data > List: '{Data.List.Id} - {Data?.List.Name}'");
            }

            if (Data?.Card != null)
            {
                context.AppendLine($"- Data > Card: '{Data.Card.Id} - {Data.Card.Name}'");
            }

            return context.ToString();
        }
    }
}
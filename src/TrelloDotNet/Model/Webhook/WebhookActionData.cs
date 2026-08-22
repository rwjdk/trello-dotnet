using System.Text.Json.Serialization;
using TrelloDotNet.Control;

namespace TrelloDotNet.Model.Webhook
{
    /// <summary>
    /// Data of a Webhook Action
    /// </summary>
    public class WebhookActionData
    {
        /// <summary>
        /// List Data Object
        /// </summary>
        [JsonPropertyName("list")]
        [JsonInclude]
        public WebhookActionDataList List { get; private set; }

        /// <summary>
        /// Board Data Object
        /// </summary>
        [JsonPropertyName("board")]
        [JsonInclude]
        public WebhookActionDataBoard Board { get; private set; }

        /// <summary>
        /// Card Data Object
        /// </summary>
        [JsonPropertyName("card")]
        [JsonInclude]
        public WebhookActionDataCard Card { get; private set; }

        /// <summary>
        /// Label Data Object
        /// </summary>
        [JsonPropertyName("label")]
        [JsonInclude]
        public WebhookActionDataLabel Label { get; private set; }

        /// <summary>
        /// Checklist Data Object
        /// </summary>
        [JsonPropertyName("checklist")]
        [JsonInclude]
        public WebhookActionDataChecklist Checklist { get; private set; }

        /// <summary>
        /// CheckItem Data Object
        /// </summary>
        [JsonPropertyName("checkItem")]
        [JsonInclude]
        public WebhookActionDataCheckItem CheckItem { get; private set; }

        /// <summary>
        /// Before List
        /// </summary>
        [JsonPropertyName("listBefore")]
        [JsonInclude]
        public WebhookActionDataList ListBefore { get; private set; }

        /// <summary>
        /// After List
        /// </summary>
        [JsonPropertyName("listAfter")]
        [JsonInclude]
        public WebhookActionDataList ListAfter { get; private set; }

        /// <summary>
        /// Old
        /// </summary>
        [JsonPropertyName("old")]
        [JsonInclude]
        public WebhookActionDataOld Old { get; private set; }

        /// <summary>
        /// Organization Data
        /// </summary>
        [JsonPropertyName("organization")]
        [JsonInclude]
        public WebhookActionDataOrganization Organization { get; private set; }

        /// <summary>
        /// Member
        /// </summary>
        [JsonPropertyName("member")]
        [JsonInclude]
        public WebhookActionDataMember Member { get; set; }

        /// <summary>
        /// The Type of added member (Only there when event is 'addMemberToBoard')
        /// </summary>
        [JsonPropertyName("memberType")]
        [JsonInclude]
        [JsonConverter(typeof(EnumViaJsonPropertyConverter<MembershipType>))]
        public MembershipType MemberType { get; private set; }

        /// <summary>
        /// The Id of an Added Member (Only there when event is 'addMemberToBoard')
        /// </summary>
        [JsonPropertyName("idMemberAdded")]
        [JsonInclude]
        public string MemberIdAdded { get; private set; }

        /// <summary>
        /// BoardTarget of the Action (Only there when event is 'moveCardFromBoard')
        /// </summary>
        [JsonPropertyName("boardTarget")]
        [JsonInclude]
        public WebhookActionDataBoard BoardTarget { get; private set; }

        /// <summary>
        /// BoardSource of the Action (Only there when event is 'moveCardToBoard')
        /// </summary>
        [JsonPropertyName("boardSource")]
        [JsonInclude]
        public WebhookActionDataBoard BoardSource { get; private set; }

        /// <summary>
        /// Attachment Data Object
        /// </summary>
        [JsonPropertyName("attachment")]
        [JsonInclude]
        public WebhookActionDataAttachment Attachment { get; private set; }

        /// <summary>
        /// Plugin of the Action (Only there when event is 'enablePlugin' or 'disablePlugin')
        /// </summary>
        [JsonPropertyName("plugin")]
        [JsonInclude]
        public WebhookActionDataPlugin Plugin { get; private set; }

        /// <summary>
        /// Parent
        /// </summary>
        public WebhookAction Parent { get; internal set; }

        internal static WebhookActionData CreateDummy(WebhookAction.WebhookActionDummyCreationScenario webhookActionDummyCreationScenario, Card cardToSimulate, List listToSimulate, Board boardToSimulate)
        {
            WebhookActionDataList listAfter = WebhookActionDataList.CreateDummy();
            WebhookActionDataList listBefore = WebhookActionDataList.CreateDummy();
            WebhookActionDataCard card = WebhookActionDataCard.CreateDummy(cardToSimulate);
            WebhookActionDataBoard board = WebhookActionDataBoard.CreateDummy(boardToSimulate);
            WebhookActionDataMember member = WebhookActionDataMember.CreateDummy();
            WebhookActionDataCheckItem checkItem = WebhookActionDataCheckItem.CreateDummy();
            WebhookActionDataChecklist checklist = WebhookActionDataChecklist.CreateDummy();
            WebhookActionDataLabel label = WebhookActionDataLabel.CreateDummy();
            WebhookActionDataList list = WebhookActionDataList.CreateDummy(listToSimulate);

            switch (webhookActionDummyCreationScenario)
            {
                case WebhookAction.WebhookActionDummyCreationScenario.CardCreated:
                case WebhookAction.WebhookActionDummyCreationScenario.CardUpdated:
                    list = null;
                    break;
                case WebhookAction.WebhookActionDummyCreationScenario.MoveCardToList:
                case WebhookAction.WebhookActionDummyCreationScenario.MoveCardAwayFromList:
                    member = null;
                    checkItem = null;
                    checklist = null;
                    label = null;
                    list = null;
                    break;
                case WebhookAction.WebhookActionDummyCreationScenario.NoListAfter:
                    listAfter = null;
                    break;
                case WebhookAction.WebhookActionDummyCreationScenario.NoListBefore:
                    listBefore = null;
                    break;
                case WebhookAction.WebhookActionDummyCreationScenario.BoardUpdated:
                    card = null;
                    list = null;
                    break;
            }

            WebhookActionData webhookActionData = new WebhookActionData()
            {
                Card = card,
                Board = board,
                Member = member,
                CheckItem = checkItem,
                Checklist = checklist,
                Label = label,
                List = list,
                ListAfter = listAfter,
                ListBefore = listBefore,
                Old = WebhookActionDataOld.CreateDummy(),
            };
            if (webhookActionData.Member != null)
            {
                webhookActionData.Member.Parent = webhookActionData;
            }

            if (webhookActionData.Card != null)
            {
                webhookActionData.Card.Parent = webhookActionData;
            }

            if (webhookActionData.Board != null)
            {
                webhookActionData.Board.Parent = webhookActionData;
            }

            if (webhookActionData.Checklist != null)
            {
                webhookActionData.Checklist.Parent = webhookActionData;
            }

            if (webhookActionData.ListBefore != null)
            {
                webhookActionData.ListBefore.Parent = webhookActionData;
            }

            if (webhookActionData.ListAfter != null)
            {
                webhookActionData.ListAfter.Parent = webhookActionData;
            }

            if (webhookActionData.List != null)
            {
                webhookActionData.List.Parent = webhookActionData;
            }

            return webhookActionData;
        }
    }
}
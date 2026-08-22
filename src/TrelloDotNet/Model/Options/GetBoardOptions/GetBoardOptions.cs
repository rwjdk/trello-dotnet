using System;
using System.Collections.Generic;
using System.Linq;
using TrelloDotNet.Control;

namespace TrelloDotNet.Model.Options.GetBoardOptions
{
    /// <summary>
    /// Options on how and what should be included on the Boards (Example only a few fields to increase performance or more nested data to avoid more API calls)
    /// </summary>
    public class GetBoardOptions
    {
        /// <summary>
        /// Provide one or more action types there (TrelloDotNet.Model.Webhook.WebhookActionTypes) to get them included with the Board
        /// </summary>
        public ActionTypesToInclude ActionsTypes { get; set; }

        /// <summary>
        /// Filter of various Boards (Default: All)
        /// </summary>
        public GetBoardOptionsTypesOfBoardsToInclude TypesOfBoardsToInclude { get; set; } = GetBoardOptionsTypesOfBoardsToInclude.All;

        /// <summary>
        /// Whether to return Cards on the board (Default: None) [NOTE: This option only works when retrieving a single board. On methods 'GetBoardsForMemberAsync', 'GetBoardsCurrentTokenCanAccessAsync' and 'GetBoardsInOrganization' it have no effect]
        /// </summary>
        public GetBoardOptionsIncludeCards IncludeCards { get; set; }

        /// <summary>
        /// Whether to return Lists on the board (Default: None)
        /// </summary>
        public GetBoardOptionsIncludeLists IncludeLists { get; set; }

        /// <summary>
        /// Whether to return Labels on the board (Default: False)
        /// </summary>
        public bool IncludeLabels { get; set; }

        /// <summary>
        /// What board-fields to include if IncludeBoard are set to True
        /// </summary>
        public BoardFields BoardFields { get; set; }

        /// <summary>
        /// What card-fields to include if IncludeCards are set to other than None. all or a comma-separated list of fields. Defaults: badges, checkItemStates, closed, dateLastActivity, desc, descData, due, start, email, idBoard, idChecklists, idLabels, idList, idMembers, idShort, idAttachmentCover, manualCoverAttachment, labels, name, pos, shortUrl, url
        /// </summary>
        public CardFields CardFields { get; set; }

        /// <summary>
        /// Whether to return Plugin object of the card (Default: False)
        /// </summary>
        public bool IncludePluginData { get; set; }

        /// <summary>
        /// What types of boards are returned (Default: All)
        /// </summary> 
        public GetBoardOptionsFilter Filter { get; set; } = GetBoardOptionsFilter.All;

        /// <summary>
        /// Additional Parameters not supported out-of-the-box
        /// </summary>
        public List<QueryParameter> AdditionalParameters { get; set; } = new List<QueryParameter>();

        internal QueryParameter[] GetParameters()
        {
            List<QueryParameter> parameters = new List<QueryParameter>();

            if (BoardFields != null)
            {
                parameters.Add(new QueryParameter("fields", string.Join(",", BoardFields.Fields)));
            }

            if (CardFields != null)
            {
                parameters.Add(new QueryParameter("card_fields", string.Join(",", CardFields.Fields)));
            }

            if (ActionsTypes != null)
            {
                parameters.Add(new QueryParameter("actions", string.Join(",", ActionsTypes.ActionTypes)));
            }

            if (TypesOfBoardsToInclude != GetBoardOptionsTypesOfBoardsToInclude.All)
            {
                Enum[] selected = Enum.GetValues(TypesOfBoardsToInclude.GetType()).Cast<Enum>().Where(TypesOfBoardsToInclude.HasFlag).ToArray();

                parameters.Add(new QueryParameter("filter", string.Join(",", selected.Select(x => x.GetJsonPropertyName()))));
            }

            parameters.Add(new QueryParameter("lists", IncludeLists.GetJsonPropertyName()));
            parameters.Add(new QueryParameter("filter", Filter.GetJsonPropertyName()));
            parameters.Add(new QueryParameter("cards", IncludeCards.GetJsonPropertyName()));
            parameters.Add(new QueryParameter("labels", IncludeLabels ? "all" : "none"));
            parameters.Add(new QueryParameter("pluginData", IncludePluginData));
            parameters.AddRange(AdditionalParameters);

            return parameters.ToArray();
        }
    }
}
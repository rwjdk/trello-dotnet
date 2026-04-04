using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace TrelloDotNet.Model
{
    /// <summary>
    /// Object representing your Trello Inbox
    /// </summary>
    [DebuggerDisplay("BoardId = {BoardId}, ListId = {ListId}, OrganizationId = {OrganizationId}")]
    public class TokenMemberInbox
    {
        /// <summary>
        /// "Board" Id of your Inbox
        /// </summary>
        [JsonPropertyName(Constants.TrelloIds.BoardFields.IdBoard)]
        [JsonInclude]
        public string BoardId { get; set; }

        /// <summary>
        /// "List" Id of your Inbox
        /// </summary>
        [JsonPropertyName(Constants.TrelloIds.ListFields.IdList)]
        [JsonInclude]
        public string ListId { get; set; }

        /// <summary>
        /// "Organization" Id of your Inbox
        /// </summary>
        [JsonPropertyName(Constants.TrelloIds.OrganizationFields.IdOrganization)]
        [JsonInclude]
        public string OrganizationId { get; private set; }

        /// <summary>
        /// Labels of the Inbox
        /// </summary>
        [JsonIgnore] public List<Label> Labels { get; internal set; }
    }
}




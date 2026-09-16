using System.Text.Json.Serialization;

namespace TrelloDotNet
{
    /// <summary>
    /// Permissions that can be requested from the Trello OAuth 2.0 authorization endpoint.
    /// </summary>
    public enum TrelloOAuth2Scope
    {
        /// <summary>
        /// Read Boards, Cards, Lists, and comments.
        /// </summary>
        [JsonPropertyName("read:board:trello")]
        ReadBoard,

        /// <summary>
        /// Create and update Boards, Cards, Lists, and comments.
        /// </summary>
        [JsonPropertyName("write:board:trello")]
        WriteBoard,

        /// <summary>
        /// Add, remove, or modify Board memberships.
        /// </summary>
        [JsonPropertyName("write:board:membership:trello")]
        WriteBoardMembership,

        /// <summary>
        /// Read workspaces.
        /// </summary>
        [JsonPropertyName("read:organization:trello")]
        ReadOrganization,

        /// <summary>
        /// Update workspaces.
        /// </summary>
        [JsonPropertyName("write:organization:trello")]
        WriteOrganization,

        /// <summary>
        /// Add, remove, or modify workspace memberships.
        /// </summary>
        [JsonPropertyName("write:organization:membership:trello")]
        WriteOrganizationMembership,

        /// <summary>
        /// Read Member details and memberships.
        /// </summary>
        [JsonPropertyName("read:member:trello")]
        ReadMember,

        /// <summary>
        /// Update supported Member resources.
        /// </summary>
        [JsonPropertyName("write:member:trello")]
        WriteMember,

        /// <summary>
        /// Read enterprises.
        /// </summary>
        [JsonPropertyName("read:enterprise:trello")]
        ReadEnterprise,

        /// <summary>
        /// Update and manage enterprises.
        /// </summary>
        [JsonPropertyName("write:enterprise:trello")]
        WriteEnterprise
    }
}

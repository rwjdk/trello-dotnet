// ReSharper disable MemberHidesStaticFromOuterClass
namespace TrelloDotNet
{
    /// <summary>
    /// Parts of the API URL
    /// </summary>
    public struct UrlPaths
    {
        /// <summary>
        /// boards
        /// </summary>
        public const string Boards = "boards";

        /// <summary>
        /// cards
        /// </summary>
        public const string Cards = "cards";

        /// <summary>
        /// lists
        /// </summary>
        public const string Lists = "lists";

        /// <summary>
        /// checklists
        /// </summary>
        public const string Checklists = "checklists";

        /// <summary>
        /// checkItems
        /// </summary>
        public const string CheckItems = "checkItems";

        /// <summary>
        /// members
        /// </summary>
        public const string Members = "members";

        /// <summary>
        /// labels
        /// </summary>
        public const string Labels = "labels";

        /// <summary>
        /// webhooks
        /// </summary>
        public const string Webhooks = "webhooks";

        /// <summary>
        /// tokens
        /// </summary>
        public const string Tokens = "tokens";

        /// <summary>
        /// actions
        /// </summary>
        public const string Actions = "actions";

        /// <summary>
        /// customFields
        /// </summary>
        public const string CustomFields = "customFields";

        /// <summary>
        /// customFieldItems
        /// </summary>
        public const string CustomFieldItems = "customFieldItems";

        /// <summary>
        /// organizations
        /// </summary>
        public const string Organizations = "organizations";

        /// <summary>
        /// search
        /// </summary>
        public const string Search = "search";
    }

#pragma warning disable CS1591
    public static class Constants
    {
        public static class TrelloIds
        {
            public static class ActionFields
            {
                public const string Action = "action";
                public const string Actions = "actions";
                public const string Address = "address";
                public const string Author = "author";
                public const string Coordinates = "coordinates";
                public const string Data = "data";
                public const string Date = "date";
                public const string DefaultLabels = "defaultLabels";
                public const string DefaultLists = "defaultLists";
                public const string Desc = "desc";
                public const string Display = "display";
                public const string DisplayName = "displayName";
                public const string Emoji = "emoji";
                public const string Id = "id";
                public const string LocationName = "locationName";
                public const string MemberCreator = "memberCreator";
                public const string Message = "message";
                public const string Name = "name";
                public const string Old = "old";
                public const string ShortLink = "shortLink";
                public const string StatusCode = "statusCode";
                public const string Text = "text";
                public const string TranslationKey = "translationKey";
                public const string Type = "type";
                public const string Url = "url";
                public const string Value200 = "200";
            }

            public static class AttachmentFields
            {
                public const string Attachment = "attachment";
                public const string Attachments = "attachments";
                public const string Bytes = "bytes";
                public const string EdgeColor = "edgeColor";
                public const string FileName = "fileName";
                public const string Id = "id";
                public const string IsUpload = "isUpload";
                public const string MimeType = "mimeType";
                public const string Name = "name";
                public const string Url = "url";
            }

            public static class BoardFields
            {
                public const string Admins = "admins";
                public const string All = "all";
                public const string Board = "board";
                public const string Boards = "boards";
                public const string BoardSource = "boardSource";
                public const string BoardTarget = "boardTarget";
                public const string CardAging = "cardAging";
                public const string CardCovers = "cardCovers";
                public const string Closed = "closed";
                public const string Comments = "comments";
                public const string Desc = "desc";
                public const string Disabled = "disabled";
                public const string FalseValue = "false";
                public const string HideVotes = "hideVotes";
                public const string Id = "id";
                public const string IdBoard = "idBoard";
                public const string IdBoards = "idBoards";
                public const string IdEnterprise = "idEnterprise";
                public const string Invitations = "invitations";
                public const string Name = "name";
                public const string None = "none";
                public const string Observers = "observers";
                public const string Open = "open";
                public const string PermissionLevel = "permissionLevel";
                public const string Pinned = "pinned";
                public const string Pirate = "pirate";
                public const string Prefs = "prefs";
                public const string Private = "private";
                public const string Public = "public";
                public const string Regular = "regular";
                public const string SelfJoin = "selfJoin";
                public const string ShortUrl = "shortUrl";
                public const string ShowCompleteStatus = "showCompleteStatus";
                public const string Starred = "starred";
                public const string Subscribed = "subscribed";
                public const string TrueValue = "true";
                public const string Url = "url";
                public const string Visible = "visible";
                public const string Voting = "voting";
            }

            public static class CardFields
            {
                public const string All = "all";
                public const string Brightness = "brightness";
                public const string Card = "card";
                public const string CardFront = "cardFront";
                public const string CardRole = "cardRole";
                public const string Cards = "cards";
                public const string Closed = "closed";
                public const string Cover = "cover";
                public const string CustomFieldItems = "customFieldItems";
                public const string Dark = "dark";
                public const string DateLastActivity = "dateLastActivity";
                public const string Desc = "desc";
                public const string Due = "due";
                public const string DueComplete = "dueComplete";
                public const string DueReminder = "dueReminder";
                public const string Full = "full";
                public const string Id = "id";
                public const string IdAttachmentCover = "idAttachmentCover";
                public const string IdCard = "idCard";
                public const string IdChecklists = "idChecklists";
                public const string IdLabels = "idLabels";
                public const string IdMembers = "idMembers";
                public const string IdMembersVoted = "idMembersVoted";
                public const string IdShort = "idShort";
                public const string IdUploadedBackground = "idUploadedBackground";
                public const string Image = "image";
                public const string ImageUrl = "imageUrl";
                public const string IsTemplate = "isTemplate";
                public const string Latitude = "latitude";
                public const string Light = "light";
                public const string Longitude = "longitude";
                public const string MembersVoted = "membersVoted";
                public const string MirrorSourceId = "mirrorSourceId";
                public const string Name = "name";
                public const string Native = "native";
                public const string None = "none";
                public const string Normal = "normal";
                public const string NullValue = "null";
                public const string Open = "open";
                public const string Rotate = "rotate";
                public const string ShortName = "shortName";
                public const string ShortUrl = "shortUrl";
                public const string Size = "size";
                public const string Start = "start";
                public const string Stickers = "stickers";
                public const string Subscribed = "subscribed";
                public const string Unified = "unified";
                public const string Url = "url";
                public const string Visible = "visible";
            }

            public static class ChecklistFields
            {
                public const string Checkbox = "checkbox";
                public const string Checked = "checked";
                public const string CheckItem = "checkItem";
                public const string CheckItems = "checkItems";
                public const string Checklist = "checklist";
                public const string Checklists = "checklists";
                public const string Complete = "complete";
                public const string Id = "id";
                public const string IdChecklist = "idChecklist";
                public const string Incomplete = "incomplete";
                public const string Name = "name";
                public const string NullValue = "null";
                public const string State = "state";
            }

            public static class CustomFieldFields
            {
                public const string CustomFields = "customFields";
                public const string FieldGroup = "fieldGroup";
                public const string Id = "id";
                public const string IdCustomField = "idCustomField";
                public const string IdValue = "idValue";
                public const string IsSuggestedField = "isSuggestedField";
                public const string ModelType = "modelType";
                public const string Name = "name";
                public const string None = "none";
                public const string Number = "number";
                public const string Value = "value";
                public const string Position = "pos";
                public const string Color = "color";
            }


            public static class ColorFields
            {
                public const string Color = "color";
                public const string Black = "black";
                public const string BlackDark = "black_dark";
                public const string BlackLight = "black_light";
                public const string Blue = "blue";
                public const string BlueDark = "blue_dark";
                public const string BlueLight = "blue_light";
                public const string Green = "green";
                public const string GreenDark = "green_dark";
                public const string GreenLight = "green_light";
                public const string Yellow = "yellow";
                public const string YellowDark = "yellow_dark";
                public const string YellowLight = "yellow_light";
                public const string Orange = "orange";
                public const string OrangeDark = "orange_dark";
                public const string OrangeLight = "orange_light";
                public const string Red = "red";
                public const string RedDark = "red_dark";
                public const string RedLight = "red_light";
                public const string Purple = "purple";
                public const string PurpleDark = "purple_dark";
                public const string PurpleLight = "purple_light";
                public const string Sky = "sky";
                public const string SkyDark = "sky_dark";
                public const string SkyLight = "sky_light";
                public const string Lime = "lime";
                public const string LimeDark = "lime_dark";
                public const string LimeLight = "lime_light";
                public const string Pink = "pink";
                public const string PinkDark = "pink_dark";
                public const string PinkLight = "pink_light";
                public const string Gray = "gray";
                public const string Teal = "teal";
                public const string Magenta = "magenta";
            }
            public static class LabelFields
            {
                public const string Id = "id";
                public const string IdEmoji = "idEmoji";
                public const string Label = "label";
                public const string Labels = "labels";
                public const string Name = "name";
                public const string SkinVariation = "skinVariation";
            }

            public static class ListFields
            {
                public const string All = "all";
                public const string Bottom = "bottom";
                public const string Closed = "closed";
                public const string Id = "id";
                public const string IdList = "idList";
                public const string Left = "left";
                public const string List = "list";
                public const string ListAfter = "listAfter";
                public const string ListBefore = "listBefore";
                public const string Lists = "lists";
                public const string Name = "name";
                public const string Open = "open";
                public const string Pos = "pos";
                public const string SoftLimit = "softLimit";
                public const string Subscribed = "subscribed";
                public const string Top = "top";
                public const string ZIndex = "zIndex";
            }

            public static class MemberFields
            {
                public const string Admin = "admin";
                public const string AvatarUrl = "avatarUrl";
                public const string Confirmed = "confirmed";
                public const string DateLastActive = "dateLastActive";
                public const string DateLastImpression = "dateLastImpression";
                public const string Email = "email";
                public const string FullName = "fullName";
                public const string Ghost = "ghost";
                public const string Id = "id";
                public const string IdMember = "idMember";
                public const string IdMemberAdded = "idMemberAdded";
                public const string IdMemberCreator = "idMemberCreator";
                public const string Initials = "initials";
                public const string Member = "member";
                public const string Members = "members";
                public const string Memberships = "memberships";
                public const string MemberType = "memberType";
                public const string Normal = "normal";
                public const string Observer = "observer";
                public const string Unconfirmed = "unconfirmed";
                public const string Username = "username";
            }

            public static class OrganizationFields
            {
                public const string Desc = "desc";
                public const string DisplayName = "displayName";
                public const string Id = "id";
                public const string IdOrganization = "idOrganization";
                public const string IdOrganizationOwner = "idOrganizationOwner";
                public const string Name = "name";
                public const string Org = "org";
                public const string Organization = "organization";
                public const string Organizations = "organizations";
                public const string PremiumFeatures = "premiumFeatures";
                public const string PrivacyUrl = "privacyUrl";
                public const string SupportEmail = "supportEmail";
                public const string Url = "url";
                public const string Website = "website";
            }

            public static class PluginFields
            {
                public const string Capabilities = "capabilities";
                public const string Categories = "categories";
                public const string DateLastUpdated = "dateLastUpdated";
                public const string Id = "id";
                public const string IdPlugin = "idPlugin";
                public const string IframeConnectorUrl = "iframeConnectorUrl";
                public const string Name = "name";
                public const string Plugin = "plugin";
                public const string PluginData = "pluginData";
                public const string Public = "public";
            }

            public static class SearchFields
            {
                public const string Model = "model";
                public const string ModelTypes = "modelTypes";
                public const string Options = "options";
                public const string Partial = "partial";
                public const string Terms = "terms";
            }

            public static class TokenFields
            {
                public const string Access = "access";
                public const string DateCreated = "dateCreated";
                public const string DateExpires = "dateExpires";
                public const string Id = "id";
                public const string Identifier = "identifier";
                public const string IdModel = "idModel";
                public const string Inbox = "inbox";
                public const string ModelType = "modelType";
                public const string Permissions = "permissions";
                public const string Read = "read";
                public const string Scope = "scope";
                public const string Write = "write";
            }

            public static class WebhookFields
            {
                public const string Active = "active";
                public const string Address = "address";
                public const string CallbackUrl = "callbackURL";
                public const string ConsecutiveFailures = "consecutiveFailures";
                public const string Coordinates = "coordinates";
                public const string Deactivated = "deactivated";
                public const string Desc = "desc";
                public const string Description = "description";
                public const string DisplayName = "displayName";
                public const string FirstConsecutiveFailDate = "firstConsecutiveFailDate";
                public const string Id = "id";
                public const string LocationName = "locationName";
                public const string Name = "name";
                public const string ShortLink = "shortLink";
                public const string Url = "url";
                public const string Webhook = "webhook";
            }

            public static class QueryParameterNames
            {
                public const string Actions = "actions";
                public const string AllowBillableGuest = "allowBillableGuest";
                public const string AttachmentFields = "attachment_fields";
                public const string Attachments = "attachments";
                public const string Before = "before";
                public const string Board = "board";
                public const string BoardFields = "board_fields";
                public const string BoardsLimit = "boards_limit";
                public const string CardFields = "card_fields";
                public const string CardRole = "cardRole";
                public const string Cards = "cards";
                public const string CardsLimit = "cards_limit";
                public const string CardsPage = "cards_page";
                public const string ChecklistFields = "checklist_fields";
                public const string Checklists = "checklists";
                public const string Closed = "closed";
                public const string Cover = "cover";
                public const string CustomFieldItems = "customFieldItems";
                public const string Email = "email";
                public const string Fields = "fields";
                public const string Filter = "filter";
                public const string IdBoard = "idBoard";
                public const string IdBoards = "idBoards";
                public const string IdCards = "idCards";
                public const string IdCardSource = "idCardSource";
                public const string IdChecklistSource = "idChecklistSource";
                public const string IdList = "idList";
                public const string IdOrganization = "idOrganization";
                public const string IdOrganizations = "idOrganizations";
                public const string IsTemplate = "isTemplate";
                public const string KeepFromSource = "keepFromSource";
                public const string Labels = "labels";
                public const string Limit = "limit";
                public const string List = "list";
                public const string Lists = "lists";
                public const string MemberFields = "member_fields";
                public const string Members = "members";
                public const string MembersVoted = "membersVoted";
                public const string MemberVotedFields = "memberVoted_fields";
                public const string ModelTypes = "modelTypes";
                public const string Name = "name";
                public const string OnlyOrgMembers = "onlyOrgMembers";
                public const string Organization = "organization";
                public const string OrganizationFields = "organization_fields";
                public const string Page = "page";
                public const string Partial = "partial";
                public const string PluginData = "pluginData";
                public const string Pos = "pos";
                public const string Query = "query";
                public const string SetCover = "setCover";
                public const string Since = "since";
                public const string Sort = "sort";
                public const string StickerFields = "sticker_fields";
                public const string Stickers = "stickers";
                public const string Text = "text";
                public const string Type = "type";
                public const string Url = "url";
                public const string Urls = "urls";
                public const string Value = "value";
            }
        }
    }
#pragma warning restore CS1591
}






using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace TrelloDotNet.Model
{
    /// <summary>
    /// Represent an Emoji
    /// </summary>
    [DebuggerDisplay("Name = {Name}, ShortName = {ShortName}")]
    public class Emoji
    {
        /// <summary>
        /// Unified Id of Emoji
        /// </summary>
        [JsonPropertyName(Constants.TrelloIds.CardFields.Unified)]
        [JsonInclude]
        public string UnifiedId { get; set; }

        /// <summary>
        /// The Native value (aka the visual emoji)
        /// </summary>
        [JsonPropertyName(Constants.TrelloIds.CardFields.Native)]
        [JsonInclude]
        public string Native { get; set; }

        /// <summary>
        /// Name of the Emoji
        /// </summary>
        [JsonPropertyName(Constants.TrelloIds.CardFields.Name)]
        [JsonInclude]
        public string Name { get; set; }

        /// <summary>
        /// The Skin-variation of the Emoji
        /// </summary>
        [JsonPropertyName(Constants.TrelloIds.LabelFields.SkinVariation)]
        [JsonInclude]
        public string SkinVariation { get; set; }

        /// <summary>
        /// ShortName of the Emoji
        /// </summary>
        [JsonPropertyName(Constants.TrelloIds.CardFields.ShortName)]
        [JsonInclude]
        public string ShortName { get; set; }

        /// <summary>
        /// Alternative short names of the Emoji
        /// </summary>
        [JsonPropertyName("shortNames")]
        [JsonInclude]
        public List<string> ShortNames { get; set; }

        /// <summary>
        /// Primary text representation of the Emoji
        /// </summary>
        [JsonPropertyName("text")]
        [JsonInclude]
        public string Text { get; set; }

        /// <summary>
        /// Alternative text representations of the Emoji
        /// </summary>
        [JsonPropertyName("texts")]
        [JsonInclude]
        public List<string> Texts { get; set; }

        /// <summary>
        /// Category of the Emoji
        /// </summary>
        [JsonPropertyName("category")]
        [JsonInclude]
        public string Category { get; set; }

        /// <summary>
        /// Horizontal position of the Emoji in a sprite sheet
        /// </summary>
        [JsonPropertyName("sheetX")]
        [JsonInclude]
        public int SheetX { get; set; }

        /// <summary>
        /// Vertical position of the Emoji in a sprite sheet
        /// </summary>
        [JsonPropertyName("sheetY")]
        [JsonInclude]
        public int SheetY { get; set; }

        /// <summary>
        /// Text-to-speech description of the Emoji
        /// </summary>
        [JsonPropertyName("tts")]
        [JsonInclude]
        public string TextToSpeech { get; set; }

        /// <summary>
        /// Search keywords associated with the Emoji
        /// </summary>
        [JsonPropertyName("keywords")]
        [JsonInclude]
        public List<string> Keywords { get; set; }
    }
}






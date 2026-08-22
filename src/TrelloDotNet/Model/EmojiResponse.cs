using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrelloDotNet.Model
{
    internal class EmojiResponse
    {
        [JsonPropertyName("trello")]
        [JsonInclude]
        public List<Emoji> Trello { get; set; }
    }
}

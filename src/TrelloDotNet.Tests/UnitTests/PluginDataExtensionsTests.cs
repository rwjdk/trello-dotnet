using System.Text.Json;
using System.Text.Json.Serialization;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class PluginDataExtensionsTests
{
    [Fact]
    public void CastPluginDataDeserializesValue()
    {
        PluginData pluginData = CreatePluginData("plugin1", """{"name":"Power Up","enabled":true}""");

        PluginDataValue value = pluginData.Cast<PluginDataValue>();

        Assert.Equal("Power Up", value.Name);
        Assert.True(value.Enabled);
    }

    [Fact]
    public void CastPluginDataReturnsDefaultForNullPluginData()
    {
        PluginData pluginData = null!;

        PluginDataValue value = pluginData.Cast<PluginDataValue>();

        Assert.Null(value);
    }

    [Fact]
    public void CastPluginDataListUsesMatchingPluginId()
    {
        List<PluginData> pluginData =
        [
            CreatePluginData("plugin1", """{"name":"First","enabled":false}"""),
            CreatePluginData("plugin2", """{"name":"Second","enabled":true}""")
        ];

        PluginDataValue value = pluginData.Cast<PluginDataValue>("plugin2");

        Assert.Equal("Second", value.Name);
        Assert.True(value.Enabled);
    }

    [Fact]
    public void CastPluginDataListReturnsDefaultWhenPluginIdIsMissing()
    {
        List<PluginData> pluginData =
        [
            CreatePluginData("plugin1", """{"name":"First","enabled":false}""")
        ];

        PluginDataValue value = pluginData.Cast<PluginDataValue>("plugin2");

        Assert.Null(value);
    }

    private static PluginData CreatePluginData(string pluginId, string value)
    {
        return JsonSerializer.Deserialize<PluginData>(JsonSerializer.Serialize(new PluginDataJson
        {
            PluginId = pluginId,
            Value = value
        }))!;
    }

    private sealed class PluginDataJson
    {
        [JsonPropertyName("idPlugin")]
        public string PluginId { get; set; } = "";

        [JsonPropertyName("value")]
        public string Value { get; set; } = "";
    }

    private sealed class PluginDataValue
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
    }
}

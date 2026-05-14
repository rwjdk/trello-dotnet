using TrelloDotNet.Control;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class EnumHelperTests
{
    [Fact]
    public void GetJsonPropertyNameReturnsAttributeValue()
    {
        string value = CardCoverSize.Full.GetJsonPropertyName();

        Assert.Equal("full", value);
    }

    [Fact]
    public void GetJsonPropertyNameReturnsNullWhenEnumValueHasNoAttribute()
    {
        string value = CustomFieldType.Unknown.GetJsonPropertyName();

        Assert.Null(value);
    }

    [Fact]
    public void GetColorInfoReturnsLabelColorMetadata()
    {
        LabelColorInfo colorInfo = LabelColor.Green.GetColorInfo();

        Assert.Equal("#164B35", colorInfo.TextHex);
        Assert.Equal("#4BCE97", colorInfo.BackgroundHex);
    }

    [Fact]
    public void GetColorInfoReturnsNullWhenEnumValueHasNoColorMetadata()
    {
        LabelColorInfo colorInfo = CardCoverSize.Full.GetColorInfo();

        Assert.Null(colorInfo);
    }
}

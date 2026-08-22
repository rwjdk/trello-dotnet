using System.Text.Json;
using System.Text;
using TrelloDotNet.Control;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class EnumViaJsonPropertyConverterTests
{
    [Fact]
    public void Read()
    {
        EnumViaJsonPropertyConverter<CardCoverColor> enumViaJsonPropertyConverter = new EnumViaJsonPropertyConverter<CardCoverColor>();
        byte[] bytes = "\"pink\""u8.ToArray();
        Utf8JsonReader stringReader = new Utf8JsonReader(bytes.AsSpan());
        stringReader.Read();
        CardCoverColor readColor = enumViaJsonPropertyConverter.Read(ref stringReader, typeof(CardCoverColor), JsonSerializerOptions.Default);
        Assert.Equal(CardCoverColor.Pink, readColor);
    }

    [Fact]
    public void ReadEmpty()
    {
        EnumViaJsonPropertyConverter<CardCoverColor> enumViaJsonPropertyConverter = new EnumViaJsonPropertyConverter<CardCoverColor>();
        byte[] bytes = "\"\""u8.ToArray();
        Utf8JsonReader stringReader = new Utf8JsonReader(bytes.AsSpan());
        stringReader.Read();
        CardCoverColor readColor = enumViaJsonPropertyConverter.Read(ref stringReader, typeof(CardCoverColor), JsonSerializerOptions.Default);
        Assert.Equal(CardCoverColor.None, readColor);
    }

    [Fact]
    public void ReadInvalid()
    {
        EnumViaJsonPropertyConverter<DateTimeKind> enumViaJsonPropertyConverter = new EnumViaJsonPropertyConverter<DateTimeKind>();
        byte[] bytes = "\"aaa\""u8.ToArray();
        Assert.Throws<Exception>(() =>
        {
            Utf8JsonReader stringReader = new Utf8JsonReader(bytes.AsSpan());
            stringReader.Read();
            return enumViaJsonPropertyConverter.Read(ref stringReader, typeof(DateTimeKind), JsonSerializerOptions.Default);
        });
    }

    [Fact]
    public void Write()
    {
        EnumViaJsonPropertyConverter<CardCoverColor> enumViaJsonPropertyConverter = new EnumViaJsonPropertyConverter<CardCoverColor>();
        MemoryStream memoryStream = new MemoryStream();
        Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(memoryStream);
        enumViaJsonPropertyConverter.Write(utf8JsonWriter, CardCoverColor.Pink, JsonSerializerOptions.Default);
        utf8JsonWriter.Flush();
        string json = Encoding.UTF8.GetString(memoryStream.ToArray());

        Assert.Equal("\"pink\"", json);
    }
}
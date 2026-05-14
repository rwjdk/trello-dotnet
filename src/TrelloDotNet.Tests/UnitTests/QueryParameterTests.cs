using TrelloDotNet.Control;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class QueryParameterTests
{
    [Fact]
    public void GetParametersAsStringFormatsDifferentValueTypesForApi()
    {
        QueryParameter[] parameters =
        [
            new QueryParameter("text", "Hello World"),
            new QueryParameter("items", ["a", "b"]),
            new QueryParameter("count", 42),
            new QueryParameter("price", 42.33M),
            new QueryParameter("enabled", true),
            new QueryParameter("when", new DateTimeOffset(2024, 9, 18, 12, 34, 56, TimeSpan.FromHours(2)))
        ];

        string value = ApiRequestController.GetParametersAsString(parameters).ToString();

        Assert.Equal("&text=Hello+World&items=a%2cb&count=42&price=42.33&enabled=true&when=2024-09-18T10:34:56.000Z", value);
    }

    [Fact]
    public void GetValueAsApiFormattedStringFormatsNullAsNull()
    {
        QueryParameter parameter = new QueryParameter("optional", (string)null!);

        string value = parameter.GetValueAsApiFormattedString();

        Assert.Equal("null", value);
    }

    [Fact]
    public void GetParametersAsStringAllowsNullOrEmptyParameterLists()
    {
        Assert.Equal("", ApiRequestController.GetParametersAsString(null!).ToString());
        Assert.Equal("", ApiRequestController.GetParametersAsString([]).ToString());
    }

    [Fact]
    public void AdjustForNamedPositionTurnsDecimalPositionIntoTop()
    {
        QueryParameter[] parameters = [new QueryParameter("pos", 123M)];

        new QueryParametersBuilder().AdjustForNamedPosition(parameters, NamedPosition.Top);

        Assert.Equal(QueryParameterType.String, parameters[0].Type);
        Assert.Equal("top", parameters[0].GetValueAsApiFormattedString());
    }

    [Fact]
    public void AdjustForNamedPositionTurnsDecimalPositionIntoBottom()
    {
        QueryParameter[] parameters = [new QueryParameter("pos", 123M)];

        new QueryParametersBuilder().AdjustForNamedPosition(parameters, NamedPosition.Bottom);

        Assert.Equal(QueryParameterType.String, parameters[0].Type);
        Assert.Equal("bottom", parameters[0].GetValueAsApiFormattedString());
    }

    [Fact]
    public void AdjustForNamedPositionLeavesParametersWithoutPositionUnchanged()
    {
        QueryParameter[] parameters = [new QueryParameter("name", "Card")];

        new QueryParametersBuilder().AdjustForNamedPosition(parameters, NamedPosition.Top);

        Assert.Equal("Card", parameters[0].GetValueAsApiFormattedString());
    }
}

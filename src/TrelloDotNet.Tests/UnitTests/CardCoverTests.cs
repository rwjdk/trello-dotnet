using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class CardCoverTests
{
    [Fact]
    public void PrepareForAddUpdate()
    {
        CardCover cardCover = new CardCover
        {
            Color = CardCoverColor.None,
            Size = CardCoverSize.None,
            Brightness = CardCoverBrightness.None
        };
        cardCover.PrepareForAddUpdate();
        Assert.Null(cardCover.Color);
        Assert.Null(cardCover.Size);
        Assert.Null(cardCover.Brightness);
    }

    [Fact]
    public void Constructors()
    {
        // ReSharper disable once ObjectCreationAsStatement
        new CardCover();
        // ReSharper disable once ObjectCreationAsStatement
        new CardCover(CardCoverColor.Blue, CardCoverSize.Full);
        // ReSharper disable once ObjectCreationAsStatement
        new CardCover("abc", CardCoverBrightness.Dark);
    }
}
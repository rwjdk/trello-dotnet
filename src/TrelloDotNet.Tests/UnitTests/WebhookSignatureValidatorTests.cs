using System.Security.Cryptography;
using System.Text;
using TrelloDotNet.Control.Webhook;

namespace TrelloDotNet.Tests.UnitTests;

public class WebhookSignatureValidatorTests
{
    [Fact]
    public void ValidateSignatureReturnsTrueForMatchingSignature()
    {
        const string json = """{"action":{"type":"updateCard"}}""";
        const string webhookUrl = "https://example.com/webhook";
        const string secret = "secret";
        string signature = CreateSignature(json, webhookUrl, secret);

        bool isValid = WebhookSignatureValidator.ValidateSignature(json, signature, webhookUrl, secret);

        Assert.True(isValid);
    }

    [Fact]
    public void ValidateSignatureReturnsFalseForDifferentSignature()
    {
        const string json = """{"action":{"type":"updateCard"}}""";
        const string webhookUrl = "https://example.com/webhook";
        const string secret = "secret";
        string signature = CreateSignature(json, webhookUrl, "another-secret");

        bool isValid = WebhookSignatureValidator.ValidateSignature(json, signature, webhookUrl, secret);

        Assert.False(isValid);
    }

    [Fact]
    public void ValidateSignatureRequiresSecret()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            WebhookSignatureValidator.ValidateSignature("{}", "signature", "https://example.com/webhook", null!));

        Assert.Equal("secret", exception.ParamName);
        Assert.Contains("API secret", exception.Message);
    }

    private static string CreateSignature(string json, string webhookUrl, string secret)
    {
        byte[] payload = Encoding.UTF8.GetBytes(json + webhookUrl);
        using HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(hmac.ComputeHash(payload));
    }
}

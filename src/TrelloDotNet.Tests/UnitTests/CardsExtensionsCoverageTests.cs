using TrelloDotNet.Extensions;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class CardsExtensionsCoverageTests
{
    [Theory]
    [InlineData(CardsConditionCount.Equal, 2, "two-labels")]
    [InlineData(CardsConditionCount.NotEqual, 2, "no-labels,one-label,three-labels")]
    [InlineData(CardsConditionCount.GreaterThan, 1, "two-labels,three-labels")]
    [InlineData(CardsConditionCount.LessThan, 2, "no-labels,one-label")]
    [InlineData(CardsConditionCount.GreaterThanOrEqual, 1, "one-label,two-labels,three-labels")]
    [InlineData(CardsConditionCount.LessThanOrEqual, 1, "no-labels,one-label")]
    public void FilterByLabelCount(CardsConditionCount condition, int count, string expectedNames)
    {
        List<Card> cards = CreateCardsWithLabelCounts();

        List<Card> filtered = cards.Filter(CardsFilterCondition.LabelCount(condition, count));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Fact]
    public void FilterByLabelCountBetween()
    {
        List<Card> cards = CreateCardsWithLabelCounts();

        List<Card> filtered = cards.Filter(CardsFilterCondition.LabelCountBetween(1, 2));

        Assert.Equal(["one-label", "two-labels"], filtered.Select(x => x.Name));
    }

    [Fact]
    public void FilterByLabelCountNotBetween()
    {
        List<Card> cards = CreateCardsWithLabelCounts();

        List<Card> filtered = cards.Filter(CardsFilterCondition.LabelCountNotBetween(1, 2));

        Assert.Equal(["no-labels", "three-labels"], filtered.Select(x => x.Name));
    }

    [Fact]
    public void FilterByLabelCountBetweenRequiresTwoNumbers()
    {
        List<Card> cards = CreateCardsWithLabelCounts();

        TrelloApiException exception = Assert.Throws<TrelloApiException>(() =>
            cards.Filter(CardsFilterCondition.AdvancedNumberCondition(CardsConditionField.LabelId, CardsConditionNumber.Between, 1)));

        Assert.Equal("Between Condition for Labels need 2 and only 2 Numbers", exception.Message);
    }

    [Fact]
    public void FilterByLabelCountNotBetweenRequiresTwoNumbers()
    {
        List<Card> cards = CreateCardsWithLabelCounts();

        TrelloApiException exception = Assert.Throws<TrelloApiException>(() =>
            cards.Filter(CardsFilterCondition.AdvancedNumberCondition(CardsConditionField.LabelId, CardsConditionNumber.NotBetween, 1)));

        Assert.Equal("NotBetween Condition for Labels need 2 and only 2 Numbers", exception.Message);
    }

    [Theory]
    [InlineData(CardsConditionCount.Equal, 2, "two-members")]
    [InlineData(CardsConditionCount.NotEqual, 2, "no-members,one-member,three-members")]
    [InlineData(CardsConditionCount.GreaterThan, 1, "two-members,three-members")]
    [InlineData(CardsConditionCount.LessThan, 2, "no-members,one-member")]
    [InlineData(CardsConditionCount.GreaterThanOrEqual, 1, "one-member,two-members,three-members")]
    [InlineData(CardsConditionCount.LessThanOrEqual, 1, "no-members,one-member")]
    public void FilterByMemberCount(CardsConditionCount condition, int count, string expectedNames)
    {
        List<Card> cards = CreateCardsWithMemberCounts();

        List<Card> filtered = cards.Filter(CardsFilterCondition.MemberCount(condition, count));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Fact]
    public void FilterByMemberCountBetween()
    {
        List<Card> cards = CreateCardsWithMemberCounts();

        List<Card> filtered = cards.Filter(CardsFilterCondition.MemberCountBetween(1, 2));

        Assert.Equal(["one-member", "two-members"], filtered.Select(x => x.Name));
    }

    [Fact]
    public void FilterByMemberCountNotBetweenRequiresTwoNumbers()
    {
        List<Card> cards = CreateCardsWithMemberCounts();

        TrelloApiException exception = Assert.Throws<TrelloApiException>(() =>
            cards.Filter(CardsFilterCondition.AdvancedNumberCondition(CardsConditionField.MemberId, CardsConditionNumber.NotBetween, 1)));

        Assert.Equal("NotBetween Condition for Members need 2 and only 2 Numbers", exception.Message);
    }

    private static List<Card> CreateCardsWithLabelCounts()
    {
        return
        [
            CreateCard("no-labels", labelIds: []),
            CreateCard("one-label", labelIds: ["label1"]),
            CreateCard("two-labels", labelIds: ["label1", "label2"]),
            CreateCard("three-labels", labelIds: ["label1", "label2", "label3"])
        ];
    }

    private static List<Card> CreateCardsWithMemberCounts()
    {
        return
        [
            CreateCard("no-members", memberIds: []),
            CreateCard("one-member", memberIds: ["member1"]),
            CreateCard("two-members", memberIds: ["member1", "member2"]),
            CreateCard("three-members", memberIds: ["member1", "member2", "member3"])
        ];
    }

    private static Card CreateCard(string name, List<string>? labelIds = null, List<string>? memberIds = null)
    {
        return new Card
        {
            Name = name,
            LabelIds = labelIds ?? [],
            MemberIds = memberIds ?? []
        };
    }
}

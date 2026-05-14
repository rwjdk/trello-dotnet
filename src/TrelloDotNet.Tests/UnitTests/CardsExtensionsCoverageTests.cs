using TrelloDotNet.Extensions;
using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class CardsExtensionsCoverageTests
{
    private static readonly DateTimeOffset BaseDate = new(2025, 1, 10, 12, 0, 0, TimeSpan.Zero);

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

    [Theory]
    [InlineData(CardsConditionString.Equal, "Alpha", "alpha")]
    [InlineData(CardsConditionString.NotEqual, "alpha", "bravo,charlie,blank-name")]
    [InlineData(CardsConditionString.Contains, "ha", "alpha,charlie")]
    [InlineData(CardsConditionString.DoNotContains, "ha", "bravo,blank-name")]
    [InlineData(CardsConditionString.AnyOfThese, "alpha suffix|bravo suffix", "alpha,bravo")]
    [InlineData(CardsConditionString.NoneOfThese, "alpha|bravo", "charlie,blank-name")]
    [InlineData(CardsConditionString.RegEx, "a$", "alpha")]
    [InlineData(CardsConditionString.StartsWith, "Br", "bravo")]
    [InlineData(CardsConditionString.EndsWith, "lie", "charlie")]
    [InlineData(CardsConditionString.DoNotStartWith, "Ch", "alpha,bravo,blank-name")]
    [InlineData(CardsConditionString.DoNotEndWith, "vo", "alpha,charlie,blank-name")]
    public void FilterByNameStringConditions(CardsConditionString condition, string value, string expectedNames)
    {
        List<Card> cards = CreateTextCards();

        List<Card> filtered = cards.Filter(CardsFilterCondition.Name(condition, value.Split('|')));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Theory]
    [InlineData(CardsConditionCount.Equal, 5, "alpha,bravo")]
    [InlineData(CardsConditionCount.NotEqual, 5, "charlie,blank-name")]
    [InlineData(CardsConditionCount.GreaterThan, 5, "charlie,blank-name")]
    [InlineData(CardsConditionCount.LessThan, 6, "alpha,bravo")]
    [InlineData(CardsConditionCount.GreaterThanOrEqual, 7, "charlie,blank-name")]
    [InlineData(CardsConditionCount.LessThanOrEqual, 5, "alpha,bravo")]
    public void FilterByNameLengthConditions(CardsConditionCount condition, int value, string expectedNames)
    {
        List<Card> cards = CreateTextCards();

        List<Card> filtered = cards.Filter(CardsFilterCondition.AdvancedNumberCondition(CardsConditionField.Name, (CardsConditionNumber)condition, value));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Fact]
    public void FilterByNameValuePresence()
    {
        List<Card> cards =
        [
            CreateCard("alpha"),
            CreateCard("bravo"),
            CreateCard("charlie"),
            CreateCard("")
        ];

        Assert.Equal(["alpha", "bravo", "charlie"], cards.Filter(CardsFilterCondition.AdvancedStringCondition(CardsConditionField.Name, (CardsConditionString)CardsCondition.HasAnyValue)).Select(x => x.Name));
        Assert.Equal([""], cards.Filter(CardsFilterCondition.AdvancedStringCondition(CardsConditionField.Name, (CardsConditionString)CardsCondition.DoNotHaveAnyValue)).Select(x => x.Name));
    }

    [Theory]
    [InlineData(CardsConditionString.Equal, "first list", "alpha,bravo")]
    [InlineData(CardsConditionString.NotEqual, "first list", "charlie,blank-name")]
    [InlineData(CardsConditionString.Contains, "list", "alpha,bravo,charlie")]
    [InlineData(CardsConditionString.DoNotContains, "list", "blank-name")]
    [InlineData(CardsConditionString.AnyOfThese, "first list|other lane", "alpha,bravo,blank-name")]
    [InlineData(CardsConditionString.NoneOfThese, "first list|other lane", "charlie")]
    [InlineData(CardsConditionString.RegEx, "list$", "alpha,bravo,charlie")]
    [InlineData(CardsConditionString.StartsWith, "second", "charlie")]
    [InlineData(CardsConditionString.EndsWith, "lane", "blank-name")]
    [InlineData(CardsConditionString.DoNotStartWith, "first", "charlie,blank-name")]
    [InlineData(CardsConditionString.DoNotEndWith, "list", "blank-name")]
    public void FilterByListNameStringConditions(CardsConditionString condition, string value, string expectedNames)
    {
        List<Card> cards = CreateTextCards();

        List<Card> filtered = cards.Filter(CardsFilterCondition.ListName(condition, value.Split('|')));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Fact]
    public void FilterByListIdStringConditions()
    {
        List<Card> cards = CreateTextCards();

        Assert.Equal(["alpha", "bravo"], cards.Filter(CardsFilterCondition.ListId(CardsConditionIds.Equal, "list-1")).Select(x => x.Name));
        Assert.Equal(["alpha", "bravo", "blank-name"], cards.Filter(CardsFilterCondition.ListId(CardsConditionIds.AnyOfThese, "list-1", "list-3")).Select(x => x.Name));
        Assert.Equal(["charlie", "blank-name"], cards.Filter(CardsFilterCondition.ListId(CardsConditionIds.NotEqual, "list-1")).Select(x => x.Name));
        Assert.Equal(["charlie"], cards.Filter(CardsFilterCondition.ListId(CardsConditionIds.NoneOfThese, "list-1", "list-3")).Select(x => x.Name));
    }

    [Fact]
    public void FilterBySingleValueIdAndNameBranches()
    {
        List<Card> cards = CreateTextCards();

        Assert.Equal(["alpha"], cards.Filter(CardsFilterCondition.LabelId(CardsConditionIds.AllOfThese, "label-bug", "label-urgent")).Select(x => x.Name));
        Assert.Equal(["alpha"], cards.Filter(CardsFilterCondition.LabelId(CardsConditionIds.Equal, "label-bug", "label-urgent")).Select(x => x.Name));
        Assert.Equal(["alpha", "bravo"], cards.Filter(CardsFilterCondition.LabelId(CardsConditionIds.AnyOfThese, "label-bug", "label-feature")).Select(x => x.Name));
        Assert.Equal(["charlie", "blank-name"], cards.Filter(CardsFilterCondition.LabelId(CardsConditionIds.NoneOfThese, "label-bug", "label-feature")).Select(x => x.Name));
        Assert.Equal(["alpha"], cards.Filter(CardsFilterCondition.LabelId(CardsConditionIds.AllOfThese, "label-bug")).Select(x => x.Name));
        Assert.Equal(["bravo"], cards.Filter(CardsFilterCondition.LabelId(CardsConditionIds.Equal, "label-feature")).Select(x => x.Name));
        Assert.Equal(["alpha", "charlie", "blank-name"], cards.Filter(CardsFilterCondition.LabelId(CardsConditionIds.NotEqual, "label-feature")).Select(x => x.Name));
        Assert.Equal(["alpha"], cards.Filter(CardsFilterCondition.MemberId(CardsConditionIds.AllOfThese, "member-rasmus", "member-jane")).Select(x => x.Name));
        Assert.Equal(["alpha"], cards.Filter(CardsFilterCondition.MemberId(CardsConditionIds.Equal, "member-rasmus", "member-jane")).Select(x => x.Name));
        Assert.Equal(["alpha", "bravo"], cards.Filter(CardsFilterCondition.MemberId(CardsConditionIds.AnyOfThese, "member-rasmus", "member-lee")).Select(x => x.Name));
        Assert.Equal(["charlie", "blank-name"], cards.Filter(CardsFilterCondition.MemberId(CardsConditionIds.NoneOfThese, "member-rasmus", "member-lee")).Select(x => x.Name));
        Assert.Equal(["alpha"], cards.Filter(CardsFilterCondition.LabelName(CardsConditionString.AllOfThese, "Bug")).Select(x => x.Name));
        Assert.Equal(["bravo"], cards.Filter(CardsFilterCondition.LabelName(CardsConditionString.Equal, "Feature")).Select(x => x.Name));
        Assert.Equal(["alpha", "charlie", "blank-name"], cards.Filter(CardsFilterCondition.LabelName(CardsConditionString.NotEqual, "Feature")).Select(x => x.Name));
        Assert.Equal(["alpha"], cards.Filter(CardsFilterCondition.MemberId(CardsConditionIds.AllOfThese, "member-rasmus")).Select(x => x.Name));
        Assert.Equal(["bravo"], cards.Filter(CardsFilterCondition.MemberId(CardsConditionIds.Equal, "member-lee")).Select(x => x.Name));
        Assert.Equal(["alpha", "charlie", "blank-name"], cards.Filter(CardsFilterCondition.MemberId(CardsConditionIds.NotEqual, "member-lee")).Select(x => x.Name));
        Assert.Equal(["alpha"], cards.Filter(CardsFilterCondition.MemberName(CardsConditionString.AllOfThese, "Rasmus")).Select(x => x.Name));
        Assert.Equal(["bravo"], cards.Filter(CardsFilterCondition.MemberName(CardsConditionString.Equal, "Lee")).Select(x => x.Name));
        Assert.Equal(["alpha", "charlie", "blank-name"], cards.Filter(CardsFilterCondition.MemberName(CardsConditionString.NotEqual, "Lee")).Select(x => x.Name));
    }

    [Theory]
    [InlineData(CardsConditionString.AllOfThese, "Bug|Urgent", "alpha")]
    [InlineData(CardsConditionString.Equal, "Bug|Urgent", "alpha")]
    [InlineData(CardsConditionString.AnyOfThese, "Feature|Urgent", "alpha,bravo")]
    [InlineData(CardsConditionString.NoneOfThese, "Bug|Feature", "charlie,blank-name")]
    [InlineData(CardsConditionString.Contains, "gen", "alpha")]
    [InlineData(CardsConditionString.DoNotContains, "gen", "bravo,charlie,blank-name")]
    [InlineData(CardsConditionString.RegEx, "^[bf]", "alpha,bravo")]
    [InlineData(CardsConditionString.StartsWith, "Fea", "bravo")]
    [InlineData(CardsConditionString.EndsWith, "gent", "alpha")]
    [InlineData(CardsConditionString.DoNotStartWith, "Ur", "bravo,charlie,blank-name")]
    [InlineData(CardsConditionString.DoNotEndWith, "ure", "alpha,charlie,blank-name")]
    public void FilterByLabelNameStringConditions(CardsConditionString condition, string value, string expectedNames)
    {
        List<Card> cards = CreateTextCards();

        List<Card> filtered = cards.Filter(CardsFilterCondition.LabelName(condition, value.Split('|')));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Theory]
    [InlineData(CardsConditionString.AllOfThese, "Rasmus|Jane", "alpha")]
    [InlineData(CardsConditionString.Equal, "Rasmus|Jane", "alpha")]
    [InlineData(CardsConditionString.AnyOfThese, "Jane|Lee", "alpha,bravo")]
    [InlineData(CardsConditionString.NoneOfThese, "Rasmus|Lee", "charlie,blank-name")]
    [InlineData(CardsConditionString.Contains, "sm", "alpha")]
    [InlineData(CardsConditionString.DoNotContains, "sm", "bravo,charlie,blank-name")]
    [InlineData(CardsConditionString.RegEx, "^[rl]", "alpha,bravo")]
    [InlineData(CardsConditionString.StartsWith, "Ja", "alpha")]
    [InlineData(CardsConditionString.EndsWith, "ee", "bravo")]
    [InlineData(CardsConditionString.DoNotStartWith, "Ra", "bravo,charlie,blank-name")]
    [InlineData(CardsConditionString.DoNotEndWith, "ne", "bravo,charlie,blank-name")]
    public void FilterByMemberNameStringConditions(CardsConditionString condition, string value, string expectedNames)
    {
        List<Card> cards = CreateTextCards();

        List<Card> filtered = cards.Filter(CardsFilterCondition.MemberName(condition, value.Split('|')));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Theory]
    [InlineData(CardsConditionString.Equal, "first description", "alpha")]
    [InlineData(CardsConditionString.NotEqual, "first description", "bravo,charlie,blank-name")]
    [InlineData(CardsConditionString.Contains, "description", "alpha,charlie")]
    [InlineData(CardsConditionString.DoNotContains, "description", "bravo,blank-name")]
    [InlineData(CardsConditionString.AnyOfThese, "first description suffix|second value suffix", "alpha,bravo,blank-name")]
    [InlineData(CardsConditionString.NoneOfThese, "first description|second value", "charlie,blank-name")]
    [InlineData(CardsConditionString.RegEx, "value$", "bravo")]
    [InlineData(CardsConditionString.StartsWith, "third", "charlie")]
    [InlineData(CardsConditionString.EndsWith, "value", "bravo")]
    [InlineData(CardsConditionString.DoNotStartWith, "first", "bravo,charlie,blank-name")]
    [InlineData(CardsConditionString.DoNotEndWith, "value", "alpha,charlie,blank-name")]
    public void FilterByDescriptionStringConditions(CardsConditionString condition, string value, string expectedNames)
    {
        List<Card> cards = CreateTextCards();

        List<Card> filtered = cards.Filter(CardsFilterCondition.Description(condition, value.Split('|')));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Theory]
    [InlineData(CardsConditionCount.Equal, 0, "blank-name")]
    [InlineData(CardsConditionCount.NotEqual, 0, "alpha,bravo,charlie")]
    [InlineData(CardsConditionCount.GreaterThan, 12, "alpha,charlie")]
    [InlineData(CardsConditionCount.LessThan, 12, "blank-name")]
    [InlineData(CardsConditionCount.GreaterThanOrEqual, 12, "alpha,bravo,charlie")]
    [InlineData(CardsConditionCount.LessThanOrEqual, 12, "bravo,blank-name")]
    public void FilterByDescriptionLengthConditions(CardsConditionCount condition, int value, string expectedNames)
    {
        List<Card> cards = CreateTextCards();

        List<Card> filtered = cards.Filter(CardsFilterCondition.AdvancedNumberCondition(CardsConditionField.Description, (CardsConditionNumber)condition, value));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Theory]
    [InlineData(CardsConditionDate.Equal, 1, "due-soon")]
    [InlineData(CardsConditionDate.NotEqual, 1, "due-later,due-complete")]
    [InlineData(CardsConditionDate.GreaterThan, 1, "due-later,due-complete")]
    [InlineData(CardsConditionDate.LessThan, 5, "due-soon,due-later,due-complete")]
    [InlineData(CardsConditionDate.GreaterThanOrEqual, 4, "due-later,due-complete")]
    [InlineData(CardsConditionDate.LessThanOrEqual, 1, "due-soon")]
    public void FilterByDueDateConditions(CardsConditionDate condition, int daysToCompare, string expectedNames)
    {
        List<Card> cards = CreateDateCards();

        List<Card> filtered = cards.Filter(CardsFilterCondition.Due(condition, true, BaseDate.AddDays(daysToCompare)));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Fact]
    public void FilterByDueDateRangesAndCompletion()
    {
        List<Card> cards = CreateDateCards();

        Assert.Equal(["due-soon"], cards.Filter(CardsFilterCondition.Due(CardsConditionDate.Equal, false, BaseDate.AddDays(1))).Select(x => x.Name));
        Assert.Equal(["due-later"], cards.Filter(CardsFilterCondition.Due(CardsConditionDate.NotEqual, false, BaseDate.AddDays(1))).Select(x => x.Name));
        Assert.Equal(["due-later"], cards.Filter(CardsFilterCondition.Due(CardsConditionDate.GreaterThan, false, BaseDate.AddDays(1))).Select(x => x.Name));
        Assert.Equal(["due-soon"], cards.Filter(CardsFilterCondition.Due(CardsConditionDate.LessThan, false, BaseDate.AddDays(4))).Select(x => x.Name));
        Assert.Equal(["due-later"], cards.Filter(CardsFilterCondition.Due(CardsConditionDate.GreaterThanOrEqual, false, BaseDate.AddDays(4))).Select(x => x.Name));
        Assert.Equal(["due-soon"], cards.Filter(CardsFilterCondition.Due(CardsConditionDate.LessThanOrEqual, false, BaseDate.AddDays(1))).Select(x => x.Name));
        Assert.Equal(["due-soon", "due-later"], cards.Filter(CardsFilterCondition.DueBetween(false, BaseDate, BaseDate.AddDays(5))).Select(x => x.Name));
        Assert.Equal(["due-soon"], cards.Filter(CardsFilterCondition.DueNotBetween(true, BaseDate.AddDays(2), BaseDate.AddDays(5))).Select(x => x.Name));
        Assert.Equal(["due-soon", "due-later"], cards.Filter(CardsFilterCondition.HasDueDate(false)).Select(x => x.Name));
        Assert.Equal(["started", "starts-later", "no-dates"], cards.Filter(CardsFilterCondition.HasNoDueDate()).Select(x => x.Name));
        Assert.Equal(["due-soon", "due-later"], cards.Filter(CardsFilterCondition.Due(CardsConditionDate.AnyOfThese, false, BaseDate.AddDays(1), BaseDate.AddDays(4))).Select(x => x.Name));
        Assert.Equal(["due-later"], cards.Filter(CardsFilterCondition.Due(CardsConditionDate.NoneOfThese, false, BaseDate.AddDays(1))).Select(x => x.Name));
    }

    [Fact]
    public void FilterByStartDateRangesAndPresence()
    {
        List<Card> cards = CreateDateCards();

        Assert.Equal(["started"], cards.Filter(CardsFilterCondition.Start(CardsConditionDate.Equal, BaseDate.AddDays(-2))).Select(x => x.Name));
        Assert.Equal(["starts-later"], cards.Filter(CardsFilterCondition.Start(CardsConditionDate.NotEqual, BaseDate.AddDays(-2))).Select(x => x.Name));
        Assert.Equal(["starts-later"], cards.Filter(CardsFilterCondition.Start(CardsConditionDate.GreaterThan, BaseDate)).Select(x => x.Name));
        Assert.Equal(["started"], cards.Filter(CardsFilterCondition.Start(CardsConditionDate.LessThan, BaseDate)).Select(x => x.Name));
        Assert.Equal(["starts-later"], cards.Filter(CardsFilterCondition.Start(CardsConditionDate.GreaterThanOrEqual, BaseDate.AddDays(2))).Select(x => x.Name));
        Assert.Equal(["started"], cards.Filter(CardsFilterCondition.Start(CardsConditionDate.LessThanOrEqual, BaseDate.AddDays(-2))).Select(x => x.Name));
        Assert.Equal(["started", "starts-later"], cards.Filter(CardsFilterCondition.HasStartDate()).Select(x => x.Name));
        Assert.Equal(["due-soon", "due-later", "due-complete", "no-dates"], cards.Filter(CardsFilterCondition.HasNoStartDate()).Select(x => x.Name));
        Assert.Equal(["started", "starts-later"], cards.Filter(CardsFilterCondition.StartBetween(BaseDate.AddDays(-3), BaseDate.AddDays(3))).Select(x => x.Name));
        Assert.Equal(["started"], cards.Filter(CardsFilterCondition.StartNotBetween(BaseDate.AddDays(-1), BaseDate.AddDays(3))).Select(x => x.Name));
        Assert.Equal(["started"], cards.Filter(CardsFilterCondition.Start(CardsConditionDate.AnyOfThese, BaseDate.AddDays(-2), BaseDate.AddDays(9))).Select(x => x.Name));
        Assert.Equal(["starts-later"], cards.Filter(CardsFilterCondition.Start(CardsConditionDate.NoneOfThese, BaseDate.AddDays(-2))).Select(x => x.Name));
    }

    [Fact]
    public void FilterByCreatedDateConditions()
    {
        DateTimeOffset first = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset second = new(2025, 1, 2, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset third = new(2025, 1, 3, 0, 0, 0, TimeSpan.Zero);
        List<Card> cards =
        [
            CreateCardWithCreated("created-first", first),
            CreateCardWithCreated("created-second", second),
            CreateCardWithCreated("created-third", third),
            CreateCard("created-missing")
        ];

        Assert.Equal(["created-first"], cards.Filter(CardsFilterCondition.Created(CardsConditionDate.Equal, first)).Select(x => x.Name));
        Assert.Equal(["created-second", "created-third"], cards.Filter(CardsFilterCondition.Created(CardsConditionDate.NotEqual, first)).Select(x => x.Name));
        Assert.Equal(["created-second", "created-third"], cards.Filter(CardsFilterCondition.Created(CardsConditionDate.GreaterThan, first)).Select(x => x.Name));
        Assert.Equal(["created-first"], cards.Filter(CardsFilterCondition.Created(CardsConditionDate.LessThan, second)).Select(x => x.Name));
        Assert.Equal(["created-second", "created-third"], cards.Filter(CardsFilterCondition.Created(CardsConditionDate.GreaterThanOrEqual, second)).Select(x => x.Name));
        Assert.Equal(["created-first", "created-second"], cards.Filter(CardsFilterCondition.Created(CardsConditionDate.LessThanOrEqual, second)).Select(x => x.Name));
        Assert.Equal(["created-first", "created-second"], cards.Filter(CardsFilterCondition.CreatedBetween(first, second)).Select(x => x.Name));
        Assert.Equal(["created-third"], cards.Filter(CardsFilterCondition.CreatedNotBetween(first, second)).Select(x => x.Name));
        Assert.Equal(["created-first", "created-third"], cards.Filter(CardsFilterCondition.Created(CardsConditionDate.AnyOfThese, first, third)).Select(x => x.Name));
        Assert.Equal(["created-second"], cards.Filter(CardsFilterCondition.Created(CardsConditionDate.NoneOfThese, first, third)).Select(x => x.Name));
    }

    [Fact]
    public void FilterByDueComplete()
    {
        List<Card> cards = CreateDateCards();

        Assert.Equal(["due-complete"], cards.Filter(CardsFilterCondition.IsComplete()).Select(x => x.Name));
        Assert.Equal(["due-soon", "due-later", "started", "starts-later", "no-dates"], cards.Filter(CardsFilterCondition.IsNotComplete()).Select(x => x.Name));
    }

    [Fact]
    public void FilterThrowsForConditionsThatDoNotMakeSense()
    {
        List<Card> cards = CreateTextCards();

        Assert.Equal("AllOfThese on Description Filter does not make sense", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.Description(CardsConditionString.AllOfThese, "anything"))).Message);
        Assert.Equal("Condition 'Contains' does not make sense to apply to a List Condition", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.ListId((CardsConditionIds)CardsCondition.Contains, "list"))).Message);
        Assert.Equal("Contains on DueComplete Filter does not make sense", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.AdvancedStringCondition(CardsConditionField.DueComplete, CardsConditionString.Contains, "true"))).Message);
        Assert.Equal("Condition 'Contains' does not make sense to apply to a FilterCreateDate", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.AdvancedStringCondition(CardsConditionField.Created, CardsConditionString.Contains, "2025"))).Message);
        Assert.Equal("Between Condition for Created Date need 2 and only 2 Dates", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.Created(CardsConditionDate.Between, BaseDate))).Message);
        Assert.Equal("NotBetween Condition for Start Date need 2 and only 2 Dates", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.Start(CardsConditionDate.NotBetween, BaseDate))).Message);
        Assert.Equal("Condition 'Contains' does not make sense to apply to a FilterDueDate", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.AdvancedStringCondition(CardsConditionField.Due, CardsConditionString.Contains, "2025"))).Message);
    }

    [Theory]
    [InlineData(CardsConditionString.Equal, "Ready", "text-ready")]
    [InlineData(CardsConditionString.NotEqual, "Ready", "text-blocked,field-missing")]
    [InlineData(CardsConditionString.Contains, "ea", "text-ready")]
    [InlineData(CardsConditionString.DoNotContains, "ea", "text-blocked,field-missing")]
    [InlineData(CardsConditionString.AnyOfThese, "Ready|Done", "text-ready")]
    [InlineData(CardsConditionString.NoneOfThese, "Ready|Done", "text-blocked,field-missing")]
    [InlineData(CardsConditionString.RegEx, "^rea", "text-ready")]
    [InlineData(CardsConditionString.StartsWith, "Blo", "text-blocked")]
    [InlineData(CardsConditionString.EndsWith, "ady", "text-ready")]
    [InlineData(CardsConditionString.DoNotStartWith, "Blo", "text-ready,field-missing")]
    [InlineData(CardsConditionString.DoNotEndWith, "ady", "text-blocked,field-missing")]
    public void FilterByTextCustomField(CardsConditionString condition, string value, string expectedNames)
    {
        CustomField customField = CreateCustomField("cf-text", CustomFieldType.Text);
        List<Card> cards =
        [
            CreateCard("text-ready", customFieldItems: [CreateCustomFieldItem("cf-text", text: "Ready")]),
            CreateCard("text-blocked", customFieldItems: [CreateCustomFieldItem("cf-text", text: "Blocked")]),
            CreateCard("field-missing", customFieldItems: [])
        ];

        List<Card> filtered = cards.Filter(CardsFilterCondition.CustomField(customField, condition, value.Split('|')));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Theory]
    [InlineData(CardsConditionNumber.Equal, 7, "number-seven")]
    [InlineData(CardsConditionNumber.NotEqual, 7, "number-nine,field-missing")]
    [InlineData(CardsConditionNumber.GreaterThan, 7, "number-nine")]
    [InlineData(CardsConditionNumber.LessThan, 9, "number-seven,field-missing")]
    [InlineData(CardsConditionNumber.GreaterThanOrEqual, 9, "number-nine")]
    [InlineData(CardsConditionNumber.LessThanOrEqual, 7, "number-seven,field-missing")]
    public void FilterByNumberCustomField(CardsConditionNumber condition, int value, string expectedNames)
    {
        CustomField customField = CreateCustomField("cf-number", CustomFieldType.Number);
        List<Card> cards =
        [
            CreateCard("number-seven", customFieldItems: [CreateCustomFieldItem("cf-number", number: 7)]),
            CreateCard("number-nine", customFieldItems: [CreateCustomFieldItem("cf-number", number: 9)]),
            CreateCard("field-missing", customFieldItems: [])
        ];

        List<Card> filtered = cards.Filter(CardsFilterCondition.CustomField(customField, condition, value));

        Assert.Equal(expectedNames.Split(','), filtered.Select(x => x.Name));
    }

    [Fact]
    public void FilterByCustomFieldSpecialConditions()
    {
        DateTimeOffset date = BaseDate.AddDays(2);
        CustomField textField = CreateCustomField("cf-text", CustomFieldType.Text);
        CustomField checkboxField = CreateCustomField("cf-check", CustomFieldType.Checkbox);
        CustomField dateField = CreateCustomField("cf-date", CustomFieldType.Date);
        CustomField numberField = CreateCustomField("cf-number", CustomFieldType.Number);
        CustomField listField = CreateCustomField("cf-list", CustomFieldType.List, [CreateCustomFieldOption("option-a"), CreateCustomFieldOption("option-b")]);
        List<Card> cards =
        [
            CreateCard("field-values", customFieldItems:
            [
                CreateCustomFieldItem("cf-text", text: "Ready"),
                CreateCustomFieldItem("cf-check", isChecked: true),
                CreateCustomFieldItem("cf-date", date: date),
                CreateCustomFieldItem("cf-number", number: 7),
                CreateCustomFieldItem("cf-list", valueId: "option-a")
            ]),
            CreateCard("other-values", customFieldItems:
            [
                CreateCustomFieldItem("cf-check", isChecked: false),
                CreateCustomFieldItem("cf-date", date: BaseDate.AddDays(5)),
                CreateCustomFieldItem("cf-number", number: 11),
                CreateCustomFieldItem("cf-list", valueId: "option-b")
            ]),
            CreateCard("field-missing", customFieldItems: [])
        ];

        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(textField, (CardsConditionString)CardsCondition.HasAnyValue)).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(textField, (CardsConditionString)CardsCondition.DoNotHaveAnyValue)).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(checkboxField, CardsConditionBoolean.Equal, true)).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(checkboxField, CardsConditionBoolean.NotEqual, true)).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(dateField, CardsConditionDate.Between, BaseDate, BaseDate.AddDays(3))).Select(x => x.Name));
        Assert.Equal(["other-values"], cards.Filter(CardsFilterCondition.CustomField(dateField, CardsConditionDate.NotBetween, BaseDate, BaseDate.AddDays(3))).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(dateField, CardsConditionDate.AnyOfThese, date, BaseDate.AddDays(9))).Select(x => x.Name));
        Assert.Equal(["other-values"], cards.Filter(CardsFilterCondition.CustomField(dateField, CardsConditionDate.NoneOfThese, date, BaseDate.AddDays(9))).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(numberField, CardsConditionNumber.Between, 6, 8)).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(numberField, CardsConditionNumber.NotBetween, 6, 8)).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(numberField, CardsConditionNumber.AnyOfThese, 7, 9)).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(numberField, CardsConditionNumber.NoneOfThese, 7, 9)).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.Equal, "option-a")).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.NotEqual, "option-a")).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.AnyOfThese, "option-a", "option-c")).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.NoneOfThese, "option-a", "option-c")).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.Contains, "ion-a")).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.DoNotContains, "ion-a")).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.RegEx, "a$")).Select(x => x.Name));
        Assert.Equal(["other-values"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.StartsWith, "option-b")).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.EndsWith, "-a")).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.DoNotStartWith, "option-a")).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.DoNotEndWith, "-a")).Select(x => x.Name));
        Assert.Equal(["field-values", "other-values"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionNumber.GreaterThan, 7)).Select(x => x.Name));
        Assert.Equal(["field-missing"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionNumber.LessThan, 1)).Select(x => x.Name));
        Assert.Equal(["field-values", "other-values"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionNumber.GreaterThanOrEqual, 8)).Select(x => x.Name));
        Assert.Equal(["field-missing"], cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionNumber.LessThanOrEqual, 0)).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(textField, CardsConditionNumber.GreaterThan, 4)).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(textField, CardsConditionNumber.LessThan, 1)).Select(x => x.Name));
        Assert.Equal(["field-values"], cards.Filter(CardsFilterCondition.CustomField(textField, CardsConditionNumber.GreaterThanOrEqual, 5)).Select(x => x.Name));
        Assert.Equal(["other-values", "field-missing"], cards.Filter(CardsFilterCondition.CustomField(textField, CardsConditionNumber.LessThanOrEqual, 0)).Select(x => x.Name));
    }

    [Fact]
    public void FilterByCustomFieldThrowsHelpfulMessagesForInvalidConditions()
    {
        CustomField textField = CreateCustomField("cf-text", CustomFieldType.Text);
        CustomField checkboxField = CreateCustomField("cf-check", CustomFieldType.Checkbox);
        CustomField dateField = CreateCustomField("cf-date", CustomFieldType.Date);
        CustomField numberField = CreateCustomField("cf-number", CustomFieldType.Number);
        CustomField listField = CreateCustomField("cf-list", CustomFieldType.List, [CreateCustomFieldOption("option-a")]);
        List<Card> cards = [CreateCard("field-values", customFieldItems: [CreateCustomFieldItem("cf-text", text: "Ready")])];

        Assert.Equal("CustomField was not provided", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.CustomField(null!, CardsConditionString.Equal, "Ready"))).Message);
        Assert.Equal("Custom Field of Type Checkbox can't use Condition 'Contains'", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.CustomField(checkboxField, CardsConditionString.Contains, "true"))).Message);
        Assert.Equal("Condition 'Contains' does not make sense to apply to a CustomField of Type Date", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.CustomField(dateField, CardsConditionString.Contains, "2025"))).Message);
        Assert.Equal("Between Condition for Custom Field need 2 and only 2 Dates", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.CustomField(dateField, CardsConditionDate.Between, BaseDate))).Message);
        Assert.Equal("Between Condition for Custom Field need 2 and only 2 Numbers", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.CustomField(numberField, CardsConditionNumber.Between, 7))).Message);
        Assert.Equal("Condition 'Contains' does not make sense to apply to a CustomField of Type Number", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.CustomField(numberField, CardsConditionString.Contains, "7"))).Message);
        Assert.Equal("Condition 'AllOfThese' does not make sense to apply to a CustomField of Type List", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.CustomField(listField, CardsConditionString.AllOfThese, "option-a"))).Message);
        Assert.Equal("Condition 'AllOfThese' does not make sense to apply to a CustomField of Type Text", Assert.Throws<TrelloApiException>(() => cards.Filter(CardsFilterCondition.CustomField(textField, CardsConditionString.AllOfThese, "Ready"))).Message);
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

    private static List<Card> CreateTextCards()
    {
        return
        [
            CreateCard("alpha", description: "first description", listId: "list-1", listName: "first list", labelIds: ["label-bug", "label-urgent"], labels: ["Bug", "Urgent"], memberIds: ["member-rasmus", "member-jane"], members: ["Rasmus", "Jane"]),
            CreateCard("bravo", description: "second value", listId: "list-1", listName: "first list", labelIds: ["label-feature"], labels: ["Feature"], memberIds: ["member-lee"], members: ["Lee"]),
            CreateCard("charlie", description: "third description", listId: "list-2", listName: "second list"),
            CreateCard("blank-name", description: "", listId: "list-3", listName: "other lane")
        ];
    }

    private static List<Card> CreateDateCards()
    {
        return
        [
            CreateCard("due-soon", due: BaseDate.AddDays(1)),
            CreateCard("due-later", due: BaseDate.AddDays(4)),
            CreateCard("due-complete", due: BaseDate.AddDays(4), dueComplete: true),
            CreateCard("started", start: BaseDate.AddDays(-2)),
            CreateCard("starts-later", start: BaseDate.AddDays(2)),
            CreateCard("no-dates")
        ];
    }

    private static Card CreateCardWithCreated(string name, DateTimeOffset created)
    {
        Card card = CreateCard(name);
        SetPrivateProperty(card, nameof(Card.Id), $"{created.ToUnixTimeSeconds():x8}0000000000000000");
        return card;
    }

    private static Card CreateCard(
        string name,
        string? description = null,
        string? listId = null,
        string? listName = null,
        List<string>? labelIds = null,
        List<string>? labels = null,
        List<string>? memberIds = null,
        List<string>? members = null,
        DateTimeOffset? start = null,
        DateTimeOffset? due = null,
        bool dueComplete = false,
        List<CustomFieldItem>? customFieldItems = null)
    {
        Card card = new()
        {
            Name = name,
            Description = description ?? string.Empty,
            ListId = listId ?? string.Empty,
            LabelIds = labelIds ?? [],
            MemberIds = memberIds ?? [],
            Start = start,
            Due = due,
            DueComplete = dueComplete
        };

        SetPrivateProperty(card, nameof(Card.Labels), labels?.Select(CreateLabel).ToList() ?? []);
        SetPrivateProperty(card, nameof(Card.Members), members?.Select(CreateMember).ToList() ?? []);
        SetPrivateProperty(card, nameof(Card.List), new TrelloDotNet.Model.List { Name = listName ?? string.Empty });
        SetPrivateProperty(card, nameof(Card.CustomFieldItems), customFieldItems ?? []);
        return card;
    }

    private static Label CreateLabel(string name)
    {
        return new Label("board", name);
    }

    private static Member CreateMember(string fullName)
    {
        Member member = new();
        SetPrivateProperty(member, nameof(Member.FullName), fullName);
        return member;
    }

    private static CustomField CreateCustomField(string id, CustomFieldType type, List<CustomFieldOption>? options = null)
    {
        CustomField customField = new();
        SetPrivateProperty(customField, nameof(CustomField.Id), id);
        SetPrivateProperty(customField, nameof(CustomField.Type), type);
        SetPrivateProperty(customField, nameof(CustomField.Options), options ?? []);
        return customField;
    }

    private static CustomFieldItem CreateCustomFieldItem(string customFieldId, string? text = null, decimal? number = null, DateTimeOffset? date = null, bool? isChecked = null, string? valueId = null)
    {
        CustomFieldItemValue value = new();
        if (text != null)
        {
            SetPrivateProperty(value, nameof(CustomFieldItemValue.TextAsString), text);
        }

        if (number.HasValue)
        {
            SetPrivateProperty(value, nameof(CustomFieldItemValue.NumberAsString), number.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        if (date.HasValue)
        {
            SetPrivateProperty(value, nameof(CustomFieldItemValue.DateAsString), date.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture));
        }

        if (isChecked.HasValue)
        {
            SetPrivateProperty(value, nameof(CustomFieldItemValue.CheckedAsString), isChecked.Value ? "true" : "false");
        }

        CustomFieldItem customFieldItem = new();
        SetPrivateProperty(customFieldItem, nameof(CustomFieldItem.CustomFieldId), customFieldId);
        SetPrivateProperty(customFieldItem, nameof(CustomFieldItem.Value), value);
        SetPrivateProperty(customFieldItem, nameof(CustomFieldItem.ValueId), valueId);
        return customFieldItem;
    }

    private static CustomFieldOption CreateCustomFieldOption(string id)
    {
        CustomFieldOption option = new();
        SetPrivateProperty(option, nameof(CustomFieldOption.Id), id);
        return option;
    }

    private static void SetPrivateProperty<T>(T instance, string propertyName, object? value)
    {
        typeof(T).GetProperty(propertyName)!.SetValue(instance, value);
    }
}

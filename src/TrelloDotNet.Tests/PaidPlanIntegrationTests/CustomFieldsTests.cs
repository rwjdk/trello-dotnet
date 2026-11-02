using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Options.GetListOptions;

namespace TrelloDotNet.Tests.PaidPlanIntegrationTests;

public class CustomFieldsTests : TestBase
{
    /// <summary>
    /// Tests custom field values and filtering on a paid Board.
    /// </summary>
    [Fact(Skip = "No test Board with Custom Fields is available.")]
    public async Task CustomFieldsAsync()
    {
        Board? board = await GetSpecialPaidSubscriptionBoard();
        if (board == null)
        {
            return; //Special Test-board not available
        }

        const string listPrefix = "UnitTestCustomFields";
        await CleanUp(board, listPrefix);
        try
        {
            List<CustomField>? customFields = await TrelloClient.GetCustomFieldsOnBoardAsync(board.Id, cancellationToken: TestCancellationToken);
            Assert.Equal(5, customFields.Count);
            CustomField? fieldList = customFields.FirstOrDefault(x => x.Name == "Priority");
            CustomField? fieldCheckbox = customFields.FirstOrDefault(x => x.Name == "IsSomething");
            CustomField? fieldDate = customFields.FirstOrDefault(x => x.Name == "SomeDate");
            CustomField? fieldNumber = customFields.FirstOrDefault(x => x.Name == "SomeNumber");
            CustomField? fieldText = customFields.FirstOrDefault(x => x.Name == "SomeText");

            Assert.NotNull(fieldList);
            Assert.Equal(6, fieldList.Options.Count);
            Assert.NotNull(fieldCheckbox);
            Assert.NotNull(fieldDate);
            Assert.NotNull(fieldNumber);
            Assert.NotNull(fieldText);

            DateTimeOffset dateValue = DateTimeOffset.UtcNow;
            const decimal numberValue = 42.33M;
            const bool checkboxValue = true;
            const string textValue = "Hello World";
            CustomFieldOption listValue = fieldList.Options[2];

            List listForCustomFieldTests = await TrelloClient.AddListAsync(new List(listPrefix + Guid.NewGuid(), board.Id), cancellationToken: TestCancellationToken);
            Card card1 = await AddDummyCardToList(listForCustomFieldTests, "Card 1");

            List<CustomFieldItem>? customValues = await TrelloClient.GetCustomFieldItemsForCardAsync(card1.Id, cancellationToken: TestCancellationToken);
            Assert.Empty(customValues);

            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card1.Id, fieldList, listValue, cancellationToken: TestCancellationToken);
            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card1.Id, fieldCheckbox, checkboxValue, cancellationToken: TestCancellationToken);
            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card1.Id, fieldNumber, Convert.ToInt32(numberValue), cancellationToken: TestCancellationToken);
            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card1.Id, fieldNumber, numberValue, cancellationToken: TestCancellationToken);
            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card1.Id, fieldDate, dateValue, cancellationToken: TestCancellationToken);
            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card1.Id, fieldText, textValue, cancellationToken: TestCancellationToken);

            customValues = await TrelloClient.GetCustomFieldItemsForCardAsync(card1.Id, cancellationToken: TestCancellationToken);
            Assert.Equal(5, customValues.Count);
            CustomFieldItem? valueList = customValues.FirstOrDefault(x => x.CustomFieldId == fieldList.Id && x.ValueId == listValue.Id);
            CustomFieldItem? valueCheckbox = customValues.FirstOrDefault(x => x.CustomFieldId == fieldCheckbox.Id && x.Value.Checked);
            CustomFieldItem? valueDate = customValues.FirstOrDefault(x => x.CustomFieldId == fieldDate.Id && x.Value.Date?.ToUnixTimeSeconds() == dateValue.ToUnixTimeSeconds());
            CustomFieldItem? valueNumber = customValues.FirstOrDefault(x => x.CustomFieldId == fieldNumber.Id && x.Value.Number == numberValue);
            CustomFieldItem? valueText = customValues.FirstOrDefault(x => x.CustomFieldId == fieldText.Id && x.Value.TextAsString == textValue);

            Assert.NotNull(valueList);
            Assert.NotNull(valueCheckbox);
            Assert.NotNull(valueDate);
            Assert.NotNull(valueNumber);
            Assert.NotNull(valueText);

            Assert.Equal(checkboxValue, customValues.GetCustomFieldValueAsBoolean(fieldCheckbox));
            Assert.Equal(dateValue.ToUnixTimeSeconds(), customValues.GetCustomFieldValueAsDateTimeOffset(fieldDate)?.ToUnixTimeSeconds());
            Assert.Equal(numberValue, customValues.GetCustomFieldValueAsDecimal(fieldNumber));
            Assert.Equal(Convert.ToInt32(numberValue), customValues.GetCustomFieldValueAsInteger(fieldNumber));
            Assert.Equal(listValue.Id, customValues.GetCustomFieldValueAsOption(fieldList).Id);
            Assert.Equal(textValue, customValues.GetCustomFieldValueAsString(fieldText));
            Assert.NotNull(customValues.GetCustomFieldValueAsObject(fieldText));

            Assert.Throws<ArgumentOutOfRangeException>(() => customValues.GetCustomFieldValueAsDateTimeOffset(fieldCheckbox));
            Assert.Throws<ArgumentOutOfRangeException>(() => customValues.GetCustomFieldValueAsDateTimeOffset(fieldNumber));
            Assert.Throws<ArgumentOutOfRangeException>(() => customValues.GetCustomFieldValueAsDateTimeOffset(fieldList));
            Assert.Throws<ArgumentOutOfRangeException>(() => customValues.GetCustomFieldValueAsDateTimeOffset(fieldText));
            Assert.Throws<ArgumentOutOfRangeException>(() => customValues.GetCustomFieldValueAsInteger(fieldDate));

            List<Card>? cards = await TrelloClient.GetCardsOnBoardAsync(board.Id, new GetCardOptions
            {
                FilterConditions =
                [
                    CardsFilterCondition.ListId(CardsConditionIds.Equal, listForCustomFieldTests.Id),

                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.Equal, textValue),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.NotEqual, Guid.NewGuid().ToString()),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.AnyOfThese, textValue, Guid.NewGuid().ToString()),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.NoneOfThese, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.StartsWith, textValue),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.DoNotStartWith, Guid.NewGuid().ToString()),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.EndsWith, textValue),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.DoNotEndWith, Guid.NewGuid().ToString()),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.RegEx, textValue),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.Contains, textValue),
                    CardsFilterCondition.CustomField(fieldText, CardsConditionString.DoNotContains, Guid.NewGuid().ToString()),

                    CardsFilterCondition.CustomField(fieldCheckbox, CardsConditionBoolean.Equal, checkboxValue),
                    CardsFilterCondition.CustomField(fieldCheckbox, CardsConditionBoolean.NotEqual, !checkboxValue),

                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.Equal, dateValue),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.NotEqual, dateValue.AddDays(1)),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.Between, dateValue.AddDays(-1), dateValue.AddDays(1)),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.NotBetween, DateTimeOffset.MinValue, dateValue.AddDays(-1)),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.GreaterThan, dateValue.AddDays(-1)),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.GreaterThanOrEqual, dateValue.AddDays(-1)),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.LessThan, dateValue.AddDays(1)),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.LessThanOrEqual, dateValue.AddDays(1)),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.AnyOfThese, dateValue, dateValue.AddDays(1)),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.HasAnyValue),
                    CardsFilterCondition.CustomField(fieldDate, CardsConditionDate.NoneOfThese, DateTimeOffset.MinValue, DateTimeOffset.MaxValue),

                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.Equal, numberValue),
                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.NotEqual, -numberValue),
                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.Between, numberValue - 1, numberValue + 1),
                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.NotBetween, decimal.MinValue, numberValue - 1),
                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.GreaterThan, numberValue - 1),
                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.GreaterThanOrEqual, numberValue - 1),
                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.LessThan, numberValue + 1),
                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.LessThanOrEqual, numberValue + 1),
                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.HasAnyValue, Array.Empty<decimal>()),
                    CardsFilterCondition.CustomField(fieldNumber, CardsConditionNumber.AnyOfThese, numberValue, numberValue + 1),

                    CardsFilterCondition.CustomField(fieldList, CardsConditionString.Equal, listValue.Id),
                    CardsFilterCondition.CustomField(fieldList, CardsConditionString.NotEqual, Guid.NewGuid().ToString()),
                ]
            }, cancellationToken: TestCancellationToken);
            Assert.Single(cards);
            Assert.Equal(card1.Id, cards.First().Id);

            await TrelloClient.ClearCustomFieldValueOnCardAsync(card1.Id, fieldList, cancellationToken: TestCancellationToken);
            await TrelloClient.ClearCustomFieldValueOnCardAsync(card1.Id, fieldCheckbox, cancellationToken: TestCancellationToken);
            await TrelloClient.ClearCustomFieldValueOnCardAsync(card1.Id, fieldNumber, cancellationToken: TestCancellationToken);
            await TrelloClient.ClearCustomFieldValueOnCardAsync(card1.Id, fieldDate, cancellationToken: TestCancellationToken);
            await TrelloClient.ClearCustomFieldValueOnCardAsync(card1.Id, fieldText, cancellationToken: TestCancellationToken);

            customValues = await TrelloClient.GetCustomFieldItemsForCardAsync(card1.Id, cancellationToken: TestCancellationToken);
            Assert.Empty(customValues);


            await TrelloClient.AddCardAsync(new AddCardOptions
            {
                ListId = listForCustomFieldTests.Id,
                Name = "Add with Custom Fields",
                CustomFields =
                [
                    new AddCardOptionsCustomField(fieldCheckbox, checkboxValue),
                    new AddCardOptionsCustomField(fieldDate, dateValue),
                    new AddCardOptionsCustomField(fieldList, listValue),
                    new AddCardOptionsCustomField(fieldNumber, Convert.ToInt32(numberValue)),
                    new AddCardOptionsCustomField(fieldNumber, numberValue),
                    new AddCardOptionsCustomField(fieldText, textValue)
                ]
            }, cancellationToken: TestCancellationToken);
        }
        finally
        {
            await CleanUp(board, listPrefix);
        }
    }

    private async Task CleanUp(Board board, string listPrefix)
    {
        //Potential previous cleanup
        List<List>? lists = await TrelloClient.GetListsOnBoardAsync(board.Id, new GetListOptions
        {
            Filter = ListFilter.All
        }, cancellationToken: TestCancellationToken);
        foreach (List list in lists.Where(x => x.Name.StartsWith(listPrefix)))
        {
            await TrelloClient.DeleteListAsync(list.Id, cancellationToken: TestCancellationToken);
        }
    }
}

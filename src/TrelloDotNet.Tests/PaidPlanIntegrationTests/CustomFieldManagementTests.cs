using TrelloDotNet.Model;
using TrelloDotNet.Model.Options.AddCustomFieldOptions;
using TrelloDotNet.Model.Options.UpdateCustomFieldOptions;

namespace TrelloDotNet.Tests.PaidPlanIntegrationTests;

public class CustomFieldManagementTests : TestBase
{
    [Fact]
    public async Task AddUpdateAndDeleteCustomFieldsAndOptions()
    {
        Board? board = await GetSpecialPaidSubscriptionBoard();
        if (board == null)
        {
            return; //Special Test-board not available
        }

        string fieldPrefix = "UnitTestCustomField-" + Guid.NewGuid();
        List<string> customFieldIdsToDelete = [];

        try
        {
            CustomField textField = await TrelloClient.AddCustomFieldAsync(board.Id, new AddCustomFieldOptions
            {
                Name = fieldPrefix + "-Text",
                Type = CustomFieldType.Text,
                ShowFieldOnFrontOfCard = false
            }, TestCancellationToken);
            customFieldIdsToDelete.Add(textField.Id);

            Assert.Equal(fieldPrefix + "-Text", textField.Name);
            Assert.Equal(CustomFieldType.Text, textField.Type);
            Assert.False(textField.Display.ShowFieldOnFrontOfCard);

            CustomField updatedTextField = await TrelloClient.UpdateCustomFieldAsync(textField.Id, new UpdateCustomFieldOptions
            {
                Name = fieldPrefix + "-Text-Updated",
                ShowFieldOnFrontOfCard = true,
                Position = 1
            }, TestCancellationToken);

            Assert.Equal(fieldPrefix + "-Text-Updated", updatedTextField.Name);
            Assert.True(updatedTextField.Display.ShowFieldOnFrontOfCard);

            CustomField movedTextField = await TrelloClient.UpdateCustomFieldAsync(textField.Id, new UpdateCustomFieldOptions
            {
                NamedPosition = NamedPosition.Bottom
            }, TestCancellationToken);

            Assert.Equal(textField.Id, movedTextField.Id);

            CustomField listField = await TrelloClient.AddCustomFieldAsync(board.Id, new AddCustomFieldOptions
            {
                Name = fieldPrefix + "-List",
                Type = CustomFieldType.List,
                Options =
                [
                    new AddCustomFieldOption("Low", CustomFieldOptionColor.Green),
                    new AddCustomFieldOption("High", CustomFieldOptionColor.Red)
                ]
            }, TestCancellationToken);
            customFieldIdsToDelete.Add(listField.Id);

            Assert.Equal(CustomFieldType.List, listField.Type);
            Assert.Equal(2, listField.Options.Count);
            Assert.Contains(listField.Options, x => x.Value.Text == "Low" && x.Color == CustomFieldOptionColor.Green);
            Assert.Contains(listField.Options, x => x.Value.Text == "High" && x.Color == CustomFieldOptionColor.Red);

            await TrelloClient.AddCustomFieldOptionAsync(listField.Id, new AddCustomFieldOption("Medium", CustomFieldOptionColor.Yellow), TestCancellationToken);
            CustomField listFieldAfterAddOption = await GetCustomFieldAsync(board.Id, listField.Id);
            CustomFieldOption mediumOption = Assert.Single(listFieldAfterAddOption.Options, x => x.Value.Text == "Medium");
            Assert.Equal(CustomFieldOptionColor.Yellow, mediumOption.Color);

            await TrelloClient.UpdateCustomFieldOptionAsync(listField.Id, mediumOption.Id, new UpdateCustomFieldOption
            {
                Text = "Medium Updated",
                Color = CustomFieldOptionColor.Blue,
                Position = 1
            }, TestCancellationToken);
            CustomField listFieldAfterUpdateOption = await GetCustomFieldAsync(board.Id, listField.Id);
            CustomFieldOption updatedMediumOption = Assert.Single(listFieldAfterUpdateOption.Options, x => x.Id == mediumOption.Id);
            Assert.Equal("Medium Updated", updatedMediumOption.Value.Text);
            Assert.Equal(CustomFieldOptionColor.Blue, updatedMediumOption.Color);

            await TrelloClient.UpdateCustomFieldOptionAsync(listField.Id, updatedMediumOption.Id, new UpdateCustomFieldOption
            {
                NamedPosition = NamedPosition.Bottom
            }, TestCancellationToken);

            CustomField listFieldAfterNamedPositionUpdate = await GetCustomFieldAsync(board.Id, listField.Id);
            Assert.Contains(listFieldAfterNamedPositionUpdate.Options, x => x.Id == updatedMediumOption.Id);

            await TrelloClient.DeleteCustomFieldOptionAsync(listField.Id, updatedMediumOption.Id, TestCancellationToken);
            CustomField listFieldAfterDeleteOption = await GetCustomFieldAsync(board.Id, listField.Id);
            Assert.DoesNotContain(listFieldAfterDeleteOption.Options, x => x.Id == updatedMediumOption.Id);

            await TrelloClient.DeleteCustomFieldAsync(textField.Id, TestCancellationToken);
            customFieldIdsToDelete.Remove(textField.Id);

            List<CustomField> customFieldsAfterDelete = await TrelloClient.GetCustomFieldsOnBoardAsync(board.Id, TestCancellationToken);
            Assert.DoesNotContain(customFieldsAfterDelete, x => x.Id == textField.Id);
        }
        finally
        {
            foreach (string customFieldId in customFieldIdsToDelete)
            {
                try
                {
                    await TrelloClient.DeleteCustomFieldAsync(customFieldId, TestCancellationToken);
                }
                catch
                {
                    //Best-effort cleanup; the test assertions above report the actual failure.
                }
            }
        }
    }

    [Fact]
    public async Task AddCustomFieldValidatesRequiredListOptionsAndType()
    {
        Board? board = await GetSpecialPaidSubscriptionBoard();
        if (board == null)
        {
            return; //Special Test-board not available
        }

        TrelloApiException noOptionsException = await Assert.ThrowsAsync<TrelloApiException>(async () =>
            await TrelloClient.AddCustomFieldAsync(board.Id, new AddCustomFieldOptions
            {
                Name = "UnitTestCustomField-NoOptions-" + Guid.NewGuid(),
                Type = CustomFieldType.List
            }, TestCancellationToken));

        Assert.Equal("No option items defined for the custom field of type list (need at least one)", noOptionsException.Message);

        TrelloApiException noTypeException = await Assert.ThrowsAsync<TrelloApiException>(async () =>
            await TrelloClient.AddCustomFieldAsync(board.Id, new AddCustomFieldOptions
            {
                Name = "UnitTestCustomField-NoType-" + Guid.NewGuid()
            }, TestCancellationToken));

        Assert.Equal("Custom Field Type have not been defined", noTypeException.Message);
    }

    [Fact]
    public async Task UpdateCustomFieldValuesOnCardUsingStringOverloads()
    {
        Board? board = await GetSpecialPaidSubscriptionBoard();
        if (board == null)
        {
            return; //Special Test-board not available
        }

        const string listPrefix = "UnitTestCustomFieldStringValues";
        await CleanUp(board, listPrefix);
        try
        {
            List<CustomField> customFields = await TrelloClient.GetCustomFieldsOnBoardAsync(board.Id, TestCancellationToken);
            CustomField fieldList = customFields.Single(x => x.Name == "Priority");
            CustomField fieldCheckbox = customFields.Single(x => x.Name == "IsSomething");
            CustomField fieldDate = customFields.Single(x => x.Name == "SomeDate");
            CustomField fieldNumber = customFields.Single(x => x.Name == "SomeNumber");
            CustomField fieldText = customFields.Single(x => x.Name == "SomeText");
            CustomFieldOption listOption = fieldList.Options.First();

            List list = await TrelloClient.AddListAsync(new List(listPrefix + Guid.NewGuid(), board.Id), cancellationToken: TestCancellationToken);
            Card card = await AddDummyCardToList(list, "String Values");

            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card.Id, fieldCheckbox, "true", TestCancellationToken);
            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card.Id, fieldDate, "2099-01-01T12:00:00.000Z", TestCancellationToken);
            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card.Id, fieldList, listOption.Id, TestCancellationToken);
            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card.Id, fieldNumber, "42.33", TestCancellationToken);
            await TrelloClient.UpdateCustomFieldValueOnCardAsync(card.Id, fieldText, "Hello from string overload", TestCancellationToken);

            List<CustomFieldItem> customValues = await TrelloClient.GetCustomFieldItemsForCardAsync(card.Id, TestCancellationToken);
            Assert.True(customValues.GetCustomFieldValueAsBoolean(fieldCheckbox));
            Assert.Equal(new DateTimeOffset(2099, 1, 1, 12, 0, 0, TimeSpan.Zero), customValues.GetCustomFieldValueAsDateTimeOffset(fieldDate));
            Assert.Equal(listOption.Id, customValues.GetCustomFieldValueAsOption(fieldList).Id);
            Assert.Equal(42.33M, customValues.GetCustomFieldValueAsDecimal(fieldNumber));
            Assert.Equal("Hello from string overload", customValues.GetCustomFieldValueAsString(fieldText));
        }
        finally
        {
            await CleanUp(board, listPrefix);
        }
    }

    [Fact]
    public async Task UpdateCustomFieldValueOnCardRejectsMismatchedTypedValues()
    {
        Board? board = await GetSpecialPaidSubscriptionBoard();
        if (board == null)
        {
            return; //Special Test-board not available
        }

        List<CustomField> customFields = await TrelloClient.GetCustomFieldsOnBoardAsync(board.Id, TestCancellationToken);
        CustomField fieldText = customFields.Single(x => x.Name == "SomeText");
        CustomField fieldCheckbox = customFields.Single(x => x.Name == "IsSomething");
        CustomField fieldDate = customFields.Single(x => x.Name == "SomeDate");
        CustomField fieldNumber = customFields.Single(x => x.Name == "SomeNumber");
        CustomField fieldList = customFields.Single(x => x.Name == "Priority");

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await TrelloClient.UpdateCustomFieldValueOnCardAsync("cardId", fieldText, true, TestCancellationToken));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await TrelloClient.UpdateCustomFieldValueOnCardAsync("cardId", fieldCheckbox, DateTimeOffset.UtcNow, TestCancellationToken));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await TrelloClient.UpdateCustomFieldValueOnCardAsync("cardId", fieldDate, 1, TestCancellationToken));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await TrelloClient.UpdateCustomFieldValueOnCardAsync("cardId", fieldList, 1.5M, TestCancellationToken));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await TrelloClient.UpdateCustomFieldValueOnCardAsync("cardId", fieldNumber, fieldList.Options.First(), TestCancellationToken));
    }

    private async Task<CustomField> GetCustomFieldAsync(string boardId, string customFieldId)
    {
        List<CustomField> customFields = await TrelloClient.GetCustomFieldsOnBoardAsync(boardId, TestCancellationToken);
        return customFields.Single(x => x.Id == customFieldId);
    }

    private async Task CleanUp(Board board, string listPrefix)
    {
        List<List>? lists = await TrelloClient.GetListsOnBoardAsync(board.Id, cancellationToken: TestCancellationToken);
        foreach (List list in lists.Where(x => x.Name.StartsWith(listPrefix)))
        {
            await TrelloClient.DeleteListAsync(list.Id, cancellationToken: TestCancellationToken);
        }
    }
}

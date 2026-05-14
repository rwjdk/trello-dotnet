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

    private async Task<CustomField> GetCustomFieldAsync(string boardId, string customFieldId)
    {
        List<CustomField> customFields = await TrelloClient.GetCustomFieldsOnBoardAsync(boardId, TestCancellationToken);
        return customFields.Single(x => x.Id == customFieldId);
    }
}

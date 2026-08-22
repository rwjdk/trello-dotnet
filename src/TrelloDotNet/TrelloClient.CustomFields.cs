using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TrelloDotNet.Control;
using TrelloDotNet.Model;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Update a Custom field on a Card
        /// </summary>
        /// <remarks>
        /// Tip: To remove a value from a custom field use .ClearCustomFieldValueOnCardAsync()
        /// </remarks>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="customField">The custom Field to update</param>
        /// <param name="newValue">The new value</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task UpdateCustomFieldValueOnCardAsync(string cardId, CustomField customField, bool newValue, CancellationToken cancellationToken = default)
        {
            string payload;
            switch (customField.Type)
            {
                case CustomFieldType.Checkbox:
                    string valueAsString = newValue ? "true" : "false";
                    payload = $"{{\"value\": {{ \"checked\": \"{HttpUtility.JavaScriptStringEncode(valueAsString)}\" }}}}";
                    break;
                case CustomFieldType.Date:
                case CustomFieldType.List:
                case CustomFieldType.Number:
                case CustomFieldType.Text:
                default:
                    throw new ArgumentOutOfRangeException(nameof(customField), "Only a custom field of type 'Checkbox' can be set with a bool value");
            }

            await SendCustomFieldChangeRequestAsync(cardId, customField, payload, cancellationToken);
        }

        /// <summary>
        /// Update a Custom field on a Card
        /// </summary>
        /// <remarks>
        /// Tip: To remove a value from a custom field use .ClearCustomFieldValueOnCardAsync()
        /// </remarks>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="customField">The custom Field to update</param>
        /// <param name="newValue">The new value</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task UpdateCustomFieldValueOnCardAsync(string cardId, CustomField customField, DateTimeOffset newValue, CancellationToken cancellationToken = default)
        {
            string payload;
            switch (customField.Type)
            {
                case CustomFieldType.Date:
                    string valueAsString = newValue.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
                    payload = $"{{\"value\": {{ \"date\": \"{HttpUtility.JavaScriptStringEncode(valueAsString)}\" }}}}";
                    break;
                case CustomFieldType.Checkbox:
                case CustomFieldType.List:
                case CustomFieldType.Number:
                case CustomFieldType.Text:
                default:
                    throw new ArgumentOutOfRangeException(nameof(customField), "Only a custom field of type 'Date' can be set with a DateTimeOffset value");
            }

            await SendCustomFieldChangeRequestAsync(cardId, customField, payload, cancellationToken);
        }

        /// <summary>
        /// Update a Custom field on a Card
        /// </summary>
        /// <remarks>
        /// Tip: To remove a value from a custom field use .ClearCustomFieldValueOnCardAsync()
        /// </remarks>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="customField">The custom Field to update</param>
        /// <param name="newValue">The new value</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task UpdateCustomFieldValueOnCardAsync(string cardId, CustomField customField, int newValue, CancellationToken cancellationToken = default)
        {
            string payload;
            switch (customField.Type)
            {
                case CustomFieldType.Number:
                    string valueAsString = newValue.ToString(CultureInfo.InvariantCulture);
                    payload = $"{{\"value\": {{ \"number\": \"{HttpUtility.JavaScriptStringEncode(valueAsString)}\" }}}}";
                    break;
                case CustomFieldType.Checkbox:
                case CustomFieldType.Date:
                case CustomFieldType.List:
                case CustomFieldType.Text:
                default:
                    throw new ArgumentOutOfRangeException(nameof(customField), "Only a custom field of type 'Number' can be set with a integer value");
            }

            await SendCustomFieldChangeRequestAsync(cardId, customField, payload, cancellationToken);
        }

        /// <summary>
        /// Update a Custom field on a Card
        /// </summary>
        /// <remarks>
        /// Tip: To remove a value from a custom field use .ClearCustomFieldValueOnCardAsync()
        /// </remarks>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="customField">The custom Field to update</param>
        /// <param name="newValue">The new value</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task UpdateCustomFieldValueOnCardAsync(string cardId, CustomField customField, decimal newValue, CancellationToken cancellationToken = default)
        {
            string payload;
            switch (customField.Type)
            {
                case CustomFieldType.Number:
                    string valueAsString = newValue.ToString(CultureInfo.InvariantCulture);
                    payload = $"{{\"value\": {{ \"number\": \"{HttpUtility.JavaScriptStringEncode(valueAsString)}\" }}}}";
                    break;
                case CustomFieldType.Checkbox:
                case CustomFieldType.Date:
                case CustomFieldType.List:
                case CustomFieldType.Text:
                default:
                    throw new ArgumentOutOfRangeException(nameof(customField), "Only a custom field of type 'Number' can be set with a decimal value");
            }

            await SendCustomFieldChangeRequestAsync(cardId, customField, payload, cancellationToken);
        }

        /// <summary>
        /// Update a Custom field on a Card
        /// </summary>
        /// <remarks>
        /// Tip: To remove a value from a custom field use .ClearCustomFieldValueOnCardAsync()
        /// </remarks>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="customField">The custom Field to update</param>
        /// <param name="newValue">The new value</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task UpdateCustomFieldValueOnCardAsync(string cardId, CustomField customField, CustomFieldOption newValue, CancellationToken cancellationToken = default)
        {
            string payload;
            switch (customField.Type)
            {
                case CustomFieldType.List:
                    string valueAsString = string.Empty;
                    if (newValue != null)
                    {
                        valueAsString = newValue.Id;
                    }

                    payload = $"{{\"idValue\": \"{HttpUtility.JavaScriptStringEncode(valueAsString)}\" }}";
                    break;
                case CustomFieldType.Checkbox:
                case CustomFieldType.Date:
                case CustomFieldType.Number:
                case CustomFieldType.Text:
                default:
                    throw new ArgumentOutOfRangeException(nameof(customField), "Only a custom field of type 'List' can be set with a CustomFieldOption value");
            }

            await SendCustomFieldChangeRequestAsync(cardId, customField, payload, cancellationToken);
        }

        /// <summary>
        /// Update a Custom field on a Card
        /// </summary>
        /// <remarks>
        /// Tip: To remove a value from a custom field use .ClearCustomFieldValueOnCardAsync()
        /// </remarks>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="customField">The custom Field to update</param>
        /// <param name="newValue">The new value</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task UpdateCustomFieldValueOnCardAsync(string cardId, CustomField customField, string newValue, CancellationToken cancellationToken = default)
        {
            string payload;
            switch (customField.Type)
            {
                case CustomFieldType.Checkbox:
                    payload = $"{{\"value\": {{ \"checked\": \"{HttpUtility.JavaScriptStringEncode(newValue)}\" }}}}";
                    break;
                case CustomFieldType.Date:
                    payload = $"{{\"value\": {{ \"date\": \"{HttpUtility.JavaScriptStringEncode(newValue)}\" }}}}";
                    break;
                case CustomFieldType.List:
                    payload = $"{{\"idValue\": \"{HttpUtility.JavaScriptStringEncode(newValue)}\" }}";
                    break;
                case CustomFieldType.Number:
                    payload = $"{{\"value\": {{ \"number\": \"{HttpUtility.JavaScriptStringEncode(newValue)}\" }}}}";
                    break;
                case CustomFieldType.Text:
                    payload = $"{{\"value\": {{ \"text\": \"{HttpUtility.JavaScriptStringEncode(newValue)}\" }}}}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await SendCustomFieldChangeRequestAsync(cardId, customField, payload, cancellationToken);
        }

        /// <summary>
        /// Clear a Custom field on a Card
        /// </summary>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="customField">The custom Field to clear</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task ClearCustomFieldValueOnCardAsync(string cardId, CustomField customField, CancellationToken cancellationToken = default)
        {
            string payload;
            switch (customField.Type)
            {
                case CustomFieldType.Checkbox:
                case CustomFieldType.Date:
                case CustomFieldType.Number:
                case CustomFieldType.Text:
                    payload = "{\"value\": \"\" }";
                    break;
                case CustomFieldType.List:
                    payload = "{\"idValue\": \"\" }";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await SendCustomFieldChangeRequestAsync(cardId, customField, payload, cancellationToken);
        }

        private async Task SendCustomFieldChangeRequestAsync(string cardId, CustomField customField, string payload, CancellationToken cancellationToken)
        {
            await _apiRequestController.PutWithJsonPayload($"{UrlPaths.Cards}/{cardId}/customField/{customField.Id}/item", cancellationToken, payload, 0);
        }

        /// <summary>
        /// Get Custom Field Item Values for a Card
        /// </summary>
        /// <remarks>Tip: Use Extension methods GetCustomFieldValueAsXYZ for a handy way to get values</remarks>
        /// <param name="cardId">Id of the Card</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The Custom Fields</returns>
        public async Task<List<CustomFieldItem>> GetCustomFieldItemsForCardAsync(string cardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<CustomFieldItem>>(GetUrlBuilder.GetCustomFieldItemsForCard(cardId), cancellationToken);
        }

        /// <summary>
        /// Get Custom Fields of a Board
        /// </summary>
        /// <param name="boardId">Id of the Board (long version)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of CustomFields</returns>
        public async Task<List<CustomField>> GetCustomFieldsOnBoardAsync(string boardId, CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<List<CustomField>>(GetUrlBuilder.GetCustomFieldsOnBoard(boardId), cancellationToken);
        }
    }
}
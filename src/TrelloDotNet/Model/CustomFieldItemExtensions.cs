using System.Globalization;
using System.Linq;
using System;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet.Model
{
    /// <summary>
    /// Extension Method to Get Values from Custom Values
    /// </summary>
    public static class CustomFieldItemsListExtensions
    {
        /// <summary>
        /// Get Custom Field Value as Integer
        /// </summary>
        /// <param name="items">The Items</param>
        /// <param name="customField">Custom Field to get value for</param>
        /// <returns>Value</returns>
        public static int? GetCustomFieldValueAsInteger(this List<CustomFieldItem> items, CustomField customField)
        {
            object @object = items.GetCustomFieldValueAsObject(customField);
            if (@object == null)
            {
                return null;
            }

            switch (customField.Type)
            {
                case CustomFieldType.Number:
                    return Convert.ToInt32(Convert.ToDecimal(@object.ToString(), CultureInfo.InvariantCulture));
                default:
                    throw new ArgumentOutOfRangeException($"Custom field of type '{customField.Type}' can't be converted to a integer");
            }
        }

        /// <summary>
        /// Get Custom Field Value as Decimal
        /// </summary>
        /// <param name="items">The Items</param>
        /// <param name="customField">Custom Field to get value for</param>
        /// <returns>Value</returns>
        public static decimal? GetCustomFieldValueAsDecimal(this List<CustomFieldItem> items, CustomField customField)
        {
            object @object = items.GetCustomFieldValueAsObject(customField);
            if (@object == null)
            {
                return null;
            }

            switch (customField.Type)
            {
                case CustomFieldType.Number:
                    return Convert.ToDecimal(@object.ToString(), CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentOutOfRangeException($"Custom field of type '{customField.Type}' can't be converted to a decimal");
            }
        }

        /// <summary>
        /// Get Custom Field Value as Boolean
        /// </summary>
        /// <param name="items">The Items</param>
        /// <param name="customField">Custom Field to get value for</param>
        /// <returns>Value</returns>
        public static bool? GetCustomFieldValueAsBoolean(this List<CustomFieldItem> items, CustomField customField)
        {
            object @object = items.GetCustomFieldValueAsObject(customField);
            if (@object == null)
            {
                return null;
            }

            switch (customField.Type)
            {
                case CustomFieldType.Checkbox:
                    return @object.ToString() == "true";
                default:
                    throw new ArgumentOutOfRangeException($"Custom field of type '{customField.Type}' can't be converted to a boolean");
            }
        }

        /// <summary>
        /// Get Custom Field Value as DateTimeOffset
        /// </summary>
        /// <param name="items">The Items</param>
        /// <param name="customField">Custom Field to get value for</param>
        /// <returns>Value</returns>
        public static DateTimeOffset? GetCustomFieldValueAsDateTimeOffset(this List<CustomFieldItem> items, CustomField customField)
        {
            object @object = items.GetCustomFieldValueAsObject(customField);
            if (@object == null)
            {
                return null;
            }

            switch (customField.Type)
            {
                case CustomFieldType.Date:
                    string dateAsString = @object.ToString();
                    return DateTimeOffset.ParseExact(dateAsString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentOutOfRangeException($"Custom field of type '{customField.Type}' can't be converted to a Date");
            }
        }

        /// <summary>
        /// Get Custom Field Value as Option
        /// </summary>
        /// <param name="items">The Items</param>
        /// <param name="customField">Custom Field to get value for</param>
        /// <returns>Value</returns>
        public static CustomFieldOption GetCustomFieldValueAsOption(this List<CustomFieldItem> items, CustomField customField)
        {
            object @object = items.GetCustomFieldValueAsObject(customField);
            if (@object == null)
            {
                return null;
            }

            switch (customField.Type)
            {
                case CustomFieldType.List:
                    return (CustomFieldOption)@object;
                default:
                    throw new ArgumentOutOfRangeException($"Custom field of type '{customField.Type}' can't be converted to a CustomFieldOption");
            }
        }

        /// <summary>
        /// Get Custom Field Value as String
        /// </summary>
        /// <param name="items">The Items</param>
        /// <param name="customField">Custom Field to get value for</param>
        /// <returns>Value</returns>
        public static string GetCustomFieldValueAsString(this List<CustomFieldItem> items, CustomField customField)
        {
            object @object = items.GetCustomFieldValueAsObject(customField);
            if (@object == null)
            {
                return null;
            }

            switch (customField.Type)
            {
                case CustomFieldType.Checkbox:
                    return @object.ToString();
                case CustomFieldType.Date:
                    return @object.ToString();
                case CustomFieldType.List:
                    return ((CustomFieldOption)@object).Value?.Text;
                case CustomFieldType.Text:
                    return @object.ToString();
                case CustomFieldType.Number:
                    return @object.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static object GetCustomFieldValueAsObject(this List<CustomFieldItem> items, CustomField customField)
        {
            if (items == null)
            {
                throw new TrelloApiException("CustomFields are null. Did you remember to include them in the TrelloClientOptions?", null);
            }

            CustomFieldItem customFieldItem = items.FirstOrDefault(x => x.CustomFieldId == customField.Id);
            if (customFieldItem == null)
            {
                return null;
            }

            switch (customField.Type)
            {
                case CustomFieldType.Checkbox:
                    return customFieldItem.Value.CheckedAsString;
                case CustomFieldType.Date:
                    return customFieldItem.Value.DateAsString;
                case CustomFieldType.List:
                    return customField.Options.FirstOrDefault(x => x.Id == customFieldItem.ValueId);
                case CustomFieldType.Number:
                    return customFieldItem.Value.NumberAsString;
                case CustomFieldType.Text:
                    return customFieldItem.Value.TextAsString;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
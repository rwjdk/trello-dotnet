using System.Collections.Generic;

namespace TrelloDotNet.Model.Options.AddCustomFieldOptions
{
    /// <summary>
    /// Represent a new custom field
    /// </summary>
    public class AddCustomFieldOptions
    {
        /// <summary>
        /// Name of the Custom field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the Custom Field
        /// </summary>
        public CustomFieldType Type { get; set; } = CustomFieldType.Unknown;

        /// <summary>
        /// Show the Custom Field on front of Card (Default: true)
        /// </summary>
        public bool ShowFieldOnFrontOfCard { get; set; } = true;

        /// <summary>
        /// Options of DropDown List (only used if Type = List)
        /// </summary>
        public List<AddCustomFieldOption> Options { get; set; }
    }
}
using System.Collections.Generic;

namespace TrelloDotNet.Model.Options.UpdateCustomFieldOptions
{
    /// <summary>
    /// Represent a new custom field
    /// </summary>
    public class UpdateCustomFieldOptions
    {
        /// <summary>
        /// Updated Name of the Custom field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Show the Custom Field on front of Card (Default: true)
        /// </summary>
        public bool? ShowFieldOnFrontOfCard { get; set; } = true;

        /// <summary>
        /// Updated position
        /// </summary>
        public decimal? Position { get; set; }

        /// <summary>
        /// Updated named position
        /// </summary>
        public NamedPosition? NamedPosition { internal get; set; }
    }
}
namespace TrelloDotNet.Model.Options.UpdateCustomFieldOptions
{
    /// <summary>
    /// Update of an CustomFieldOption
    /// </summary>
    public class UpdateCustomFieldOption
    {
        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Updated position
        /// </summary>
        public decimal? Position { get; set; }

        /// <summary>
        /// Updated named position
        /// </summary>
        public NamedPosition? NamedPosition { internal get; set; }

        /// <summary>
        /// Updated Color
        /// </summary>
        public CustomFieldOptionColor? Color { get; set; }
    }
}
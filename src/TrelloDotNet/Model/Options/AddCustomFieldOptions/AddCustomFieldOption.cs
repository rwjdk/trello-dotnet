namespace TrelloDotNet.Model.Options.AddCustomFieldOptions
{
    /// <summary>
    /// Option of a Custom Field of type List
    /// </summary>
    public class AddCustomFieldOption
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AddCustomFieldOption()
        {
            //Empty
        }

        /// <summary>
        /// Constructor
        /// <param name="text">Text of the option</param>
        /// </summary>
        public AddCustomFieldOption(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Text of the option</param>
        /// <param name="color"></param>
        public AddCustomFieldOption(string text, CustomFieldOptionColor color)
        {
            Text = text;
            Color = color;
        }

        /// <summary>
        /// Text of the Option
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Color of the Option
        /// </summary>
        public CustomFieldOptionColor Color { get; set; } = CustomFieldOptionColor.None;
    }
}
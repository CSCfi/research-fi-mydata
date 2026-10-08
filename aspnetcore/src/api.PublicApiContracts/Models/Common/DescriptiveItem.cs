namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Text content in a single language (schema: descriptive_item).
    /// </summary>
    public class DescriptiveItem
    {
        /// <summary>Descriptive text.</summary>
        public string DescriptiveContent { get; set; }

        /// <summary>Language of the text.</summary>
        public string Language { get; set; }
    }
}

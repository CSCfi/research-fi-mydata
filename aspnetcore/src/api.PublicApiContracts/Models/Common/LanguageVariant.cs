namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Text in Finnish, Swedish and English (schema: language_variant).
    /// </summary>
    public class LanguageVariant
    {
        /// <summary>Finnish.</summary>
        public string Fi { get; set; }

        /// <summary>Swedish.</summary>
        public string Sv { get; set; }

        /// <summary>English.</summary>
        public string En { get; set; }
    }
}

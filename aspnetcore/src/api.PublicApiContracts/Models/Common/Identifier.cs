namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Persistent identifier (schema: identifier).
    /// </summary>
    public class Identifier
    {
        /// <summary>Persistent identifier value.</summary>
        public string PidContent { get; set; }

        /// <summary>Persistent identifier type, for example DOI or URN.</summary>
        public string PidType { get; set; }
    }
}

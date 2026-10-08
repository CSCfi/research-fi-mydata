namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Person's affiliation to an organization (schema: affiliation).
    /// </summary>
    public class Affiliation
    {
        /// <summary>Affiliation start date.</summary>
        public PartialDate AffiliationStartsOn { get; set; }

        /// <summary>Affiliation end date.</summary>
        public PartialDate AffiliationEndsOn { get; set; }

        /// <summary>Affiliation type.</summary>
        public LanguageVariant AffiliationType { get; set; }

        /// <summary>Organization the affiliation refers to.</summary>
        public Organization AffiliationOrganization { get; set; }

        /// <summary>Position title.</summary>
        public LanguageVariant Position { get; set; }
    }
}

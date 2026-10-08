namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Funding granted to the person (schema: granted_funding).
    /// </summary>
    public class GrantedFunding
    {
        /// <summary>Funding start date.</summary>
        public PartialDate FundingStartsOn { get; set; }

        /// <summary>Funder.</summary>
        public Organization IsFundedBy { get; set; }

        /// <summary>Funding end date.</summary>
        public PartialDate FundingEndsOn { get; set; }

        /// <summary>Funding name.</summary>
        public LanguageVariant GrantedFundingName { get; set; }
    }
}

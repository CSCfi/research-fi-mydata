namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Education (schema: education).
    /// </summary>
    public class Education
    {
        /// <summary>Education name.</summary>
        public LanguageVariant EducationName { get; set; }

        /// <summary>Education end date.</summary>
        public PartialDate EducationEndsOn { get; set; }

        /// <summary>Name of the degree-granting institution.</summary>
        public string DegreeGrantingInstitutionName { get; set; }

        /// <summary>Education start date.</summary>
        public PartialDate EducationStartsOn { get; set; }
    }
}

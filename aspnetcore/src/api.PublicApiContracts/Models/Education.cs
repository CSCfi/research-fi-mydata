using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Education (schema: education).
    /// </summary>
    public class Education
    {
        /// <summary>Education name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LanguageVariant EducationName { get; set; }

        /// <summary>Education end date.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PartialDate EducationEndsOn { get; set; }

        /// <summary>Name of the degree-granting institution.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string DegreeGrantingInstitutionName { get; set; }

        /// <summary>Education start date.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PartialDate EducationStartsOn { get; set; }
    }
}

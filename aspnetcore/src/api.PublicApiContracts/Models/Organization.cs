using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Organization (schema: organization).
    /// </summary>
    public class Organization
    {
        /// <summary>Organization identifiers, for example business ID or PIC.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Identifier> OrganizationIsIdentifiedBy { get; set; }

        /// <summary>Parent organization.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Organization IsPartOfOrganization { get; set; }

        /// <summary>Organization name.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LanguageVariant OrganizationName { get; set; }

        /// <summary>Sector the organization is classified into.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ReferenceData OrganizationSector { get; set; }
    }
}

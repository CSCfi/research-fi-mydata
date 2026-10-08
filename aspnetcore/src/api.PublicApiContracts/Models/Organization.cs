using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Organization (schema: organization).
    /// </summary>
    public class Organization
    {
        /// <summary>Organization identifiers, for example business ID or PIC.</summary>
        public List<Identifier> OrganizationIsIdentifiedBy { get; set; }

        /// <summary>Parent organization.</summary>
        public Organization IsPartOfOrganization { get; set; }

        /// <summary>Organization name.</summary>
        public LanguageVariant OrganizationName { get; set; }

        /// <summary>Sector the organization is classified into.</summary>
        public ReferenceData OrganizationSector { get; set; }
    }
}

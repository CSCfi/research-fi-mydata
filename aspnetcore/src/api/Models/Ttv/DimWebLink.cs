using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimWebLink
    {
        public DimWebLink()
        {
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public string Url { get; set; }
        public string LinkLabel { get; set; }
        public string LinkType { get; set; }
        public string LanguageVariant { get; set; }
        public int? DimOrganizationId { get; set; }
        public int? DimKnownPersonId { get; set; }
        public int? DimCallProgrammeId { get; set; }
        public int? DimFundingDecisionId { get; set; }
        public int? DimResearchDataCatalogId { get; set; }
        public int? DimResearchDatasetId { get; set; }
        public int? DimResearchCommunityId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}

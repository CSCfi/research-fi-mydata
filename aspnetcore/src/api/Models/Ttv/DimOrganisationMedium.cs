using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimOrganisationMedium
    {
        public int DimOrganizationId { get; set; }
        public string MediaUri { get; set; }
        public string LanguageVariant { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimOrganization DimOrganization { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimOrganisationMedia
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

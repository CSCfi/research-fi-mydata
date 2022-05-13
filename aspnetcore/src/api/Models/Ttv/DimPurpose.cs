using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimPurpose
    {
        public int Id { get; set; }
        public int DimOrganizationId { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string NameUnd { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimOrganization DimOrganization { get; set; }
    }
}

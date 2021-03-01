using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimOrganization
    {
        public DimOrganization()
        {
            DimPid = new HashSet<DimPid>();
            DimWebLink = new HashSet<DimWebLink>();
        }

        public int Id { get; set; }
        public int? DimOrganizationBroader { get; set; }
        public int? DimSectorid { get; set; }
        public string OrganizationId { get; set; }
        public bool? OrganizationActive { get; set; }
        public string LocalOrganizationUnitId { get; set; }
        public string LocalOrganizationSector { get; set; }
        public string OrganizationBackground { get; set; }
        public string OrganizationType { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string NameVariants { get; set; }
        public string NameUnd { get; set; }
        public string CountryCode { get; set; }
        public DateTime? Established { get; set; }
        public string VisitingAddress { get; set; }
        public string PostalAddress { get; set; }
        public int? StaffCountAsFte { get; set; }
        public int? DegreeCountBsc { get; set; }
        public int? DegreeCountMsc { get; set; }
        public int? DegreeCountLic { get; set; }
        public int? DegreeCountPhd { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<DimPid> DimPid { get; set; }
        public virtual ICollection<DimWebLink> DimWebLink { get; set; }
    }
}

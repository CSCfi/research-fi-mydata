using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimAffiliation
    {
        public DimAffiliation()
        {
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public int DimKnownPersonId { get; set; }
        public int DimOrganizationId { get; set; }
        public int StartDate { get; set; }
        public int? EndDate { get; set; }
        public int AffiliationType { get; set; }
        public int? PositionCode { get; set; }
        public string PositionNameFi { get; set; }
        public string PositionNameEn { get; set; }
        public string PositionNameSv { get; set; }
        public string SourceDescription { get; set; }
        public string SourceId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimReferencedatum AffiliationTypeNavigation { get; set; }
        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimOrganization DimOrganization { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual DimDate EndDateNavigation { get; set; }
        public virtual DimReferencedatum PositionCodeNavigation { get; set; }
        public virtual DimDate StartDateNavigation { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}

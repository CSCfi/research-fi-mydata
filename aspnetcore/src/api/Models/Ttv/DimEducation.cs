using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimEducation
    {
        public DimEducation()
        {
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public int? LocalIdentifier { get; set; }
        public float? Credits { get; set; }
        public string NameFi { get; set; }
        public string NameSv { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFi { get; set; }
        public string DescriptionSv { get; set; }
        public string DescriptionEn { get; set; }
        public string DegreeNameFi { get; set; }
        public string DegreeNameSv { get; set; }
        public string DegreeNameEn { get; set; }
        public string DegreeGrantingInstitutionName { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int? DimStartDate { get; set; }
        public int? DimEndDate { get; set; }
        public int? DimInstructionLanguage { get; set; }
        public int DimKnownPersonId { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimDate DimEndDateNavigation { get; set; }
        public virtual DimReferencedatum DimInstructionLanguageNavigation { get; set; }
        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual DimDate DimStartDateNavigation { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}

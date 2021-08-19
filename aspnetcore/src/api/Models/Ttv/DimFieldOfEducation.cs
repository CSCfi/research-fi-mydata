using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimFieldOfEducation
    {
        public DimFieldOfEducation()
        {
            BrFieldOfEducationDimPublications = new HashSet<BrFieldOfEducationDimPublication>();
        }

        public int Id { get; set; }
        public string FieldId { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<BrFieldOfEducationDimPublication> BrFieldOfEducationDimPublications { get; set; }
    }
}

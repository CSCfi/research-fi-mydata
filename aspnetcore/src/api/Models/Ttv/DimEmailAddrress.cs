using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimEmailAddrress
    {
        public DimEmailAddrress()
        {
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int DimRegisteredDataSourceId { get; set; }
        public int DimKnownPersonId { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}

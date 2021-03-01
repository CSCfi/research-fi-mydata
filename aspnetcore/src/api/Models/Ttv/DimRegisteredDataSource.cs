using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimRegisteredDataSource
    {
        public DimRegisteredDataSource()
        {
            BrFieldDisplaySettingsDimRegisteredDataSource = new HashSet<BrFieldDisplaySettingsDimRegisteredDataSource>();
        }

        public int Id { get; set; }
        public int DimOrganizationId { get; set; }
        public string Name { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }

        public virtual ICollection<BrFieldDisplaySettingsDimRegisteredDataSource> BrFieldDisplaySettingsDimRegisteredDataSource { get; set; }
    }
}

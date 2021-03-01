using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimFieldDisplaySettings
    {
        public DimFieldDisplaySettings()
        {
            BrFieldDisplaySettingsDimRegisteredDataSource = new HashSet<BrFieldDisplaySettingsDimRegisteredDataSource>();
            FactFieldValues = new HashSet<FactFieldValues>();
        }

        public int Id { get; set; }
        public int DimUserProfileId { get; set; }
        public int FieldIdentifier { get; set; }
        public bool Show { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimUserProfile DimUserProfile { get; set; }
        public virtual ICollection<BrFieldDisplaySettingsDimRegisteredDataSource> BrFieldDisplaySettingsDimRegisteredDataSource { get; set; }
        public virtual ICollection<FactFieldValues> FactFieldValues { get; set; }
    }
}

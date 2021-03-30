using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimFieldDisplaySetting
    {
        public DimFieldDisplaySetting()
        {
            BrFieldDisplaySettingsDimRegisteredDataSources = new HashSet<BrFieldDisplaySettingsDimRegisteredDataSource>();
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public int DimUserProfileId { get; set; }
        public int FieldIdentifier { get; set; }
        public bool Show { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimUserProfile DimUserProfile { get; set; }
        public virtual ICollection<BrFieldDisplaySettingsDimRegisteredDataSource> BrFieldDisplaySettingsDimRegisteredDataSources { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}

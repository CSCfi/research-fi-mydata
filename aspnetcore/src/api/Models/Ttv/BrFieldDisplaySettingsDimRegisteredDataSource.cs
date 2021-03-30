using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class BrFieldDisplaySettingsDimRegisteredDataSource
    {
        public int DimFieldDisplaySettingsId { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimFieldDisplaySetting DimFieldDisplaySettings { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
    }
}

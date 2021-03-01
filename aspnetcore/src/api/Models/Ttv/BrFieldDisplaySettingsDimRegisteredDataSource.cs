using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class BrFieldDisplaySettingsDimRegisteredDataSource
    {
        public int DimFieldDisplaySettingsId { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimFieldDisplaySettings DimFieldDisplaySettings { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
    }
}

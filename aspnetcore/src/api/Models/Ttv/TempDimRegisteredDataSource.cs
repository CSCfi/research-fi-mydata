using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class TempDimRegisteredDataSource
    {
        public int Id { get; set; }
        public int DimOrganizationId { get; set; }
        public string Name { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }
    }
}

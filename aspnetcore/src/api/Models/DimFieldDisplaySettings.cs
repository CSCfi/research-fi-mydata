using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimFieldDisplaySettings
    {
        public int Id { get; set; }
        public int DimUserProfileId { get; set; }
        public int FieldIdentifier { get; set; }
        public bool Show { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimUserProfile DimUserProfile { get; set; }
    }
}

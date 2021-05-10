﻿using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimUserProfile
    {
        public DimUserProfile()
        {
            DimFieldDisplaySettings = new HashSet<DimFieldDisplaySetting>();
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public int DimKnownPersonId { get; set; }
        public bool AllowAllSubscriptions { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual ICollection<DimFieldDisplaySetting> DimFieldDisplaySettings { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimFieldOfScience
    {
        public DimFieldOfScience()
        {
            FactFieldValues = new HashSet<FactFieldValue>();
            DimInfrastructures = new HashSet<DimInfrastructure>();
            DimKnownPeople = new HashSet<DimKnownPerson>();
            DimResearchActivities = new HashSet<DimResearchActivity>();
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

        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }

        public virtual ICollection<DimInfrastructure> DimInfrastructures { get; set; }
        public virtual ICollection<DimKnownPerson> DimKnownPeople { get; set; }
        public virtual ICollection<DimResearchActivity> DimResearchActivities { get; set; }
    }
}

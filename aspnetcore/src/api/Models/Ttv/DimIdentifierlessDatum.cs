﻿using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimIdentifierlessDatum
    {
        public DimIdentifierlessDatum()
        {
            FactContributions = new HashSet<FactContribution>();
            FactFieldValues = new HashSet<FactFieldValue>();
            InverseDimIdentifierlessData = new HashSet<DimIdentifierlessDatum>();
        }

        public int Id { get; set; }
        public int? DimIdentifierlessDataId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimIdentifierlessDatum DimIdentifierlessData { get; set; }
        public virtual ICollection<FactContribution> FactContributions { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
        public virtual ICollection<DimIdentifierlessDatum> InverseDimIdentifierlessData { get; set; }
    }
}
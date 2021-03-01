using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimDate
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}

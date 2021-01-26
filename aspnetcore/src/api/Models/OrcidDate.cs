using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class OrcidDate
    {
        public OrcidDate()
        {
            Year = null;
            Month = null;
            Day = null;
        }

        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
    }
}
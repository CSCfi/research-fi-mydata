namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Date that may be given with only year, or year and month (schema: date).
    /// </summary>
    public class PartialDate
    {
        /// <summary>Year.</summary>
        public int Year { get; set; }

        /// <summary>Month.</summary>
        public int? Month { get; set; }

        /// <summary>Day.</summary>
        public int? Day { get; set; }

        /// <summary>Partial date as text.</summary>
        public string DatePartial { get; set; }
    }
}

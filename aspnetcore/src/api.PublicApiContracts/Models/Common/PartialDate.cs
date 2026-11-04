using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Date given as a year, year and month, or full date (schema: date).
    /// </summary>
    public class PartialDate
    {
        /// <summary>Year.</summary>
        public int Year { get; set; }

        /// <summary>Month.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Month { get; set; }

        /// <summary>Day.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Day { get; set; }

        /// <summary>Date value formatted as YYYY, YYYY-MM, or YYYY-MM-DD, depending on available parts.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string DatePartial { get; set; }
    }
}

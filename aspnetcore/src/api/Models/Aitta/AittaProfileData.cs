using System.Collections.Generic;
using api.Models.Ttv;

namespace api.Models.Aitta
{
    public partial class AittaProfileData
    {
        public AittaProfileData()
        {
        }

        public List<string> Name { get; set; } = new List<string>();
        public List<string> ResearcherDescription { get; set; } = new List<string>();
        public List<Affiliation> Affiliations { get; set; } = new List<Affiliation>();
        public List<Education> Educations { get; set; } = new List<Education>();
        public List<Publication> Publications { get; set; } = new List<Publication>();
    }

    public class ProfileDataDate
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
    }

    public class Affiliation
    {
        public string Position { get; set; }
        public string Organization { get; set; }
        public ProfileDataDate? StartDate { get; set; }
        public ProfileDataDate? EndDate { get; set; }
    }

    public class Education
    {
        public string Name { get; set; }
    }

    public class Publication
    {
        public string Name { get; set; }
        public string? Abstract { get; set; }
        public int? Year { get; set; }
    }
}

using System;

namespace api.Models.Orcid
{
    public class OrcidName
    {
        public OrcidName(string givenNames, string familyName, DateTime? lastModifiedDate, DateTime? createdDate)
        {
            GivenNames = givenNames;
            FamilyName = familyName;
            LastModifiedDate = lastModifiedDate;
            CreatedDate = createdDate;
        }

        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

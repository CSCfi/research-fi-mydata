using static api.Models.Common.Constants;

namespace api.Models.Orcid
{
    public partial class OrcidResearchActivity {
        public OrcidResearchActivity(
            string dimReferencedataCodeValue,
            string workType,
            string name,
            string disambiguatedOrganizationIdentifier,
            string disambiguationSource,
            string organizationName,
            string departmentName,
            OrcidDate startDate,
            OrcidDate endDate,
            OrcidPutCode putCode,
            string url)
        {
            DimReferencedataCodeValue = dimReferencedataCodeValue;
            WorkType = workType;
            Name = name;
            DisambiguatedOrganizationIdentifier = disambiguatedOrganizationIdentifier;
            DisambiguationSource = disambiguationSource;
            OrganizationName = organizationName;
            DepartmentName = departmentName;
            StartDate = startDate;
            EndDate = endDate;
            PutCode = putCode;
            Url = url;
        }

        public string DimReferencedataCodeValue { get; set; }
        public string WorkType { get; set; }
        public string Name { get; set; }
        public string DisambiguatedOrganizationIdentifier { get; set; }
        public string DisambiguationSource { get; set; }
        public string OrganizationName { get; set; }
        public string DepartmentName { get; set; }
        public OrcidDate StartDate { get; set; }
        public OrcidDate EndDate { get; set; }
        public OrcidPutCode PutCode { get; set; }
        public string Url { get; set; }
    }
}
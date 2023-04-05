using static api.Models.Common.Constants;

namespace api.Models.Orcid
{
    public partial class OrcidResearchActivity {
        public OrcidResearchActivity(
            string organizationName,
            string disambiguatedOrganizationIdentifier,
            string disambiguationSource,
            string departmentName,
            string roleTitle,
            OrcidDate startDate,
            OrcidDate endDate,
            OrcidPutCode putCode,
            string url)
        {
            OrganizationName = organizationName;
            DisambiguatedOrganizationIdentifier = disambiguatedOrganizationIdentifier;
            DisambiguationSource = disambiguationSource;
            DepartmentName = departmentName;
            RoleTitle = roleTitle;
            StartDate = startDate;
            EndDate = endDate;
            PutCode = putCode;
            Url = url;
        }

        public string OrganizationName { get; set; }
        public string DisambiguatedOrganizationIdentifier { get; set; }
        public string DisambiguationSource { get; set; }
        public string DepartmentName { get; set; }
        public string RoleTitle { get; set; }
        public OrcidDate StartDate { get; set; }
        public OrcidDate EndDate { get; set; }
        public OrcidPutCode PutCode { get; set; }
        public string Url { get; set; }
    }
}
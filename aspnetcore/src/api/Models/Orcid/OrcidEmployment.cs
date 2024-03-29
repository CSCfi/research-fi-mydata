﻿namespace api.Models.Orcid
{
    public partial class OrcidEmployment {
        public OrcidEmployment(string organizationName, string disambiguatedOrganizationIdentifier, string disambiguationSource, string departmentName, string roleTitle, OrcidDate startDate, OrcidDate endDate, OrcidPutCode putCode)
        {
            OrganizationName = organizationName;
            DisambiguatedOrganizationIdentifier = disambiguatedOrganizationIdentifier;
            DisambiguationSource = disambiguationSource;
            DepartmentName = departmentName;
            RoleTitle = roleTitle;
            StartDate = startDate;
            EndDate = endDate;
            PutCode = putCode;
        }

        public string OrganizationName { get; set; }
        public string DisambiguatedOrganizationIdentifier { get; set; }
        public string DisambiguationSource { get; set; }
        public string DepartmentName { get; set; }
        public string RoleTitle { get; set; }
        public OrcidDate StartDate { get; set; }
        public OrcidDate EndDate { get; set; }
        public OrcidPutCode PutCode { get; set; }
    }
}
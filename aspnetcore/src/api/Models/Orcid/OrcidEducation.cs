namespace api.Models.Orcid
{
    public partial class OrcidEducation {
        public OrcidEducation(string organizationName, string departmentName, string roleTitle, OrcidDate startDate, OrcidDate endDate, OrcidPutCode putCode)
        {
            OrganizationName = organizationName;
            DepartmentName = departmentName;
            RoleTitle = roleTitle;
            StartDate = startDate;
            EndDate = endDate;
            PutCode = putCode;
        }

        public string OrganizationName { get; set; }
        public string DepartmentName { get; set; }
        public string RoleTitle { get; set; }
        public OrcidDate StartDate { get; set; }
        public OrcidDate EndDate { get; set; }
        public OrcidPutCode PutCode { get; set; }
    }
}
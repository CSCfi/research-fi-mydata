namespace api.Models.Elasticsearch
{
    public partial class ItemEducation
    {
        public ItemEducation()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            DegreeGrantingInstitutionName = "";
            StartDate = new ItemDate();
            EndDate = new ItemDate();
            PrimaryValue = null;
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string DegreeGrantingInstitutionName { get; set; }
        public ItemDate StartDate { get; set; }
        public ItemDate EndDate { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

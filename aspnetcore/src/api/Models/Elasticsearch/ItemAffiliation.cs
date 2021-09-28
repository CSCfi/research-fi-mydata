namespace api.Models.Elasticsearch
{
    public partial class ItemAffiliation
    {
        public ItemAffiliation()
        {
            OrganizationNameFi = "";
            OrganizationNameEn = "";
            OrganizationNameSv = "";
            PositionNameFi = "";
            PositionNameEn = "";
            PositionNameSv = "";
            Type = "";
            StartDate = new ItemDate();
            EndDate = new ItemDate();
            PrimaryValue = null;
        }

        public string OrganizationNameFi { get; set; }
        public string OrganizationNameEn { get; set; }
        public string OrganizationNameSv { get; set; }
        public string PositionNameFi { get; set; }
        public string PositionNameEn { get; set; }
        public string PositionNameSv { get; set; }
        public string Type { get; set; } 
        public ItemDate StartDate { get; set; }
        public ItemDate EndDate { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

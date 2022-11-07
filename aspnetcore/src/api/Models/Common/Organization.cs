namespace api.Models.Common
{
    public partial class Organization
    {
        public Organization()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
            SectorId = "";
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string SectorId { get; set; }
    }
}

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorCooperationItem
    {
        public ProfileEditorCooperationItem()
        {
        }

        public int Id { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public bool Selected { get; set; }
        public int? Order { get; set; }
    }
}

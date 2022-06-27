namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingPermissionItem
    {
        public ProfileEditorSharingPermissionItem()
        {
            PermissionId = -1;
            NameFi = "";
            NameEn = "";
            NameSv = "";
        }

        public int PermissionId { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
    }
}

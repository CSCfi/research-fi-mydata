namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorExternalIdentifier : ProfileEditorItem
    {
        public ProfileEditorExternalIdentifier()
        {
            PidContent = "";
            PidType = "";
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
    }
}

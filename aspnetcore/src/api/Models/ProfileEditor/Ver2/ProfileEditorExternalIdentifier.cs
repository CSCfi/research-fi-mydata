namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorExternalIdentifier : ProfileEditorItem2
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

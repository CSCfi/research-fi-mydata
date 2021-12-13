namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemExternalIdentifier : ProfileEditorItem
    {
        public ProfileEditorItemExternalIdentifier()
        {
            PidContent = "";
            PidType = "";
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
    }
}

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorPreferredIdentifier
    {
        public ProfileEditorPreferredIdentifier()
        {
            PidContent = "";
            PidType = "";
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
    }
}

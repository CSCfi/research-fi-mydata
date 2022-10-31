namespace api.Models.ProfileEditor.Items
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

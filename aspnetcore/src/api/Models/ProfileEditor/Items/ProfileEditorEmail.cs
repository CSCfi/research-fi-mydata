namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorEmail : ProfileEditorItem
    {
        public ProfileEditorEmail()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}

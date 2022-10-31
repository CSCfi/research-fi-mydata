namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorTelephoneNumber : ProfileEditorItem
    {
        public ProfileEditorTelephoneNumber()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}

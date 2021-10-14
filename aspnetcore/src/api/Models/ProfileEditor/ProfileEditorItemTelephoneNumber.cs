namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemTelephoneNumber : ProfileEditorItem
    {
        public ProfileEditorItemTelephoneNumber()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}

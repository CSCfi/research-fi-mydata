namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorTelephoneNumber : ProfileEditorItem2
    {
        public ProfileEditorTelephoneNumber()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}

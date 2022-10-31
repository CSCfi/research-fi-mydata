namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorKeyword : ProfileEditorItem
    {
        public ProfileEditorKeyword()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}

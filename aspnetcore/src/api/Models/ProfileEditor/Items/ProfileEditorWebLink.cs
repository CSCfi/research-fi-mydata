namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorWebLink : ProfileEditorItem
    {
        public ProfileEditorWebLink()
        {
            Url = "";
            LinkLabel = "";
        }

        public string Url { get; set; }
        public string LinkLabel { get; set; }
    }
}

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorWebLink : ProfileEditorItem2
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

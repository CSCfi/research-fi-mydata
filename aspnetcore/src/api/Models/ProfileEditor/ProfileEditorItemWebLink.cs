namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemWebLink : ProfileEditorItem
    {
        public ProfileEditorItemWebLink()
        {
            Url = "";
            LinkLabel = "";
        }

        public string Url { get; set; }
        public string LinkLabel { get; set; }
    }
}

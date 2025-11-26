namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorWebLink_WithoutItemMeta
    {
        public ProfileEditorWebLink_WithoutItemMeta()
        {
            Url = "";
            LinkLabel = "";
            LinkType = "";
        }

        public string Url { get; set; }
        public string LinkLabel { get; set; }
        public string LinkType { get; set; }
    }
}

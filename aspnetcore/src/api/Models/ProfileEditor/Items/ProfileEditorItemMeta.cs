namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorItemMeta
    {
        public ProfileEditorItemMeta()
        {
        }

        public int Id { get; set; }
        public int Type { get; set; }
        public bool? Show { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

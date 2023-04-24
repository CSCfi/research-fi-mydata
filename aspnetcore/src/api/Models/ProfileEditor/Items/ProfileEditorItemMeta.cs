namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorItemMeta
    {
        public ProfileEditorItemMeta(int id, int type, bool? show, bool? primaryValue)
        {
            Id = id;
            Type = type;
            Show = show;
            PrimaryValue = primaryValue;
            TemporaryUniqueId = ulong.Parse(Type.ToString() + Id.ToString());
        }

        public int Id { get; set; }
        public int Type { get; set; }
        public bool? Show { get; set; }
        public bool? PrimaryValue { get; set; }
        public ulong TemporaryUniqueId { get; set; }
    }
}

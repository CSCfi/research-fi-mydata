namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorSharingItemMeta
    {
        public ProfileEditorSharingItemMeta()
        {
            PurposeId = -1;
            PermittedFieldGroupId = -1;
        }

        public ProfileEditorSharingItemMeta(int purposeId, int permittedFieldGroupId)
        {
            PurposeId = purposeId;
            PermittedFieldGroupId = permittedFieldGroupId;
        }

        public int PurposeId { get; set; }
        public int PermittedFieldGroupId { get; set; }
    }
}

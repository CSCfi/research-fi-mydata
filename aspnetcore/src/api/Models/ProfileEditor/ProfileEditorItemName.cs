namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorItemName : ProfileEditorItem
    {
        public ProfileEditorItemName()
        {
            FirstNames = "";
            LastName = "";
            FullName = "";
        }

        public string FirstNames { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
    }
}

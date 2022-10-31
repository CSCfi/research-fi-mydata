using api.Models.Common;
namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorSource
    {
        public ProfileEditorSource()
        {
        }

        public int Id { get; set; }
        public string RegisteredDataSource { get; set; }
        public Organization Organization { get; set; }
    }
}

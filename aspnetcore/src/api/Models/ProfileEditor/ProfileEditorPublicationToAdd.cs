
namespace api.Models
{
    public partial class ProfileEditorPublicationToAdd
    {
        public ProfileEditorPublicationToAdd()
        {
        }

        public string PublicationId { get; set; }
        public bool? Show { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

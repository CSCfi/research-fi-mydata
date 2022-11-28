using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorPublication : ProfileEditorItem
    {
        public ProfileEditorPublication()
        {
            PublicationId = "";
            PublicationName = "";
            PublicationYear = null;
            Doi = "";
            AuthorsText = "";
            TypeCode = "";
            JournalName = "";
            ConferenceName = "";
            ParentPublicationName = "";
        }

        public string PublicationId { get; set; }
        public string PublicationName { get; set; }
        public int? PublicationYear { get; set; }
        public string Doi { get; set; }
        public string AuthorsText { get; set; }
        public string TypeCode { get; set; }
        public string JournalName { get; set; }
        public string ConferenceName { get; set; }
        public string ParentPublicationName { get; set; }
    }
}

using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorPublication : ProfileEditorItem2
    {
        public ProfileEditorPublication()
        {
            PublicationId = "";
            PublicationName = "";
            PublicationYear = null;
            Doi = "";
            TypeCode = "";
        }

        public string PublicationId { get; set; }
        public string PublicationName { get; set; }
        public int? PublicationYear { get; set; }
        public string Doi { get; set; }
        public string TypeCode { get; set; }
    }
}

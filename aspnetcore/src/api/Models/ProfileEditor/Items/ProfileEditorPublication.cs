using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorPublication : ProfileEditorItem
    {        public ProfileEditorPublication()
        {
            ArticleNumberText = "";
            AuthorsText = "";
            Doi = "";
            IssueNumber = "";
            ConferenceName = "";
            JournalName = "";
            OpenAccess = 0;
            PageNumberText = "";
            ParentPublicationName = "";
            ParentPublicationPublisher = "";
            PublicationId = "";
            PublicationName = "";
            PublicationTypeCode = "";
            PublicationYear = null;
            PublisherName = "";
            SelfArchivedAddress = "";
            SelfArchivedCode = "";
            Volume = "";
        }
        public string ArticleNumberText { get; set; }
        public string AuthorsText { get; set; }
        public string ConferenceName { get; set; }
        public string Doi { get; set; }
        public string IssueNumber { get; set; }
        public string JournalName { get; set; }
        public int OpenAccess { get; set; }
        public string PageNumberText { get; set; }
        public string ParentPublicationName { get; set; }
        public string ParentPublicationPublisher { get; set; }
        public List<ProfileEditorPublicationPeerReviewed> PeerReviewed { get; set; } = new List<ProfileEditorPublicationPeerReviewed>();
        public string PublicationId { get; set; }
        public string PublicationName { get; set; }
        public string PublicationTypeCode { get; set; }
        public int? PublicationYear { get; set; }
        public string PublisherName { get; set; }
        public string SelfArchivedAddress { get; set; }
        public string SelfArchivedCode { get; set; }
        public string Volume { get; set; }
    }
}
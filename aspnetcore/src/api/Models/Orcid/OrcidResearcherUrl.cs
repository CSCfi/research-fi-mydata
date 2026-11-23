using System;

namespace api.Models.Orcid
{
    public partial class OrcidResearcherUrl {
        public OrcidResearcherUrl(string urlName, string url, OrcidPutCode putCode, DateTime? lastModifiedDate = null, DateTime? createdDate = null)
        {
            UrlName = urlName;
            Url = url;
            PutCode = putCode;
            LastModifiedDate = lastModifiedDate;
            CreatedDate = createdDate;
        }

        public string UrlName { get; set; }
        public string Url { get; set; }
        public OrcidPutCode PutCode { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
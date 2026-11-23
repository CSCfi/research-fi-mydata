using System;

namespace api.Models.Orcid
{
    public partial class OrcidExternalIdentifier {
        public OrcidExternalIdentifier(string externalIdType, string externalIdValue, string externalIdUrl, OrcidPutCode putCode, DateTime? lastModifiedDate = null, DateTime? createdDate = null)
        {
            ExternalIdType = externalIdType;
            ExternalIdValue = externalIdValue;
            ExternalIdUrl = externalIdUrl;
            PutCode = putCode;
            LastModifiedDate = lastModifiedDate;
            CreatedDate = createdDate;
        }

        public string ExternalIdType { get; set; }
        public string ExternalIdValue { get; set; }
        public string ExternalIdUrl { get; set; }
        public OrcidPutCode PutCode { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
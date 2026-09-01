using System;

namespace api.Models.Orcid
{
    public partial class OrcidKeyword {
        public OrcidKeyword(string value, OrcidPutCode putCode, DateTime? lastModifiedDate = null, DateTime? createdDate = null)
        {
            Value = value;
            PutCode = putCode;
            LastModifiedDate = lastModifiedDate;
            CreatedDate = createdDate;
        }

        public string Value { get; set; }
        public OrcidPutCode PutCode { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
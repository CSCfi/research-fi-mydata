using System;

namespace api.Models.Orcid
{
    public partial class OrcidCreditName {
        public OrcidCreditName(string value, DateTime? lastModifiedDate = null, DateTime? createdDate = null)
        {
            Value = value;
            LastModifiedDate = lastModifiedDate;
            CreatedDate = createdDate;
        }

        public string Value { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
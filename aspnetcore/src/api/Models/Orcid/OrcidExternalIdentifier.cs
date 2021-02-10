namespace api.Models.Orcid
{
    public partial class OrcidExternalIdentifier {
        public OrcidExternalIdentifier(string externalIdType, string externalIdValue, string externalIdUrl, OrcidPutCode putCode)
        {
            ExternalIdType = externalIdType;
            ExternalIdValue = externalIdValue;
            ExternalIdUrl = externalIdUrl;
            PutCode = putCode;
        }

        public string ExternalIdType { get; set; }
        public string ExternalIdValue { get; set; }
        public string ExternalIdUrl { get; set; }
        public OrcidPutCode PutCode { get; set; }
    }
}
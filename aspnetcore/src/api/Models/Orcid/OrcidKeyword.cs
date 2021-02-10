namespace api.Models.Orcid
{
    public partial class OrcidKeyword {
        public OrcidKeyword(string value, OrcidPutCode putCode)
        {
            Value = value;
            PutCode = putCode;
        }

        public string Value { get; set; }
        public OrcidPutCode PutCode { get; set; }
    }
}
namespace api.Models.Orcid
{
    public partial class OrcidEmail {
        public OrcidEmail(string value, OrcidPutCode putCode)
        {
            Value = value;
            PutCode = putCode;
        }

        public string Value { get; set; }
        public OrcidPutCode PutCode { get; set; }
    }
}
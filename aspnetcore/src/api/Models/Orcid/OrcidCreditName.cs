namespace api.Models.Orcid
{
    public partial class OrcidCreditName {
        public OrcidCreditName(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
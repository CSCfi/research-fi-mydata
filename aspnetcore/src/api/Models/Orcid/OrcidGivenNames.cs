namespace api.Models.Orcid
{
    public partial class OrcidGivenNames {
        public OrcidGivenNames(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
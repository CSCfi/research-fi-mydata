namespace api.Models.Orcid
{
    public partial class OrcidFamilyName {
        public OrcidFamilyName(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
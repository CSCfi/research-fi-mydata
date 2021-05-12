namespace api.Models.Orcid
{
    public partial class OrcidBiography {
        public OrcidBiography(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
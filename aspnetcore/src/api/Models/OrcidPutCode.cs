namespace api.Models.Orcid
{
    public partial class OrcidPutCode
    {
        public OrcidPutCode(uint? value)
        {
            Value = value;
        }

        public uint? Value { get; set; }
    }
}
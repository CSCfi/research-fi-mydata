namespace api.Models.Orcid
{
    public partial class OrcidPutCode
    {
        public OrcidPutCode(int? value)
        {
            Value = value;
        }

        public string GetDbValue()
        {
            if (this.Value == null)
            {
                return "-1";
            }
            else
            {
                return this.Value.ToString();
            }
        }

        public int? Value { get; set; }
    }
}
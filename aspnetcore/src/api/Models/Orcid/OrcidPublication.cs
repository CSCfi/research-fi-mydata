namespace api.Models.Orcid
{
    public partial class OrcidPublication {
        public OrcidPublication()
        {
            PublicationName = "";
            PublicationYear = null;
            DoiHandle = "";
            Type = "";
            PutCode = new OrcidPutCode(0) { };
        }

        public string PublicationName { get; set; }
        public int? PublicationYear { get; set; }
        public string DoiHandle { get; set; }
        public string Type { get; set; }
        public OrcidPutCode PutCode { get; set; }
    }
}
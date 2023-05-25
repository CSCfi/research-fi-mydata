namespace api.Models.Orcid
{
    public partial class OrcidDataset {
        public OrcidDataset()
        {
            DatasetName = "";
            DatasetDate = null;
            Type = "";
            PutCode = new OrcidPutCode(0) { };
            Url = "";
        }

        public string DatasetName { get; set; }
        public OrcidDate DatasetDate { get; set; }
        public string Type { get; set; }
        public OrcidPutCode PutCode { get; set; }
        public string Url { get; set; }
    }
}
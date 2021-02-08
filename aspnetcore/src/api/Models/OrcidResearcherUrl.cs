namespace api.Models.Orcid
{
    public partial class OrcidResearcherUrl {
        public OrcidResearcherUrl(string urlName, string url, OrcidPutCode putCode)
        {
            UrlName = urlName;
            Url = url;
            PutCode = putCode;
        }

        public string UrlName { get; set; }
        public string Url { get; set; }
        public OrcidPutCode PutCode { get; set; }
    }
}
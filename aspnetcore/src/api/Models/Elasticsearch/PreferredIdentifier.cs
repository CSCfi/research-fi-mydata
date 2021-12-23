namespace api.Models.Elasticsearch
{
    public partial class PreferredIdentifier
    {
        public PreferredIdentifier()
        {
            PidContent = "";
            PidType = "";
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
    }
}

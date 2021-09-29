namespace api.Models.Elasticsearch
{
    public partial class ItemExternalIdentifier
    {
        public ItemExternalIdentifier()
        {
            PidContent = "";
            PidType = "";
            PrimaryValue = null;
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

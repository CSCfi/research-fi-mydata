namespace api.Models.Elasticsearch
{
    public partial class ItemWebLink
    {
        public ItemWebLink()
        {
            Url = "";
            LinkLabel = "";
            PrimaryValue = null;
        }

        public string Url { get; set; }
        public string LinkLabel { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

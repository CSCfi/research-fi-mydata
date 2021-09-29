namespace api.Models.Elasticsearch
{
    public partial class ItemKeyword
    {
        public ItemKeyword()
        {
            Value = "";
            PrimaryValue = null;
        }

        public string Value { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

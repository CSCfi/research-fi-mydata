namespace api.Models.Elasticsearch
{
    public partial class ItemEmail
    {
        public ItemEmail()
        {
            Value = "";
            PrimaryValue = false;
        }

        public string Value { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

namespace api.Models.Elasticsearch
{
    public partial class ItemTelephoneNumber
    {
        public ItemTelephoneNumber()
        {
            Value = "";
            PrimaryValue = null;
        }

        public string Value { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

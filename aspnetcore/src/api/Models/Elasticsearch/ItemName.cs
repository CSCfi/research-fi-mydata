namespace api.Models.Elasticsearch
{
    public partial class ItemName
    {
        public ItemName()
        {
            FirstNames = "";
            LastName = "";
            FullName = "";
            PrimaryValue = false;
        }

        public string FirstNames { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

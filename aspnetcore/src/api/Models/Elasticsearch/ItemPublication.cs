namespace api.Models.Elasticsearch
{
    public partial class ItemPublication
    {
        public ItemPublication()
        {
            PublicationId = "";
            PublicationName = "";
            PublicationYear = null;
            Doi = "";
            TypeCode = "";
            PrimaryValue = null;
        }

        public string PublicationId { get; set; }
        public string PublicationName { get; set; }
        public int? PublicationYear { get; set; }
        public string Doi { get; set; }
        public string TypeCode { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

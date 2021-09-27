namespace api.Models.Elasticsearch
{
    public partial class Person
    {
        public Person(string orcidId)
        {
            id = orcidId;
            personal = new Personal();
        }

        public string id { get; set; } 
        public Personal personal { get; set; }
    }
}

namespace api.Models.Elasticsearch
{
    public partial class Person
    {
        public Person(string orcidId)
        {
            id = orcidId;
            personal = new Personal();
            activity = new Activity();
        }

        public string id { get; set; } 
        public Personal personal { get; set; }
        public Activity activity { get; set; }
    }
}

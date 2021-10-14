using api.Models.ProfileEditor;
namespace api.Models.Elasticsearch
{
    public class ElasticPerson
    {

        public string Id { get; set; }
        public ProfileEditorDataResponse Profile { get; set; }
    }
}

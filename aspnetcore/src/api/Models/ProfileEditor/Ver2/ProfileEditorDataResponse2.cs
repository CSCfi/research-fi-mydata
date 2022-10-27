using api.Models.Elasticsearch;
using System.Collections.Generic;

namespace api.Models.ProfileEditor
{
    public partial class ProfileEditorDataResponse2    {
        public ProfileEditorDataResponse2()
        {
            personal = new();
            activity = new();
            uniqueDataSources = new();
        }

        public ProfileEditorDataPersonal2 personal { get; set; }
        public ProfileEditorDataActivity2 activity { get; set; }
        public List<ProfileEditorSource> uniqueDataSources { get; set; }
    }
}

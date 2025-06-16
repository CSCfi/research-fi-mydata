using api.Models.Elasticsearch;
using System.Collections.Generic;

namespace api.Models.ProfileEditor.Items
{
    public partial class ProfileEditorDataResponse {
        public ProfileEditorDataResponse()
        {
            personal = new();
            activity = new();
            settings = new();
            uniqueDataSources = new();
        }

        public ProfileEditorDataPersonal personal { get; set; }
        public ProfileEditorDataActivity activity { get; set; }
        public ProfileSettings settings { get; set; }
        public List<ProfileEditorCooperationItem> cooperation { get; set; }
        public List<ProfileEditorSource> uniqueDataSources { get; set; }
    }
}

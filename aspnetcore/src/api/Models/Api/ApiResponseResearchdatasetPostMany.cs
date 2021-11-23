using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseResearchdatasetPostMany : ApiResponse
    {

        public ApiResponseResearchdatasetPostMany(bool success, string reason, ProfileEditorAddResearchdatasetResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorAddResearchdatasetResponse Data { get; set; }
    }
}

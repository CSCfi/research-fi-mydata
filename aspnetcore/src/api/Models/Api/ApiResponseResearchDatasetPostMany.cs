using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseResearchDatasetPostMany : ApiResponse
    {

        public ApiResponseResearchDatasetPostMany(bool success, string reason, ProfileEditorAddResearchDatasetResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorAddResearchDatasetResponse Data { get; set; }
    }
}

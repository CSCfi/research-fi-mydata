using api.Models.ProfileEditor;

namespace api.Models.Api
{
    public class ApiResponseResearchDatasetRemoveMany : ApiResponse
    {

        public ApiResponseResearchDatasetRemoveMany(bool success, string reason, ProfileEditorRemoveResearchDatasetResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorRemoveResearchDatasetResponse Data { get; set; }
    }
}

using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseCooperationDemoPatch : ApiResponse
    {

        public ApiResponseCooperationDemoPatch(bool success, string reason, ProfileEditorCooperationModificationResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorCooperationModificationResponse Data { get; set; }
    }
}

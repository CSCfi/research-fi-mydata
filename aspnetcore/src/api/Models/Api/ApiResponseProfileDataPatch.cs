using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseProfileDataPatch : ApiResponse
    {

        public ApiResponseProfileDataPatch(bool success, string reason, ProfileEditorDataModificationResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorDataModificationResponse Data { get; set; }
    }
}

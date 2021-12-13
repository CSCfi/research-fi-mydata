using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseProfileDataGet : ApiResponse
    {

        public ApiResponseProfileDataGet(bool success, string reason, ProfileEditorDataResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorDataResponse Data { get; set; }
    }
}

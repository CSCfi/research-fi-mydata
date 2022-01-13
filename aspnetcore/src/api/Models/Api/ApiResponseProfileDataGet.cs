using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseProfileDataGet : ApiResponse
    {

        public ApiResponseProfileDataGet(bool success, string reason, ProfileEditorDataResponse data, bool fromCache)
        {
            Success = success;
            Reason = reason;
            Data = data;
            FromCache = fromCache;
        }

        public ProfileEditorDataResponse Data { get; set; }
    }
}

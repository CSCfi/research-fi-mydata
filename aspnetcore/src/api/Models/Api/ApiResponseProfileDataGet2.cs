using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseProfileDataGet2 : ApiResponse
    {

        public ApiResponseProfileDataGet2(bool success, string reason, ProfileEditorDataResponse2 data, bool fromCache)
        {
            Success = success;
            Reason = reason;
            Data = data;
            FromCache = fromCache;
        }

        public ProfileEditorDataResponse2 Data { get; set; }
    }
}

using api.Models.ProfileEditor;

namespace api.Models.Api
{
    public class ApiResponseProfileSettingsGet : ApiResponse
    {

        public ApiResponseProfileSettingsGet(bool success, string reason, ProfileSettings data, bool fromCache)
        {
            Success = success;
            Reason = reason;
            Data = data;
            FromCache = fromCache;
        }

        public ProfileSettings Data { get; set; }
    }
}
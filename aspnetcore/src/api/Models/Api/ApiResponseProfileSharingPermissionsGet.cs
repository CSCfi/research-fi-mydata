using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseProfileSharingPermissionsGet : ApiResponse
    {

        public ApiResponseProfileSharingPermissionsGet(bool success, string reason, ProfileEditorSharingPermissionsResponse data, bool fromCache)
        {
            Success = success;
            Reason = reason;
            Data = data;
            FromCache = fromCache;
        }

        public ProfileEditorSharingPermissionsResponse Data { get; set; }
    }
}

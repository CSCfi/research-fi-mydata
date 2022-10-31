using api.Models.ProfileEditor;

namespace api.Models.Api
{
    public class ApiResponseProfileSharingGivenPermissionsGet : ApiResponse
    {

        public ApiResponseProfileSharingGivenPermissionsGet(bool success, string reason, ProfileEditorSharingGivenPermissionsResponse data, bool fromCache)
        {
            Success = success;
            Reason = reason;
            Data = data;
            FromCache = fromCache;
        }

        public ProfileEditorSharingGivenPermissionsResponse Data { get; set; }
    }
}

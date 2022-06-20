using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseProfileSharingPurposesGet : ApiResponse
    {

        public ApiResponseProfileSharingPurposesGet(bool success, string reason, ProfileEditorSharingPurposesResponse data, bool fromCache)
        {
            Success = success;
            Reason = reason;
            Data = data;
            FromCache = fromCache;
        }

        public ProfileEditorSharingPurposesResponse Data { get; set; }
    }
}

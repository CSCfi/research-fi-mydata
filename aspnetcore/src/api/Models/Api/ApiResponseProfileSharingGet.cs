using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseProfileSharingGet : ApiResponse
    {

        public ApiResponseProfileSharingGet(bool success, string reason, ProfileEditorSharingResponse data, bool fromCache)
        {
            Success = success;
            Reason = reason;
            Data = data;
            FromCache = fromCache;
        }

        public ProfileEditorSharingResponse Data { get; set; }
    }
}

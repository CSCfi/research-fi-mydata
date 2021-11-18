using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponsePublicationPostMany : ApiResponse
    {

        public ApiResponsePublicationPostMany(bool success, string reason, ProfileEditorAddPublicationResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorAddPublicationResponse Data { get; set; }
    }
}

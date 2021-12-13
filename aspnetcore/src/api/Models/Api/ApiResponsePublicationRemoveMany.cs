using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponsePublicationRemoveMany : ApiResponse
    {

        public ApiResponsePublicationRemoveMany(bool success, string reason, ProfileEditorRemovePublicationResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorRemovePublicationResponse Data { get; set; }
    }
}

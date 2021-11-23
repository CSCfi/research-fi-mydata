using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseFundingPostMany : ApiResponse
    {

        public ApiResponseFundingPostMany(bool success, string reason, ProfileEditorAddFundingResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorAddFundingResponse Data { get; set; }
    }
}

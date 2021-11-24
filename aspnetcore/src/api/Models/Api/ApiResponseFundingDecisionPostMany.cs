using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseFundingDecisionPostMany : ApiResponse
    {

        public ApiResponseFundingDecisionPostMany(bool success, string reason, ProfileEditorAddFundingDecisionResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorAddFundingDecisionResponse Data { get; set; }
    }
}

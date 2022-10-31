using api.Models.ProfileEditor.Items;

namespace api.Models.Api
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

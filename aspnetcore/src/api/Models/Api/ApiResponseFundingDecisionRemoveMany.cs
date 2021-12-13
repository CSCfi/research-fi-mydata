using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseFundingDecisionRemoveMany : ApiResponse
    {

        public ApiResponseFundingDecisionRemoveMany(bool success, string reason, ProfileEditorRemoveFundingDecisionResponse data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ProfileEditorRemoveFundingDecisionResponse Data { get; set; }
    }
}

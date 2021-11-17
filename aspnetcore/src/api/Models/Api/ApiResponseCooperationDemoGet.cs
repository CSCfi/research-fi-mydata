using System.Collections.Generic;
using api.Models.ProfileEditor;

namespace api.Models
{
    public class ApiResponseCooperationDemoGet : ApiResponse
    {

        public ApiResponseCooperationDemoGet(bool success, string reason, List<ProfileEditorCooperationItem> data, bool fromCache)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public List<ProfileEditorCooperationItem> Data { get; set; }
    }
}

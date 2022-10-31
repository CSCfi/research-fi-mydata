using System.Collections.Generic;
using api.Models.ProfileEditor;

namespace api.Models.Api
{
    public class ApiResponseCooperationGet : ApiResponse
    {

        public ApiResponseCooperationGet(bool success, string reason, List<ProfileEditorCooperationItem> data, bool fromCache)
        {
            Success = success;
            Reason = reason;
            Data = data;
            FromCache = fromCache;
        }

        public List<ProfileEditorCooperationItem> Data { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace api.Models
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            Success = true;
            Reason = "";
            FromCache = false;
        }

        public ApiResponse(bool success)
        {
            Success = success;
            Reason = "";
            FromCache = false;
        }

        public ApiResponse(bool success, string reason)
        {
            Success = success;
            Reason = reason;
            FromCache = false;
        }

        public ApiResponse(bool success, string reason, bool fromCache)
        {
            Success = success;
            Reason = reason;
            FromCache = fromCache;
        }

        public bool Success { get; set; }
        public string Reason { get; set; }
        public bool FromCache { get; set; }
    }
}

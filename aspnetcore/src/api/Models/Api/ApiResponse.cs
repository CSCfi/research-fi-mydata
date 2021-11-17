using System;
using System.Collections.Generic;

namespace api.Models
{
    public class ApiResponse
    {
        public ApiResponse()
        {
        }

        public ApiResponse(bool success)
        {
            Success = success;
        }

        public ApiResponse(bool success, string reason)
        {
            Success = success;
            Reason = reason;
        }

        public bool Success { get; set; }
        public string Reason { get; set; }
        public bool FromCache { get; set; }
    }
}

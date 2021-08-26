using System;
using System.Collections.Generic;

namespace api.Models
{
    public class ApiResponse
    {
        public ApiResponse(bool success)
        {
            Success = success;
            Reason = null;
            Data = null;
            FromCache = false;
        }

        public ApiResponse(object data)
        {
            Success = true;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ApiResponse(bool success, string reason)
        {
            Success = success;
            Reason = reason;
            Data = null;
            FromCache = false;
        }

        public ApiResponse(bool success, object data)
        {
            Success = success;
            Reason = null;
            Data = data;
            FromCache = false;
        }

        public ApiResponse(bool success, object data, bool fromCache)
        {
            Success = success;
            Reason = null;
            Data = data;
            FromCache = fromCache;
        }

        public ApiResponse(bool success, string reason, object data)
        {
            Success = success;
            Reason = reason;
            Data = data;
            FromCache = false;
        }

        public bool Success { get; set; }
        public string Reason { get; set; }
        public object Data { get; set; }
        public bool FromCache { get; set; }
    }
}

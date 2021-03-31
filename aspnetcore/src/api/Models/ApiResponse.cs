﻿using System;
using System.Collections.Generic;

namespace api.Models
{
    public class ApiResponse
    {
        public ApiResponse(bool success)
        {
            Success = success;
        }

        public ApiResponse(object data)
        {
            Success = true;
            Reason = null;
            Data = data;
        }

        public ApiResponse(bool success, string reason)
        {
            Success = success;
            Reason = reason;
        }

        public ApiResponse(bool success, string reason, object data)
        {
            Success = success;
            Reason = reason;
            Data = data;
        }

        public bool Success { get; set; }
        public string Reason { get; set; }
        public object Data { get; set; }
    }
}

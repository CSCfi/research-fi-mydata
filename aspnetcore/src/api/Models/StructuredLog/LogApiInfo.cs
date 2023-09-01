namespace api.Models.Log
{
    public class LogApiInfo
    {
        public LogApiInfo(string action, string state, bool error, string message)
        {
            Action = action;
            State = state;
            Error = error;
            Message = message;
        }

        public LogApiInfo(string action, string state, bool error)
        {
            Action = action;
            State = state;
            Error = error;
            Message = "";
        }

        public LogApiInfo(string action, string state, string message)
        {
            Action = action;
            State = state;
            Error = false;
            Message = message;
        }

        public LogApiInfo(string action, string state)
        {
            Action = action;
            State = state;
            Error = false;
            Message = "";
        }

        public string Action { get; set; }
        public string State { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}
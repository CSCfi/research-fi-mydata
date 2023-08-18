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
            Wood = new LogWood();
        }

        public LogApiInfo(string action, string state, bool error)
        {
            Action = action;
            State = state;
            Error = error;
            Message = "";
            Wood = new LogWood();
        }

        public LogApiInfo(string action, string state, string message)
        {
            Action = action;
            State = state;
            Error = false;
            Message = message;
            Wood = new LogWood();
        }

        public LogApiInfo(string action, string state)
        {
            Action = action;
            State = state;
            Error = false;
            Message = "";
            Wood = new LogWood();
        }

        public string Action { get; set; }
        public string State { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
        public LogWood Wood { get; set; }
    }
}
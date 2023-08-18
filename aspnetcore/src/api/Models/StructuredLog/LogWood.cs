namespace api.Models.Log
{
    public class WoodIndex
    {
        public WoodIndex()
        {
            project_number = 2000950;
            retention_months = 24;
            use_case = "research.fi 57f6";
        }

        public int project_number { get; set; }
        public int retention_months { get; set; }
        public string use_case { get; set; }
    }

    public class LogWood
    {
        public LogWood()
        {
            index = new WoodIndex();
        }

        public WoodIndex index { get; set; }
    }
}
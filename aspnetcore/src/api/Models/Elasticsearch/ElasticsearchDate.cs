﻿namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchDate
    {
        public ElasticsearchDate()
        {
            Year = 0;
            Month = 0;
            Day = 0;
        }

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}

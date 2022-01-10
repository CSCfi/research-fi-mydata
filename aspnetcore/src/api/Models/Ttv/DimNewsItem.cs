using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimNewsItem
    {
        public int Id { get; set; }
        public int DimNewsFeedId { get; set; }
        public string NewsHeadline { get; set; }
        public string NewsContent { get; set; }
        public string Url { get; set; }
        public DateTime? Timestamp { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimNewsFeed DimNewsFeed { get; set; }
    }
}

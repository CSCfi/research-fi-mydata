﻿using System.Collections.Generic;

namespace api.Models.Elasticsearch
{
    public partial class ElasticsearchPublication : ElasticsearchItem
    {
        public ElasticsearchPublication()
        {
            PublicationId = "";
            PublicationName = "";
            PublicationYear = null;
            Doi = "";
            AuthorsText = "";
            TypeCode = "";
        }

        public string PublicationId { get; set; }
        public string PublicationName { get; set; }
        public int? PublicationYear { get; set; }
        public string Doi { get; set; }
        public string AuthorsText { get; set; }
        public string TypeCode { get; set; }
    }
}

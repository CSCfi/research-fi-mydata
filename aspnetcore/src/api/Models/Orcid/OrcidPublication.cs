﻿namespace api.Models.Orcid
{
    public partial class OrcidPublication {
        public OrcidPublication()
        {
            PublicatonName = "";
            PublicationYear = null;
            DoiHandle = "";
            PutCode = new OrcidPutCode(0) { };
        }

        public string PublicatonName { get; set; }
        public int? PublicationYear { get; set; }
        public string DoiHandle { get; set; }
        public OrcidPutCode PutCode { get; set; }
    }
}
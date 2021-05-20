using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemPublication : ProfileEditorItem
    {
        public ProfileEditorItemPublication()
        {
            PublicatonName = "";
            PublicationYear = null;
            DoiHandle = "";
        }

        public string PublicatonName { get; set; }
        public int? PublicationYear { get; set; }
        public string DoiHandle { get; set; }
    }
}

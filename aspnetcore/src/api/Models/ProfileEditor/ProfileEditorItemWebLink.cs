using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemWebLink : ProfileEditorItem
    {
        public ProfileEditorItemWebLink()
        {
            Url = "";
            LinkLabel = "";
        }

        public string Url { get; set; }
        public string LinkLabel { get; set; }
    }
}

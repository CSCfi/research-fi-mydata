using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorWebLink
    {
        public ProfileEditorWebLink()
        {
        }

        public string Url { get; set; }
        public string UrlLabel { get; set; }
    }
}

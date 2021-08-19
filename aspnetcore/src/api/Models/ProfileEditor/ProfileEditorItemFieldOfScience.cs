using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemFieldOfScience : ProfileEditorItem
    {
        public ProfileEditorItemFieldOfScience()
        {
            NameFi = "";
            NameEn = "";
            NameSv = "";
        }

        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
    }
}

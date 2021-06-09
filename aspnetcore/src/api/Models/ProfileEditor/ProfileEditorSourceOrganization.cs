using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorSourceOrganization
    {
        public ProfileEditorSourceOrganization()
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

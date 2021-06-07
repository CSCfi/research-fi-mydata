using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemExternalIdentifier : ProfileEditorItem
    {
        public ProfileEditorItemExternalIdentifier()
        {
            PidContent = "";
            PidType = "";
        }

        public string PidContent { get; set; }
        public string PidType { get; set; }
    }
}

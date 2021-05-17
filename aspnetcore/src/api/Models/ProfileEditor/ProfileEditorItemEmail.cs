using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemEmail : ProfileEditorItem
    {
        public ProfileEditorItemEmail()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}

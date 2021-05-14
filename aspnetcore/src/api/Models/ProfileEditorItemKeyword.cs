using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemKeyword : ProfileEditorItem
    {
        public ProfileEditorItemKeyword()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}

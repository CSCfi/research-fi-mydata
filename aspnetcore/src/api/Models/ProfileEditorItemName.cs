using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemName : ProfileEditorItem
    {
        public ProfileEditorItemName()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}

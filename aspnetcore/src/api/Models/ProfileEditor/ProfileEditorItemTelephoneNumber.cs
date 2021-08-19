using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemTelephoneNumber : ProfileEditorItem
    {
        public ProfileEditorItemTelephoneNumber()
        {
            Value = "";
        }

        public string Value { get; set; }
    }
}

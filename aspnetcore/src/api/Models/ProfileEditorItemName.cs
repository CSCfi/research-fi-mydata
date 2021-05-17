using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorItemName : ProfileEditorItem
    {
        public ProfileEditorItemName()
        {
            FirstNames = "";
            LastName = "";
            FullName = "";
        }

        public string FirstNames { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupOtherName : ProfileEditorGroup
    {
        public ProfileEditorGroupOtherName()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemName> items { get; set; }
    }
}

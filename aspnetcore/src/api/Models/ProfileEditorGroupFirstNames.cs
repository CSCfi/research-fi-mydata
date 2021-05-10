using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupFirstNames : ProfileEditorGroup
    {
        public ProfileEditorGroupFirstNames()
        {
        }

        public ProfileEditorDataSource dataSource { get; set; }
        public List<ProfileEditorItemName> items { get; set; }
    }
}

using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupOtherNames : ProfileEditorGroup
    {
        public ProfileEditorGroupOtherNames()
        {
        }

        public ProfileEditorDataSource dataSource { get; set; }
        public List<ProfileEditorItemName> items { get; set; }
    }
}

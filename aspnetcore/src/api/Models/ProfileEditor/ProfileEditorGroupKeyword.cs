using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupKeyword : ProfileEditorGroup
    {
        public ProfileEditorGroupKeyword()
        {
        }

        public ProfileEditorDataSource dataSource { get; set; }
        public List<ProfileEditorItemKeyword> items { get; set; }
    }
}

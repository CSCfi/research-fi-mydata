using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupPublication : ProfileEditorGroup
    {
        public ProfileEditorGroupPublication()
        {
        }

        public ProfileEditorDataSource dataSource { get; set; }
        public List<ProfileEditorItemPublication> items { get; set; }
    }
}

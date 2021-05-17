using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupEmail : ProfileEditorGroup
    {
        public ProfileEditorGroupEmail()
        {
        }

        public ProfileEditorDataSource dataSource { get; set; }
        public List<ProfileEditorItemEmail> items { get; set; }
    }
}

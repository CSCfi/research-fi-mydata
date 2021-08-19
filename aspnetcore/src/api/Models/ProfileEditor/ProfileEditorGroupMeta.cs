using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class ProfileEditorGroupMeta
    {
        public ProfileEditorGroupMeta()
        {
        }

        public int Id { get; set; }
        public int Type { get; set; }
        public bool Show { get; set; }
    }
}

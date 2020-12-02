using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class ProfileEditorModificationItem
    {
        public ProfileEditorModificationItem()
        {
        }

        public int Id { get; set; }
        public bool Show { get; set; }
    }
}

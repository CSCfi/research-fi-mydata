using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class ProfileEditorItemMeta
    {
        public ProfileEditorItemMeta()
        {
        }

        public int Id { get; set; }
        public int Type { get; set; }
        public bool? Show { get; set; }
        public bool? PrimaryValue { get; set; }
    }
}

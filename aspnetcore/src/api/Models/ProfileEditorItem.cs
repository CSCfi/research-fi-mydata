using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class ProfileEditorItem
    {
        public ProfileEditorItem()
        {
        }

        public int Id { get; set; }
        public int FieldIdentifier { get; set; }
        public bool Show { get; set; }
        public string Name { get; set; }
        public ProfileEditorWebLink WebLink { get; set; }
        public string SourceId { get; set; }
    }
}

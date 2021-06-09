using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorSource
    {
        public ProfileEditorSource()
        {
        }

        public int Id { get; set; }
        public string RegisteredDataSource { get; set; }
        public ProfileEditorSourceOrganization Organization { get; set; }
    }
}

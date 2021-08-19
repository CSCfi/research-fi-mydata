using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupResearcherDescription : ProfileEditorGroup
    {
        public ProfileEditorGroupResearcherDescription()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemResearcherDescription> items { get; set; }
    }
}

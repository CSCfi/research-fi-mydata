﻿using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupWebLink : ProfileEditorGroup
    {
        public ProfileEditorGroupWebLink()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemWebLink> items { get; set; }
    }
}

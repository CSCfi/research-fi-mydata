﻿using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupName : ProfileEditorGroup
    {
        public ProfileEditorGroupName()
        {
        }

        public ProfileEditorSource source { get; set; }
        public List<ProfileEditorItemName> items { get; set; }
    }
}

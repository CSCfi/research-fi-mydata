﻿using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorDataActivity    {
        public ProfileEditorDataActivity()
        {
            educationGroups = new List<ProfileEditorGroupEducation>();
        }

        public List<ProfileEditorGroupEducation> educationGroups { get; set; }
    }
}

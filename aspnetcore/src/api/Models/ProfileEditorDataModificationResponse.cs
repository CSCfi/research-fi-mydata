﻿using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorDataModificationResponse    {
        public ProfileEditorDataModificationResponse()
        {
            groups = new List<ProfileEditorGroupMeta>();
            items = new List<ProfileEditorItemMeta>();
        }

        public List<ProfileEditorGroupMeta> groups { get; set; }
        public List<ProfileEditorItemMeta> items { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorGroupTelephoneNumber : ProfileEditorGroup
    {
        public ProfileEditorGroupTelephoneNumber()
        {
        }

        public ProfileEditorDataSource dataSource { get; set; }
        public List<ProfileEditorItemTelephoneNumber> items { get; set; }
    }
}

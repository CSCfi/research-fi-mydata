using System;
using System.Collections.Generic;
using api.Models;

namespace api.Models
{
    public partial class ProfileEditorDataResponse    {
        public ProfileEditorDataResponse()
        {
            personal = new ProfileEditorDataPersonal();
            activity = new ProfileEditorDataActivity();
        }

        public ProfileEditorDataPersonal personal { get; set; }
        public ProfileEditorDataActivity activity { get; set; }
    }
}

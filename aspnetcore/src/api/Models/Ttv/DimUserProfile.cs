using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimUserProfile
    {
        public DimUserProfile()
        {
            BrGrantedPermissions = new HashSet<BrGrantedPermission>();
            DimFieldDisplaySettings = new HashSet<DimFieldDisplaySetting>();
            DimUserChoices = new HashSet<DimUserChoice>();
            FactFieldValues = new HashSet<FactFieldValue>();
        }

        public int Id { get; set; }
        public int DimKnownPersonId { get; set; }
        public bool AllowAllSubscriptions { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string OrcidAccessToken { get; set; }
        public string OrcidRefreshToken { get; set; }
        public string OrcidTokenScope { get; set; }
        public DateTime? OrcidTokenExpires { get; set; }
        public string OrcidId { get; set; }
        public int? Statuscode { get; set; }
        public bool Hidden { get; set; }

        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual ICollection<BrGrantedPermission> BrGrantedPermissions { get; set; }
        public virtual ICollection<DimFieldDisplaySetting> DimFieldDisplaySettings { get; set; }
        public virtual ICollection<DimUserChoice> DimUserChoices { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class BrGrantedPermission
    {
        public int DimUserProfileId { get; set; }
        public int DimExternalServiceId { get; set; }
        public int DimPermittedFieldGroup { get; set; }

        public virtual DimPurpose DimExternalService { get; set; }
        public virtual DimReferencedatum DimPermittedFieldGroupNavigation { get; set; }
        public virtual DimUserProfile DimUserProfile { get; set; }
    }
}

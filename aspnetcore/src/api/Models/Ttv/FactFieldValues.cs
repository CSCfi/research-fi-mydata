using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class FactFieldValues
    {
        public int DimUserProfileId { get; set; }
        public int DimFieldDisplaySettingsId { get; set; }
        public int DimNameId { get; set; }
        public int DimWebLinkId { get; set; }
        public int DimFundingDecisionId { get; set; }
        public int DimPublicationId { get; set; }
        public int DimPidId { get; set; }
        public int DimPidIdOrcidPutCode { get; set; }
        public bool? Show { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimFieldDisplaySettings DimFieldDisplaySettings { get; set; }
        public virtual DimName DimName { get; set; }
        public virtual DimPid DimPid { get; set; }
        public virtual DimPid DimPidIdOrcidPutCodeNavigation { get; set; }
        public virtual DimUserProfile DimUserProfile { get; set; }
        public virtual DimWebLink DimWebLink { get; set; }
    }
}

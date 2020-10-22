using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class FactFieldDisplayContent
    {
        public int DimUserProfileId { get; set; }
        public int DimFieldDisplaySettingsId { get; set; }
        public string DimPidPidContent { get; set; }
        public int? DimNameId { get; set; }
        public int? DimWebLinkId { get; set; }
        public int? DimFundingDecisionId { get; set; }
        public int? DimPublicationId { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimFieldDisplaySettings DimFieldDisplaySettings { get; set; }
        public virtual DimFundingDecision DimFundingDecision { get; set; }
        public virtual DimName DimName { get; set; }
        public virtual DimPid DimPidPidContentNavigation { get; set; }
        public virtual DimPublication DimPublication { get; set; }
        public virtual DimUserProfile DimUserProfile { get; set; }
        public virtual DimWebLink DimWebLink { get; set; }
    }
}

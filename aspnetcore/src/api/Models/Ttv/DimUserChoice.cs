using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimUserChoice
    {
        public int Id { get; set; }
        public bool UserChoiceValue { get; set; }
        public int DimUserProfileId { get; set; }
        public int DimReferencedataIdAsUserChoiceLabel { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual DimReferencedatum DimReferencedataIdAsUserChoiceLabelNavigation { get; set; }
        public virtual DimUserProfile DimUserProfile { get; set; }
    }
}

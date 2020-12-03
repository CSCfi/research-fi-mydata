using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimReferencedata
    {
        public DimReferencedata()
        {
            BrLanguageCodesForDatasets = new HashSet<BrLanguageCodesForDatasets>();
            DimPublicationArticleTypeCodeNavigation = new HashSet<DimPublication>();
            DimPublicationParentPublicationTypeCodeNavigation = new HashSet<DimPublication>();
            DimPublicationPublicationTypeCode2Navigation = new HashSet<DimPublication>();
            DimPublicationTargetAudienceCodeNavigation = new HashSet<DimPublication>();
            FactJufoClassCodesForPubChannels = new HashSet<FactJufoClassCodesForPubChannels>();
        }

        public int Id { get; set; }
        public string CodeScheme { get; set; }
        public string CodeValue { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<BrLanguageCodesForDatasets> BrLanguageCodesForDatasets { get; set; }
        public virtual ICollection<DimPublication> DimPublicationArticleTypeCodeNavigation { get; set; }
        public virtual ICollection<DimPublication> DimPublicationParentPublicationTypeCodeNavigation { get; set; }
        public virtual ICollection<DimPublication> DimPublicationPublicationTypeCode2Navigation { get; set; }
        public virtual ICollection<DimPublication> DimPublicationTargetAudienceCodeNavigation { get; set; }
        public virtual ICollection<FactJufoClassCodesForPubChannels> FactJufoClassCodesForPubChannels { get; set; }
    }
}

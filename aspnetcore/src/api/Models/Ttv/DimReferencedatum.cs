using System;
using System.Collections.Generic;

#nullable disable

namespace api.Models.Ttv
{
    public partial class DimReferencedatum
    {
        public DimReferencedatum()
        {
            BrDimReferencedataDimCallProgrammes = new HashSet<BrDimReferencedataDimCallProgramme>();
            BrKeywordDimKeywords = new HashSet<BrKeywordDimKeyword>();
            BrLanguageCodesForDatasets = new HashSet<BrLanguageCodesForDataset>();
            BrLicensesForResearchDatasets = new HashSet<BrLicensesForResearchDataset>();
            DimAffiliationAffiliationTypeNavigations = new HashSet<DimAffiliation>();
            DimAffiliationPositionCodeNavigations = new HashSet<DimAffiliation>();
            DimEducations = new HashSet<DimEducation>();
            DimKeywords = new HashSet<DimKeyword>();
            DimNewsFeeds = new HashSet<DimNewsFeed>();
            DimOrcidPublicationArticleTypeCodeNavigations = new HashSet<DimOrcidPublication>();
            DimOrcidPublicationDimReferencedata = new HashSet<DimOrcidPublication>();
            DimOrcidPublicationParentPublicationTypeCodeNavigations = new HashSet<DimOrcidPublication>();
            DimOrcidPublicationPublicationTypeCode2Navigations = new HashSet<DimOrcidPublication>();
            DimOrcidPublicationPublicationTypeCodeNavigations = new HashSet<DimOrcidPublication>();
            DimOrcidPublicationTargetAudienceCodeNavigations = new HashSet<DimOrcidPublication>();
            DimPublicationArticleTypeCodeNavigations = new HashSet<DimPublication>();
            DimPublicationParentPublicationTypeCodeNavigations = new HashSet<DimPublication>();
            DimPublicationPublicationTypeCode2Navigations = new HashSet<DimPublication>();
            DimPublicationTargetAudienceCodeNavigations = new HashSet<DimPublication>();
            DimResearchDatasets = new HashSet<DimResearchDataset>();
            DimUserChoices = new HashSet<DimUserChoice>();
            FactContributions = new HashSet<FactContribution>();
            FactJufoClassCodesForPubChannels = new HashSet<FactJufoClassCodesForPubChannel>();
        }

        public int Id { get; set; }
        public string CodeScheme { get; set; }
        public string CodeValue { get; set; }
        public int? Order { get; set; }
        public string State { get; set; }
        public string NameFi { get; set; }
        public string NameEn { get; set; }
        public string NameSv { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<BrDimReferencedataDimCallProgramme> BrDimReferencedataDimCallProgrammes { get; set; }
        public virtual ICollection<BrKeywordDimKeyword> BrKeywordDimKeywords { get; set; }
        public virtual ICollection<BrLanguageCodesForDataset> BrLanguageCodesForDatasets { get; set; }
        public virtual ICollection<BrLicensesForResearchDataset> BrLicensesForResearchDatasets { get; set; }
        public virtual ICollection<DimAffiliation> DimAffiliationAffiliationTypeNavigations { get; set; }
        public virtual ICollection<DimAffiliation> DimAffiliationPositionCodeNavigations { get; set; }
        public virtual ICollection<DimEducation> DimEducations { get; set; }
        public virtual ICollection<DimKeyword> DimKeywords { get; set; }
        public virtual ICollection<DimNewsFeed> DimNewsFeeds { get; set; }
        public virtual ICollection<DimOrcidPublication> DimOrcidPublicationArticleTypeCodeNavigations { get; set; }
        public virtual ICollection<DimOrcidPublication> DimOrcidPublicationDimReferencedata { get; set; }
        public virtual ICollection<DimOrcidPublication> DimOrcidPublicationParentPublicationTypeCodeNavigations { get; set; }
        public virtual ICollection<DimOrcidPublication> DimOrcidPublicationPublicationTypeCode2Navigations { get; set; }
        public virtual ICollection<DimOrcidPublication> DimOrcidPublicationPublicationTypeCodeNavigations { get; set; }
        public virtual ICollection<DimOrcidPublication> DimOrcidPublicationTargetAudienceCodeNavigations { get; set; }
        public virtual ICollection<DimPublication> DimPublicationArticleTypeCodeNavigations { get; set; }
        public virtual ICollection<DimPublication> DimPublicationParentPublicationTypeCodeNavigations { get; set; }
        public virtual ICollection<DimPublication> DimPublicationPublicationTypeCode2Navigations { get; set; }
        public virtual ICollection<DimPublication> DimPublicationTargetAudienceCodeNavigations { get; set; }
        public virtual ICollection<DimResearchDataset> DimResearchDatasets { get; set; }
        public virtual ICollection<DimUserChoice> DimUserChoices { get; set; }
        public virtual ICollection<FactContribution> FactContributions { get; set; }
        public virtual ICollection<FactJufoClassCodesForPubChannel> FactJufoClassCodesForPubChannels { get; set; }
    }
}

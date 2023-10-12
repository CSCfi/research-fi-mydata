using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimReferencedatum
{
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

    public string State { get; set; }

    public int DimReferencedataId { get; set; }

    public int? Order { get; set; }

    public virtual ICollection<BrGrantedPermission> BrGrantedPermissions { get; set; } = new List<BrGrantedPermission>();

    public virtual ICollection<DimAffiliation> DimAffiliations { get; set; } = new List<DimAffiliation>();

    public virtual ICollection<DimEducation> DimEducations { get; set; } = new List<DimEducation>();

    public virtual ICollection<DimLocallyReportedPubInfo> DimLocallyReportedPubInfoSelfArchivedLicenseCodeNavigations { get; set; } = new List<DimLocallyReportedPubInfo>();

    public virtual ICollection<DimLocallyReportedPubInfo> DimLocallyReportedPubInfoSelfArchivedVersionCodeNavigations { get; set; } = new List<DimLocallyReportedPubInfo>();

    public virtual ICollection<DimProfileOnlyDataset> DimProfileOnlyDatasets { get; set; } = new List<DimProfileOnlyDataset>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublicationArticleTypeCodeNavigations { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublicationLanguageCodeNavigations { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublicationLicenseCodeNavigations { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublicationParentTypeClassificationCodeNavigations { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublicationPublicationCountryCodeNavigations { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublicationPublicationFormatCodeNavigations { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublicationTargetAudienceCodeNavigations { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublicationThesisTypeCodeNavigations { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimProfileOnlyPublication> DimProfileOnlyPublicationTypeClassificationCodeNavigations { get; set; } = new List<DimProfileOnlyPublication>();

    public virtual ICollection<DimPublication> DimPublicationArticleTypeCodeNavigations { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimPublication> DimPublicationFieldOfArtCodeNavigations { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimPublication> DimPublicationLanguageCodeNavigations { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimPublication> DimPublicationLicenseCodeNavigations { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimPublication> DimPublicationParentPublicationTypeCodeNavigations { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimPublication> DimPublicationPublicationCountryCodeNavigations { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimPublication> DimPublicationPublicationTypeCode2Navigations { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimPublication> DimPublicationPublicationTypeCodeNavigations { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimPublication> DimPublicationTargetAudienceCodeNavigations { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimPublication> DimPublicationThesisTypeCodeNavigations { get; set; } = new List<DimPublication>();

    public virtual DimReferencedatum DimReferencedata { get; set; }

    public virtual ICollection<DimResearchDataset> DimResearchDatasetDimReferencedataAvailabilityNavigations { get; set; } = new List<DimResearchDataset>();

    public virtual ICollection<DimResearchDataset> DimResearchDatasetDimReferencedataLicenseNavigations { get; set; } = new List<DimResearchDataset>();

    public virtual ICollection<DimUserChoice> DimUserChoices { get; set; } = new List<DimUserChoice>();

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactDimReferencedataFieldOfScience> FactDimReferencedataFieldOfSciences { get; set; } = new List<FactDimReferencedataFieldOfScience>();

    public virtual ICollection<FactFieldValue> FactFieldValueDimReferencedataActorRoles { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<FactFieldValue> FactFieldValueDimReferencedataFieldOfSciences { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<FactJufoClassCodesForPubChannel> FactJufoClassCodesForPubChannels { get; set; } = new List<FactJufoClassCodesForPubChannel>();

    public virtual ICollection<DimReferencedatum> InverseDimReferencedata { get; set; } = new List<DimReferencedatum>();

    public virtual ICollection<DimCallProgramme> DimCallProgrammes { get; set; } = new List<DimCallProgramme>();

    public virtual ICollection<DimPublication> DimPublications { get; set; } = new List<DimPublication>();

    public virtual ICollection<DimResearchDataset> DimResearchDatasets { get; set; } = new List<DimResearchDataset>();
}

using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimPublication
{
    public int Id { get; set; }

    public int? ReportingYear { get; set; }

    public string PublicationId { get; set; }

    public int PublicationStatusCode { get; set; }

    public string PublicationOrgId { get; set; }

    public string PublicationName { get; set; }

    public string AuthorsText { get; set; }

    public int? NumberOfAuthors { get; set; }

    public string PageNumberText { get; set; }

    public string ArticleNumberText { get; set; }

    public int PublicationCountryCode { get; set; }

    public string JournalName { get; set; }

    public string Volume { get; set; }

    public string IssueNumber { get; set; }

    public string ConferenceName { get; set; }

    public string PublisherName { get; set; }

    public string PublisherLocation { get; set; }

    public string ParentPublicationName { get; set; }

    public string ParentPublicationEditors { get; set; }

    public int PublicationTypeCode { get; set; }

    public bool? InternationalCollaboration { get; set; }

    public int InternationalPublication { get; set; }

    public int LanguageCode { get; set; }

    public bool? BusinessCollaboration { get; set; }

    public string DoiHandle { get; set; }

    public string OriginalPublicationId { get; set; }

    public int? PublicationYear { get; set; }

    public int LicenseCode { get; set; }

    public decimal? ApcFeeEur { get; set; }

    public int? ApcPaymentYear { get; set; }

    public int? PublicationTypeCode2 { get; set; }

    public int? TargetAudienceCode { get; set; }

    public int? ParentPublicationTypeCode { get; set; }

    public int? ArticleTypeCode { get; set; }

    public bool? PeerReviewed { get; set; }

    public bool? Report { get; set; }

    public int ThesisTypeCode { get; set; }

    public bool? SelfArchivedCode { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int DimRegisteredDataSourceId { get; set; }

    public int PublisherOpenAccessCode { get; set; }

    public int DimPublicationChannelId { get; set; }

    public int JufoClass { get; set; }

    public int? DimPublicationId { get; set; }

    public int JufoClassCodeFrozen { get; set; }

    public int DimPublicationChannelIdFrozen { get; set; }

    public int OpenAccessCode { get; set; }

    public virtual DimReferencedatum ArticleTypeCodeNavigation { get; set; }

    public virtual ICollection<DimDescriptiveItem> DimDescriptiveItems { get; set; } = new List<DimDescriptiveItem>();

    public virtual ICollection<DimLocallyReportedPubInfo> DimLocallyReportedPubInfos { get; set; } = new List<DimLocallyReportedPubInfo>();

    public virtual ICollection<DimPid> DimPids { get; set; } = new List<DimPid>();

    public virtual DimPublicationChannel DimPublicationChannel { get; set; }

    public virtual DimPublicationChannel DimPublicationChannelIdFrozenNavigation { get; set; }

    public virtual DimPublication DimPublicationNavigation { get; set; }

    public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }

    public virtual ICollection<FactContribution> FactContributions { get; set; } = new List<FactContribution>();

    public virtual ICollection<FactDimReferencedataFieldOfScience> FactDimReferencedataFieldOfSciences { get; set; } = new List<FactDimReferencedataFieldOfScience>();

    public virtual ICollection<FactFieldValue> FactFieldValues { get; set; } = new List<FactFieldValue>();

    public virtual ICollection<FactRelation> FactRelationFromPublications { get; set; } = new List<FactRelation>();

    public virtual ICollection<FactRelation> FactRelationToPublications { get; set; } = new List<FactRelation>();

    public virtual ICollection<DimPublication> InverseDimPublicationNavigation { get; set; } = new List<DimPublication>();

    public virtual DimReferencedatum JufoClassCodeFrozenNavigation { get; set; }

    public virtual DimReferencedatum JufoClassNavigation { get; set; }

    public virtual DimReferencedatum LanguageCodeNavigation { get; set; }

    public virtual DimReferencedatum LicenseCodeNavigation { get; set; }

    public virtual DimReferencedatum OpenAccessCodeNavigation { get; set; }

    public virtual DimReferencedatum ParentPublicationTypeCodeNavigation { get; set; }

    public virtual DimReferencedatum PublicationCountryCodeNavigation { get; set; }

    public virtual DimReferencedatum PublicationStatusCodeNavigation { get; set; }

    public virtual DimReferencedatum PublicationTypeCode2Navigation { get; set; }

    public virtual DimReferencedatum PublicationTypeCodeNavigation { get; set; }

    public virtual DimReferencedatum PublisherOpenAccessCodeNavigation { get; set; }

    public virtual DimReferencedatum TargetAudienceCodeNavigation { get; set; }

    public virtual DimReferencedatum ThesisTypeCodeNavigation { get; set; }

    public virtual ICollection<DimKeyword> DimKeywords { get; set; } = new List<DimKeyword>();

    public virtual ICollection<DimReferencedatum> DimReferencedata { get; set; } = new List<DimReferencedatum>();

    public virtual ICollection<DimReferencedatum> DimReferencedataNavigation { get; set; } = new List<DimReferencedatum>();
}

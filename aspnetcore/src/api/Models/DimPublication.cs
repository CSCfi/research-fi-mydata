using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DimPublication
    {
        public DimPublication()
        {
            BrFieldOfArtDimPublication = new HashSet<BrFieldOfArtDimPublication>();
            BrFieldOfEducationDimPublication = new HashSet<BrFieldOfEducationDimPublication>();
            BrFieldOfScienceDimPublication = new HashSet<BrFieldOfScienceDimPublication>();
            BrKeywordDimPublication = new HashSet<BrKeywordDimPublication>();
            DimLocallyReportedPubInfo = new HashSet<DimLocallyReportedPubInfo>();
            DimPid = new HashSet<DimPid>();
            FactContribution = new HashSet<FactContribution>();
        }

        public int Id { get; set; }
        public int? ReportingYear { get; set; }
        public string PublicationId { get; set; }
        public string PublicationStatusCode { get; set; }
        public string PublicationOrgId { get; set; }
        public string PublicationName { get; set; }
        public string AuthorsText { get; set; }
        public int? NumberOfAuthors { get; set; }
        public string PageNumberText { get; set; }
        public string ArticleNumberText { get; set; }
        public string Isbn { get; set; }
        public string Isbn2 { get; set; }
        public string JufoCode { get; set; }
        public string JufoClassCode { get; set; }
        public string PublicationCountryCode { get; set; }
        public string JournalName { get; set; }
        public string Issn { get; set; }
        public string Issn2 { get; set; }
        public string Volume { get; set; }
        public string IssueNumber { get; set; }
        public string ConferenceName { get; set; }
        public string PublisherName { get; set; }
        public string PublisherLocation { get; set; }
        public string ParentPublicationName { get; set; }
        public string ParentPublicationPublisher { get; set; }
        public string PublicationTypeCode { get; set; }
        public bool InternationalCollaboration { get; set; }
        public bool HospitalDistrictCollaboration { get; set; }
        public bool InternationalPublication { get; set; }
        public bool GovermentCollaboration { get; set; }
        public bool OtherCollaboration { get; set; }
        public string LanguageCode { get; set; }
        public string OpenAccessCode { get; set; }
        public bool SpecialStateSubsidy { get; set; }
        public bool BusinessCollaboration { get; set; }
        public string DoiHandle { get; set; }
        public string JuuliAddress { get; set; }
        public string OriginalPublicationId { get; set; }
        public string Doi { get; set; }
        public int? PublicationYear { get; set; }
        public int? LicenseCode { get; set; }
        public decimal? ApcFeeEur { get; set; }
        public int? ApcPaymentYear { get; set; }
        public int PublicationTypeCode2 { get; set; }
        public int TargetAudienceCode { get; set; }
        public int ParentPublicationTypeCode { get; set; }
        public int ArticleTypeCode { get; set; }
        public string SelfArchivedCode { get; set; }
        public bool? PeerReviewed { get; set; }
        public bool? Report { get; set; }
        public int? ThesisTypeCode { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<BrFieldOfArtDimPublication> BrFieldOfArtDimPublication { get; set; }
        public virtual ICollection<BrFieldOfEducationDimPublication> BrFieldOfEducationDimPublication { get; set; }
        public virtual ICollection<BrFieldOfScienceDimPublication> BrFieldOfScienceDimPublication { get; set; }
        public virtual ICollection<BrKeywordDimPublication> BrKeywordDimPublication { get; set; }
        public virtual ICollection<DimLocallyReportedPubInfo> DimLocallyReportedPubInfo { get; set; }
        public virtual ICollection<DimPid> DimPid { get; set; }
        public virtual ICollection<FactContribution> FactContribution { get; set; }
    }
}

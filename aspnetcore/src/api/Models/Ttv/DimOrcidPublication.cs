using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimOrcidPublication
    {
        public DimOrcidPublication()
        {
            DimPids = new HashSet<DimPid>();
            FactFieldValues = new HashSet<FactFieldValue>();
            InverseDimParentOrcidPublication = new HashSet<DimOrcidPublication>();
        }

        public int Id { get; set; }
        public int? DimParentOrcidPublicationId { get; set; }
        public int ParentPublicationTypeCode { get; set; }
        public int PublicationTypeCode { get; set; }
        public int PublicationTypeCode2 { get; set; }
        public int ArticleTypeCode { get; set; }
        public int TargetAudienceCode { get; set; }
        public string OrcidWorkType { get; set; }
        public string PublicationName { get; set; }
        public string ConferenceName { get; set; }
        public string ShortDescription { get; set; }
        public int? PublicationYear { get; set; }
        public string PublicationId { get; set; }
        public string AuthorsText { get; set; }
        public int? NumberOfAuthors { get; set; }
        public string PageNumberText { get; set; }
        public string ArticleNumberText { get; set; }
        public string IssueNumber { get; set; }
        public string Volume { get; set; }
        public string PublicationCountryCode { get; set; }
        public string PublisherName { get; set; }
        public string PublisherLocation { get; set; }
        public string ParentPublicationName { get; set; }
        public string ParentPublicationEditors { get; set; }
        public int? LicenseCode { get; set; }
        public string LanguageCode { get; set; }
        public string OpenAccessCode { get; set; }
        public string OriginalPublicationId { get; set; }
        public bool? PeerReviewed { get; set; }
        public bool? Report { get; set; }
        public int? ThesisTypeCode { get; set; }
        public string DoiHandle { get; set; }
        public string SourceId { get; set; }
        public string SourceDescription { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public int OrcidPersonDataSource { get; set; }
        public int DimRegisteredDataSourceId { get; set; }

        public virtual DimReferencedatum ArticleTypeCodeNavigation { get; set; }
        public virtual DimOrcidPublication DimParentOrcidPublication { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual DimKnownPerson OrcidPersonDataSourceNavigation { get; set; }
        public virtual DimReferencedatum ParentPublicationTypeCodeNavigation { get; set; }
        public virtual DimReferencedatum PublicationTypeCode2Navigation { get; set; }
        public virtual DimReferencedatum PublicationTypeCodeNavigation { get; set; }
        public virtual DimReferencedatum TargetAudienceCodeNavigation { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
        public virtual ICollection<DimOrcidPublication> InverseDimParentOrcidPublication { get; set; }
    }
}

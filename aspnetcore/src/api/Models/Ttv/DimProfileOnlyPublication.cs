using System;
using System.Collections.Generic;

namespace api.Models.Ttv
{
    public partial class DimProfileOnlyPublication
    {
        public DimProfileOnlyPublication()
        {
            DimPids = new HashSet<DimPid>();
            FactFieldValues = new HashSet<FactFieldValue>();
            InverseDimProfileOnlyPublicationNavigation = new HashSet<DimProfileOnlyPublication>();
        }

        public int Id { get; set; }
        public int? DimProfileOnlyPublicationId { get; set; }
        public int ParentTypeClassificationCode { get; set; }
        /// <summary>
        /// code_schema = &apos;julkaisutyyppiluokitus&apos;
        /// </summary>
        public int TypeClassificationCode { get; set; }
        /// <summary>
        /// code_scheme = &apos;julkaisumuoto&apos;
        /// </summary>
        public int PublicationFormatCode { get; set; }
        /// <summary>
        /// code_scheme = &apos;Artikkelintyyppikoodi&apos;
        /// </summary>
        public int ArticleTypeCode { get; set; }
        /// <summary>
        /// code_scheme = &apos;julkaisunyleiso&apos;
        /// </summary>
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
        public int DimKnownPersonId { get; set; }
        public int DimRegisteredDataSourceId { get; set; }
        public int LanguageCode { get; set; }

        public virtual DimReferencedatum ArticleTypeCodeNavigation { get; set; }
        public virtual DimKnownPerson DimKnownPerson { get; set; }
        public virtual DimProfileOnlyPublication DimProfileOnlyPublicationNavigation { get; set; }
        public virtual DimRegisteredDataSource DimRegisteredDataSource { get; set; }
        public virtual DimReferencedatum LanguageCodeNavigation { get; set; }
        public virtual DimReferencedatum ParentTypeClassificationCodeNavigation { get; set; }
        public virtual DimReferencedatum PublicationFormatCodeNavigation { get; set; }
        public virtual DimReferencedatum TargetAudienceCodeNavigation { get; set; }
        public virtual DimReferencedatum TypeClassificationCodeNavigation { get; set; }
        public virtual ICollection<DimPid> DimPids { get; set; }
        public virtual ICollection<FactFieldValue> FactFieldValues { get; set; }
        public virtual ICollection<DimProfileOnlyPublication> InverseDimProfileOnlyPublicationNavigation { get; set; }
    }
}

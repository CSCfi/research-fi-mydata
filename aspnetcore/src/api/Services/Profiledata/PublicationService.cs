using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using api.Models.Common;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public class PublicationService : IPublicationService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly IDuplicateHandlerService _duplicateHandlerService;
        private readonly ILogger<PublicationService> _logger;


        public PublicationService(
            TtvContext ttvContext,
            ILanguageService languageService,
            IDuplicateHandlerService duplicateHandlerService,
            ILogger<PublicationService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _duplicateHandlerService = duplicateHandlerService;
            _logger = logger;
        }

        // DataSource DTO class.
        public class DataSourceDto
        {
            public int DimRegisteredDatasource_Id { get; set; }
            public string? DimRegisteredDatasource_Name { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameFi { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameEn { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameSv { get; set; }
            public string? DimregisteredDatasource_DimOrganization_DimSector_SectorId { get; set; }
        }

        // Publication query DTO class.
        // This should have properties to store values from both DimPublication and DimProfileOnlyPublication.
        public class PublicationDto
        {
            public bool IsProfileOnlyPublication { get; set; }
            public int Id { get; set; }
            public bool? Show { get; set; }
            public bool? PrimaryValue { get; set; }
            public int DimRegisteredDatasource_Id { get; set; }
            public string? DimRegisteredDatasource_Name { get; set; }
            public List<DataSourceDto> DataSources { get; set; }
            public string ArticleNumberText { get; set; }
            public string AuthorsText { get; set; }
            public string ConferenceName { get; set; }
            public string Doi { get; set; }
            public string DoiDictionaryKey { get; set; }
            public string IssueNumber { get; set; }
            public string JournalName { get; set; }
            public string PageNumberText { get; set; }
            public string ParentPublicationName { get; set; }
            public bool? PeerReviewed { get; set; }
            public string PublicationId { get; set; }
            public string PublicationIdDictionaryKey { get; set; }
            public string PublicationName { get; set; }
            public int? PublicationYear { get; set; }
            public string PublisherName { get; set; }
            public string PublicationTypeCode { get; set; }
            public string SelfArchivedAddress { get; set; }
            public bool? SelfArchivedCode { get; set; }
            public string OpenAccessCodeUnprocessed { get; set; }
            public int OpenAccessCode { get; set; }
            public string Volume { get; set; }
        }

        /*
         * Publications
         */
        public async Task<List<ProfileEditorPublication>> GetProfileEditorPublications(int userprofileId, bool forElasticsearch = false)
        { 
            // DimPublication => DTOs
            List<PublicationDto> publicationDtos = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfileId == userprofileId && ffv.DimPublicationId > 0
                    && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION
                    && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new PublicationDto()
                {
                    IsProfileOnlyPublication = false,
                    Id = ffv.DimPublicationId,
                    Show = ffv.Show,
                    PrimaryValue = ffv.PrimaryValue,
                    ArticleNumberText = ffv.DimPublication.ArticleNumberText,
                    AuthorsText = ffv.DimPublication.AuthorsText,
                    ConferenceName = ffv.DimPublication.ConferenceName,
                    DataSources = new List<DataSourceDto> {
                        new DataSourceDto() {
                            DimRegisteredDatasource_Id = ffv.DimRegisteredDataSourceId,
                            DimRegisteredDatasource_Name = ffv.DimRegisteredDataSource.Name,
                            DimRegisteredDatasource_DimOrganization_NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                            DimRegisteredDatasource_DimOrganization_NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                            DimRegisteredDatasource_DimOrganization_NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                            DimregisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId
                        }
                    },
                    Doi = ffv.DimPublication.DimPids.FirstOrDefault(pid => pid.PidType == Constants.PidTypes.DOI) != null ? ffv.DimPublication.DimPids.FirstOrDefault(pid => pid.PidType == Constants.PidTypes.DOI).PidContent : null,
                    DoiDictionaryKey = ffv.DimPublication.DimPids.FirstOrDefault(pid => pid.PidType == Constants.PidTypes.DOI) != null ? ffv.DimPublication.DimPids.FirstOrDefault(pid => pid.PidType == Constants.PidTypes.DOI).PidContent.Trim().ToLower() : null,
                    IssueNumber = ffv.DimPublication.IssueNumber,
                    JournalName = ffv.DimPublication.JournalName,
                    PageNumberText = ffv.DimPublication.PageNumberText,
                    ParentPublicationName = ffv.DimPublication.ParentPublicationName,
                    PeerReviewed = ffv.DimPublication.PeerReviewed,
                    OpenAccessCodeUnprocessed = ffv.DimPublication.OpenAccessCode != -1 ? ffv.DimPublication.OpenAccessCodeNavigation.CodeValue : "9", // Unknown value is set to 9
                    PublicationId = ffv.DimPublication.PublicationId,
                    PublicationIdDictionaryKey = ffv.DimPublication.PublicationId != null ? ffv.DimPublication.PublicationId.Trim().ToLower() : null,
                    PublicationName = ffv.DimPublication.PublicationName,
                    PublicationYear = ffv.DimPublication.PublicationYear,
                    PublisherName = ffv.DimPublication.PublisherName,
                    PublicationTypeCode = ffv.DimPublication.PublicationTypeCode != -1 ? ffv.DimPublication.PublicationTypeCodeNavigation.CodeValue : "", // Unknown value is set to empty string
                    SelfArchivedAddress = ffv.DimPublication.DimLocallyReportedPubInfos.FirstOrDefault() != null ? ffv.DimPublication.DimLocallyReportedPubInfos.FirstOrDefault().SelfArchivedUrl : null,
                    SelfArchivedCode = ffv.DimPublication.SelfArchivedCode,
                    Volume = ffv.DimPublication.Volume
                }).AsNoTracking().ToListAsync();

            // DimProfileOnlyPublication => DTOs
            List<PublicationDto> profileOnlyPublicationDtos = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfileId == userprofileId && ffv.DimProfileOnlyPublicationId > 0
                    && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY
                    && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new PublicationDto()
                {
                    IsProfileOnlyPublication = true,
                    Id = ffv.DimProfileOnlyPublicationId,
                    Show = ffv.Show,
                    PrimaryValue = ffv.PrimaryValue,
                    ArticleNumberText = ffv.DimProfileOnlyPublication.ArticleNumberText ?? "",
                    AuthorsText = ffv.DimProfileOnlyPublication.AuthorsText ?? "",
                    ConferenceName = ffv.DimProfileOnlyPublication.ConferenceName ?? "",
                    DataSources = new List<DataSourceDto> {
                        new DataSourceDto() {
                            DimRegisteredDatasource_Id = ffv.DimRegisteredDataSourceId,
                            DimRegisteredDatasource_Name = ffv.DimRegisteredDataSource.Name,
                            DimRegisteredDatasource_DimOrganization_NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                            DimRegisteredDatasource_DimOrganization_NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                            DimRegisteredDatasource_DimOrganization_NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                            DimregisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId
                        }
                    },
                    Doi = ffv.DimProfileOnlyPublication.DoiHandle,
                    DoiDictionaryKey = ffv.DimProfileOnlyPublication.DoiHandle != null ? ffv.DimProfileOnlyPublication.DoiHandle.Trim().ToLower() : null,
                    IssueNumber = ffv.DimProfileOnlyPublication.IssueNumber ?? "",
                    JournalName = "",
                    OpenAccessCodeUnprocessed = ffv.DimProfileOnlyPublication.OpenAccessCode != null ? ffv.DimProfileOnlyPublication.OpenAccessCode : "0", // Default value for profile-only publications is set to "0" (not open access)
                    PageNumberText = ffv.DimProfileOnlyPublication.PageNumberText ?? "",
                    ParentPublicationName = ffv.DimProfileOnlyPublication.ParentPublicationName ?? "",
                    PeerReviewed = ffv.DimProfileOnlyPublication.PeerReviewed,
                    PublicationId = ffv.DimProfileOnlyPublication.PublicationId,
                    PublicationIdDictionaryKey = ffv.DimProfileOnlyPublication.PublicationId != null ? ffv.DimProfileOnlyPublication.PublicationId.Trim().ToLower() : null,
                    PublicationName = ffv.DimProfileOnlyPublication.PublicationName,
                    PublicationTypeCode = "",
                    PublicationYear = ffv.DimProfileOnlyPublication.PublicationYear,
                    PublisherName = ffv.DimProfileOnlyPublication.PublisherName ?? "",
                    SelfArchivedAddress = "",
                    SelfArchivedCode = false,
                    Volume = ffv.DimProfileOnlyPublication.Volume ?? ""
                }).AsNoTracking().ToListAsync();

            var publicationIdDict = new Dictionary<string, PublicationDto>();
            var doiDict = new Dictionary<string, PublicationDto>();
            List<PublicationDto> processedProfileOnlyPublications = new List<PublicationDto>();

            // Local function for processing publications and adding them to the dictionaries with deduplication logic.
            void ProcessItem(PublicationDto newPublication)
            {
                PublicationDto existingPublicationDto = null;

                // CHECK 1: Match by PublicationId. Not for profileOnlyPublications.
                if (!newPublication.IsProfileOnlyPublication && !string.IsNullOrWhiteSpace(newPublication.PublicationIdDictionaryKey))
                {
                    publicationIdDict.TryGetValue(newPublication.PublicationIdDictionaryKey, out existingPublicationDto);
                }

                // CHECK 2: If not found by PublicationId and DOI is available, match by DOI. Only for profileOnlyPublications.
                if (newPublication.IsProfileOnlyPublication && existingPublicationDto == null && !string.IsNullOrWhiteSpace(newPublication.DoiDictionaryKey))
                {
                    doiDict.TryGetValue(newPublication.DoiDictionaryKey, out existingPublicationDto);
                }

                // DEDUPLICATION LOGIC:
                if (
                    existingPublicationDto != null &&
                    (
                        !newPublication.IsProfileOnlyPublication
                        ||
                        (
                            newPublication.IsProfileOnlyPublication &&
                            !_duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                                publicationName: newPublication.PublicationName,
                                ttvPublicationName: existingPublicationDto.PublicationName,
                                ttvPublicationTypeCode: existingPublicationDto.PublicationTypeCode
                            )
                        )
                    )
                )                
                {
                    // DUPLICATE FOUND: Merge data sources. Ensure that data sources are unique after merging.
                    existingPublicationDto.DataSources = existingPublicationDto.DataSources
                        .Concat(newPublication.DataSources)
                        .GroupBy(ds => ds.DimRegisteredDatasource_Id)
                        .Select(g => g.First())
                        .ToList();

                    if (!existingPublicationDto.IsProfileOnlyPublication)
                    {
                        if (!string.IsNullOrWhiteSpace(existingPublicationDto.PublicationIdDictionaryKey))
                            publicationIdDict.TryAdd(existingPublicationDto.PublicationIdDictionaryKey, existingPublicationDto);

                        if (!string.IsNullOrWhiteSpace(existingPublicationDto.DoiDictionaryKey))
                            doiDict.TryAdd(existingPublicationDto.DoiDictionaryKey, existingPublicationDto);
                    }
                }
                else
                {
                    if (!newPublication.IsProfileOnlyPublication)
                    {
                        if (!string.IsNullOrWhiteSpace(newPublication.PublicationIdDictionaryKey))
                            publicationIdDict.Add(newPublication.PublicationIdDictionaryKey, newPublication);
                        
                        if (!string.IsNullOrWhiteSpace(newPublication.DoiDictionaryKey))                 
                            doiDict.Add(newPublication.DoiDictionaryKey, newPublication);
                    }
                    else
                        processedProfileOnlyPublications.Add(newPublication);
                }                
            }

            // Process publications (from DimPublication). These must be processed before profile-only publications, so that they take precedence in matching when both PublicationId and DOI are available.
            foreach (PublicationDto publicationDto in publicationDtos) ProcessItem(publicationDto);

            // Process profile-only publications (from DimProfileOnlyPublication)
            foreach (PublicationDto profileOnlyPublicationDto in profileOnlyPublicationDtos) ProcessItem(profileOnlyPublicationDto);

            // Extract unique publications
            List<PublicationDto> uniquePublications = publicationIdDict.Values.Concat(processedProfileOnlyPublications).Distinct().ToList();

            // Hard coded values for translating publications peer review status.
            string PeerReviewedCode = "1";
            string NotPeerReviewedCode = "0";
            string PeerReviewedFi = "Vertaisarvioitu";
            string NotPeerReviewedFi = "Ei-vertaisarvioitu";
            string PeerReviewedSv = "Kollegialt utvärderad";
            string NotPeerReviewedSv = "Inte kollegialt utvärderad";
            string PeerReviewedEn = "Peer-Reviewed";
            string NotPeerReviewedEn = "Non Peer-Reviewed";
            
            List<ProfileEditorPublication> publications = new List<ProfileEditorPublication>();
            foreach (PublicationDto publicationDto in uniquePublications)
            {
                // Parse open access code. If parsing fails, set to 9 (unknown value).
                int openAccessCode = int.TryParse(publicationDto.OpenAccessCodeUnprocessed, out int openAccessCodeParsed) ? openAccessCodeParsed : 9;

                publications.Add(new ProfileEditorPublication()
                {
                    ArticleNumberText = publicationDto.ArticleNumberText,
                    AuthorsText = publicationDto.AuthorsText,
                    ConferenceName = publicationDto.ConferenceName,
                    Doi = publicationDto.Doi,
                    IssueNumber = publicationDto.IssueNumber,
                    JournalName = publicationDto.JournalName,
                    OpenAccess = openAccessCode,
                    PageNumberText = publicationDto.PageNumberText,
                    ParentPublicationName = publicationDto.ParentPublicationName,
                    PeerReviewed = new List<ProfileEditorPublicationPeerReviewed> {
                        new ProfileEditorPublicationPeerReviewed()
                        {
                            Id = publicationDto.PeerReviewed != null && publicationDto.PeerReviewed.Value ? PeerReviewedCode : NotPeerReviewedCode,
                            NameFiPeerReviewed = publicationDto.PeerReviewed != null && publicationDto.PeerReviewed.Value ? PeerReviewedFi : NotPeerReviewedFi,
                            NameSvPeerReviewed = publicationDto.PeerReviewed != null && publicationDto.PeerReviewed.Value ? PeerReviewedSv : NotPeerReviewedSv,
                            NameEnPeerReviewed = publicationDto.PeerReviewed != null && publicationDto.PeerReviewed.Value ? PeerReviewedEn : NotPeerReviewedEn
                        }
                    },
                    PublicationId = publicationDto.PublicationId,
                    PublicationName = publicationDto.PublicationName,
                    PublicationYear = publicationDto.PublicationYear == null || publicationDto.PublicationYear < 1 ? null : publicationDto.PublicationYear,
                    PublisherName = publicationDto.PublisherName,
                    PublicationTypeCode = publicationDto.PublicationTypeCode,
                    SelfArchivedAddress = publicationDto.SelfArchivedAddress,
                    SelfArchivedCode = (publicationDto.SelfArchivedCode != null && (bool)publicationDto.SelfArchivedCode) ? "1" : "0",
                    Volume = publicationDto.Volume,
                    DataSources = publicationDto.DataSources.Select(dataSourceDto => new ProfileEditorSource()
                    {
                        Id = dataSourceDto.DimRegisteredDatasource_Id,
                        RegisteredDataSource = dataSourceDto.DimRegisteredDatasource_Name,
                        Organization = new Organization()
                        {
                            NameFi = dataSourceDto.DimRegisteredDatasource_DimOrganization_NameFi,
                            NameEn = dataSourceDto.DimRegisteredDatasource_DimOrganization_NameEn,
                            NameSv = dataSourceDto.DimRegisteredDatasource_DimOrganization_NameSv,
                            SectorId = dataSourceDto.DimregisteredDatasource_DimOrganization_DimSector_SectorId
                        }
                    }).ToList(),
                    itemMeta = new ProfileEditorItemMeta(
                        id: publicationDto.Id,
                        type: publicationDto.IsProfileOnlyPublication ? Constants.ItemMetaTypes.ACTIVITY_PUBLICATION_PROFILE_ONLY : Constants.ItemMetaTypes.ACTIVITY_PUBLICATION,
                        show: publicationDto.Show,
                        publicationDto.PrimaryValue
                    )
                });
            }

            // Postprocessing
            foreach (ProfileEditorPublication publication in publications)
            {
                // Translate data source organizaton names.
                foreach (ProfileEditorSource dataSource in publication.DataSources)
                {
                    NameTranslation dataSourceOrganizationName = _languageService.GetNameTranslation(
                        nameFi: dataSource.Organization.NameFi,
                        nameEn: dataSource.Organization.NameEn,
                        nameSv: dataSource.Organization.NameSv
                    );
                    dataSource.Organization.NameFi = dataSourceOrganizationName.NameFi;
                    dataSource.Organization.NameEn = dataSourceOrganizationName.NameEn;
                    dataSource.Organization.NameSv = dataSourceOrganizationName.NameSv;
                }
            }

            return publications;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using api.Models.Ttv;
using api.Models.Keycloak;
using api.Models.Log;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using ResearchFi.ProfilePublicApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace api.Services
{
    public class PublicApiService : IPublicApiService
    {
        private const string ConsentManagementPath = "/mydata/account-settings?login=1";
        private const string PublicationTypeCodeScheme = "julkaisutyyppiluokitus";
        private readonly TtvContext _ttvContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PublicApiService> _logger;
        private readonly IUserProfileService _userProfileService;
        private readonly IConfiguration _configuration;


        public PublicApiService(
            TtvContext ttvContext,
            IHttpClientFactory httpClientFactory,
            ILogger<PublicApiService> logger,
            IUserProfileService userProfileService,
            IConfiguration configuration)
        {
            _ttvContext = ttvContext;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _userProfileService = userProfileService;
            _configuration = configuration;
        }

        // Get profile data for public API. Only published (show=true) items are included.
        public async Task<ProfileDataResponse> GetProfileDataForPublicApi(ProfileDataRequest request)
        {
            DimUserProfile dimUserProfile = await FindUserProfile(request?.PersonIdentifiers);
            List<int> controllerOrganizationIds = await FindControllerOrganizationIds(request.ControllerROR);
            bool profileFound = dimUserProfile != null;
            bool controllerFound = controllerOrganizationIds.Count > 0;
            bool? consentActive = null;

            if (profileFound && controllerFound)
            {
                consentActive = await HasActiveConsent(dimUserProfile.Id, controllerOrganizationIds);
            }

            ProfileDataResponse response = new()
            {
                ProfileFound = profileFound,
                ControllerFound = controllerFound,
                ConsentActive = consentActive,
                ConsentManagementURL = profileFound
                    ? new LanguageVariant
                    {
                        Fi = GetConsentManagementUrl("PORTAL:URL:FI", request.ControllerROR),
                        Sv = GetConsentManagementUrl("PORTAL:URL:SV", request.ControllerROR),
                        En = GetConsentManagementUrl("PORTAL:URL:EN", request.ControllerROR)
                    }
                    : new LanguageVariant
                    {
                        Fi = string.Empty,
                        Sv = string.Empty,
                        En = string.Empty
                    }
            };

            if (!profileFound || dimUserProfile.Hidden || consentActive != true)
            {
                return response;
            }

            response.ProfileData = await GetPersonProfileData(dimUserProfile.Id, dimUserProfile.OrcidId);
            return response;
        }

        public async Task<PersonProfileData> GetPersonProfileData(int userProfileId, string orcidId)
        {
            ProfileEditorDataResponse profileData = await _userProfileService.GetProfileData(
                userprofileId: userProfileId,
                logUserIdentification: new LogUserIdentification(),
                forElasticsearch: true);
            Dictionary<string, ReferenceData> publicationClassifications = await GetPublicationClassifications(profileData.activity.publications);

            return new PersonProfileData
            {
                HasGrantedFunding = profileData.activity.fundingDecisions.Select(ToGrantedFunding).ToList(),
                DescriptionOfActivities = ToDescriptionOfActivities(profileData.personal.researcherDescriptions),
                IsNamedBy = ToPersonName(profileData.personal.names.FirstOrDefault()),
                IsReachableFrom = ToContactInformation(profileData.personal),
                ContributesToOutput = new List<Output>
                {
                    new Output
                    {
                        ContributesToWrittenPublication = profileData.activity.publications
                            .Select(publication => ToWrittenPublication(publication, publicationClassifications))
                            .ToList(),
                        ContributesToDataset = profileData.activity.researchDatasets.Select(ToResearchDataset).ToList(),
                        ContributesToApplication = new List<Software>(),
                        ContributesToAudiovisualPublication = new List<AudiovisualPublication>(),
                        ContributesToArtpublication = new List<ArtPublication>()
                    }
                },
                Affiliations = profileData.activity.affiliations.Select(ToAffiliation).ToList(),
                PersonIdentifier = new Identifier { PidType = api.Models.Common.Constants.PidTypes.ORCID, PidContent = orcidId },
                CollaborationInterests = await GetCollaborationInterests(userProfileId),
                HasEducations = profileData.activity.educations.Select(ToEducation).ToList()
            };
        }

        private string GetConsentManagementUrl(string configurationKey, string controllerRor)
        {
            string portalUrl = _configuration[configurationKey];
            if (string.IsNullOrWhiteSpace(portalUrl))
            {
                throw new InvalidOperationException($"Required configuration '{configurationKey}' is missing.");
            }

            return $"{portalUrl.TrimEnd('/')}{ConsentManagementPath}&rorid={Uri.EscapeDataString(controllerRor)}&returnurl={{URL_ENCODED_RETURN_URL}}";
        }

        // ORCID takes precedence when both supported identifiers are supplied.
        private async Task<DimUserProfile> FindUserProfile(IEnumerable<Identifier> personIdentifiers)
        {
            Identifier orcid = personIdentifiers?.FirstOrDefault(i => string.Equals(i?.PidType, api.Models.Common.Constants.PidTypes.ORCID, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(i.PidContent));
            if (orcid != null)
            {
                return await _userProfileService.GetUserprofile(orcid.PidContent);
            }

            Identifier hetu = personIdentifiers?.FirstOrDefault(i => string.Equals(i?.PidType, "HETU", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(i.PidContent));
            if (hetu != null)
            {
                return await _userProfileService.GetUserprofileByUsername(GetUsernameFromNationalIdentificationNumber(hetu.PidContent));
            }

            return null;
        }

        private async Task<List<int>> FindControllerOrganizationIds(string controllerROR)
        {
            string normalizedRor = controllerROR.Trim().ToUpperInvariant();
            return await _ttvContext.FactOrganizationGenerals
                .Where(controller => _ttvContext.DimPids.Any(pid =>
                    pid.DimOrganizationId == controller.DimOrganizationId
                    && pid.PidType.ToUpper() == "RORID"
                    && pid.PidContent.ToUpper() == normalizedRor))
                .Select(controller => controller.Id)
                .Distinct()
                .ToListAsync();
        }

        private async Task<bool> HasActiveConsent(int userProfileId, List<int> controllerOrganizationIds)
        {
            return await _ttvContext.FactProfileDataSharingConsents
                .AnyAsync(consent =>
                    consent.DimUserProfileId == userProfileId
                    && controllerOrganizationIds.Contains(consent.ControllerOrganizationId)
                    && consent.ConsentActive);
        }

        // The profile data cooperation items do not contain the reference data code, so query the selected choices directly.
        private async Task<List<ReferenceData>> GetCollaborationInterests(int userprofileId)
        {
            var choices = await _ttvContext.DimUserChoices
                .Where(c => c.DimUserProfileId == userprofileId && c.UserChoiceValue)
                .OrderBy(c => c.DimReferencedataIdAsUserChoiceLabelNavigation.Order)
                .Select(c => new
                {
                    c.DimReferencedataIdAsUserChoiceLabelNavigation.CodeValue,
                    c.DimReferencedataIdAsUserChoiceLabelNavigation.NameFi,
                    c.DimReferencedataIdAsUserChoiceLabelNavigation.NameSv,
                    c.DimReferencedataIdAsUserChoiceLabelNavigation.NameEn
                }).AsNoTracking().ToListAsync();

            return choices
                .Where(c => !string.IsNullOrWhiteSpace(c.CodeValue))
                .Select(c => new ReferenceData
                {
                    CodeValue = c.CodeValue,
                    CodeLabel = ToLanguageVariant(c.NameFi, c.NameSv, c.NameEn)
                }).ToList();
        }

        private async Task<Dictionary<string, ReferenceData>> GetPublicationClassifications(
            IEnumerable<ProfileEditorPublication> publications)
        {
            List<string> codeValues = publications
                .Select(publication => publication.PublicationTypeCode)
                .Where(codeValue => !string.IsNullOrWhiteSpace(codeValue))
                .Distinct()
                .ToList();

            if (codeValues.Count == 0)
            {
                return new Dictionary<string, ReferenceData>();
            }

            var classifications = await _ttvContext.DimReferencedata
                .Where(referenceData => referenceData.CodeScheme == PublicationTypeCodeScheme
                    && codeValues.Contains(referenceData.CodeValue))
                .Select(referenceData => new
                {
                    referenceData.CodeValue,
                    referenceData.NameFi,
                    referenceData.NameSv,
                    referenceData.NameEn
                })
                .AsNoTracking()
                .ToListAsync();

            return classifications
                .GroupBy(classification => classification.CodeValue)
                .ToDictionary(
                    group => group.Key,
                    group => new ReferenceData
                    {
                        CodeValue = group.Key,
                        CodeLabel = ToLanguageVariant(
                            group.First().NameFi,
                            group.First().NameSv,
                            group.First().NameEn)
                    });
        }

        private static string NullIfBlank(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static LanguageVariant ToLanguageVariant(string fi, string sv, string en)
        {
            return new LanguageVariant { Fi = NullIfBlank(fi), Sv = NullIfBlank(sv), En = NullIfBlank(en) };
        }

        // Profile editor uses 0 for missing date parts.
        private static PartialDate ToPartialDate(ProfileEditorDate date)
        {
            if (date == null || date.Year <= 0)
            {
                return null;
            }
            string datePartial = date.Year.ToString("D4", CultureInfo.InvariantCulture);
            if (date.Month > 0)
            {
                datePartial += $"-{date.Month.ToString("D2", CultureInfo.InvariantCulture)}";
                if (date.Day > 0)
                {
                    datePartial += $"-{date.Day.ToString("D2", CultureInfo.InvariantCulture)}";
                }
            }

            return new PartialDate
            {
                Year = date.Year,
                Month = date.Month > 0 ? date.Month : null,
                Day = date.Day > 0 ? date.Day : null,
                DatePartial = datePartial
            };
        }

        private static PartialDate ToPartialDate(int? year)
        {
            return year > 0
                ? new PartialDate { Year = year.Value, DatePartial = year.Value.ToString("D4", CultureInfo.InvariantCulture) }
                : null;
        }

        private static List<DescriptiveItem> ToDescriptiveItems(string fi, string sv, string en)
        {
            List<DescriptiveItem> items = new();
            if (!string.IsNullOrWhiteSpace(fi)) items.Add(new DescriptiveItem { DescriptiveContent = fi, Language = "fi" });
            if (!string.IsNullOrWhiteSpace(sv)) items.Add(new DescriptiveItem { DescriptiveContent = sv, Language = "sv" });
            if (!string.IsNullOrWhiteSpace(en)) items.Add(new DescriptiveItem { DescriptiveContent = en, Language = "en" });
            return items;
        }

        private static PersonName ToPersonName(ProfileEditorName name)
        {
            return name == null ? null : new PersonName { FirstName = name.FirstNames, LastName = name.LastName };
        }

        private static LanguageVariant ToDescriptionOfActivities(List<ProfileEditorResearcherDescription> descriptions)
        {
            ProfileEditorResearcherDescription description = descriptions.FirstOrDefault();
            return description == null ? null : ToLanguageVariant(description.ResearchDescriptionFi, description.ResearchDescriptionSv, description.ResearchDescriptionEn);
        }

        private static ContactInformation ToContactInformation(ProfileEditorDataPersonal personal)
        {
            return new ContactInformation
            {
                PhoneNumber = personal.telephoneNumbers.Select(t => t.Value).ToList(),
                Email = personal.emails.Select(e => e.Value).ToList(),
                Homepage = personal.webLinks.Select(w => new WebLink { WeblinkLabel = w.LinkLabel, WeblinkURL = w.Url }).ToList()
            };
        }

        // TODO: not handled yet: Organization identifiers (OrganizationIdentifier) of the funder.
        private static GrantedFunding ToGrantedFunding(ProfileEditorFundingDecision funding)
        {
            return new GrantedFunding
            {
                FundingStartsOn = ToPartialDate(funding.FundingStartYear),
                FundingEndsOn = ToPartialDate(funding.FundingEndYear),
                IsFundedBy = new Organization
                {
                    OrganizationIdentifier = new List<Identifier>(),
                    OrganizationName = ToLanguageVariant(funding.FunderNameFi, funding.FunderNameSv, funding.FunderNameEn)
                },
                GrantedFundingName = ToLanguageVariant(funding.ProjectNameFi, funding.ProjectNameSv, funding.ProjectNameEn)
            };
        }

        // TODO: not handled yet: organization identifiers and organization sector.
        private static Affiliation ToAffiliation(ProfileEditorAffiliation affiliation)
        {
            bool hasDepartmentName = !string.IsNullOrWhiteSpace(affiliation.DepartmentNameFi)
                || !string.IsNullOrWhiteSpace(affiliation.DepartmentNameSv)
                || !string.IsNullOrWhiteSpace(affiliation.DepartmentNameEn);
            LanguageVariant organizationName = ToLanguageVariant(
                affiliation.OrganizationNameFi,
                affiliation.OrganizationNameSv,
                affiliation.OrganizationNameEn);

            return new Affiliation
            {
                AffiliationStartsOn = ToPartialDate(affiliation.StartDate),
                AffiliationEndsOn = ToPartialDate(affiliation.EndDate),
                AffiliationType = ToLanguageVariant(affiliation.AffiliationTypeFi, affiliation.AffiliationTypeSv, affiliation.AffiliationTypeEn),
                AffiliationOrganization = new Organization
                {
                    OrganizationIdentifier = new List<Identifier>(),
                    OrganizationName = hasDepartmentName
                        ? ToLanguageVariant(affiliation.DepartmentNameFi, affiliation.DepartmentNameSv, affiliation.DepartmentNameEn)
                        : organizationName,
                    IsPartOfOrganization = hasDepartmentName
                        ? new Organization { OrganizationName = organizationName }
                        : null
                },
                Position = ToLanguageVariant(affiliation.PositionNameFi, affiliation.PositionNameSv, affiliation.PositionNameEn)
            };
        }

        private static Education ToEducation(ProfileEditorEducation education)
        {
            return new Education
            {
                EducationName = ToLanguageVariant(education.NameFi, education.NameSv, education.NameEn),
                EducationStartsOn = ToPartialDate(education.StartDate),
                EducationEndsOn = ToPartialDate(education.EndDate),
                DegreeGrantingInstitutionName = NullIfBlank(education.DegreeGrantingInstitutionName)
            };
        }

        // TODO: not handled yet: identifiers other than DOI, JuFo ids, ISSN, parent publication issue number.
        private static WrittenPublication ToWrittenPublication(
            ProfileEditorPublication publication,
            Dictionary<string, ReferenceData> publicationClassifications)
        {
            List<Identifier> identifiers = new();
            if (!string.IsNullOrWhiteSpace(publication.Doi))
            {
                identifiers.Add(new Identifier { PidContent = publication.Doi, PidType = "DOI" });
            }

            Publisher publisher = string.IsNullOrWhiteSpace(publication.PublisherName) ? null : new Publisher { PublisherName = publication.PublisherName };
            SerialPublication serie = string.IsNullOrWhiteSpace(publication.JournalName) ? null : new SerialPublication { JournalName = publication.JournalName, Volume = NullIfBlank(publication.Volume) };
            ConferenceEvent conference = string.IsNullOrWhiteSpace(publication.ConferenceName) ? null : new ConferenceEvent { ConferenceName = publication.ConferenceName };
            PublicationChannel channel = publisher == null && serie == null && conference == null
                ? null
                : new PublicationChannel { IsPublishedBy = publisher, IsPublishedInSerie = serie, RelatesToConference = conference };

            ParentPublication parentPublication = null;
            if (!string.IsNullOrWhiteSpace(publication.ParentPublicationName))
            {
                parentPublication = new ParentPublication
                {
                    ParentPublicationName = publication.ParentPublicationName,
                    PublishedOn = string.IsNullOrWhiteSpace(publication.ParentPublicationPublisher)
                        ? null
                        : new PublicationChannel { IsPublishedBy = new Publisher { PublisherName = publication.ParentPublicationPublisher } }
                };
            }

            return new WrittenPublication
            {
                PublicationYear = publication.PublicationYear,
                PublishedOn = channel,
                WrittenPublicationIdentifier = identifiers,
                WrittenPublicationIsPartOfParentPublication = parentPublication,
                PublicationName = publication.PublicationName,
                PublicationClassification = !string.IsNullOrWhiteSpace(publication.PublicationTypeCode)
                    && publicationClassifications.TryGetValue(publication.PublicationTypeCode, out ReferenceData classification)
                        ? classification
                        : null,
                ListOfAuthors = publication.AuthorsText
            };
        }

        private static ResearchDataset ToResearchDataset(ProfileEditorResearchDataset dataset)
        {
            return new ResearchDataset
            {
                DatasetTitle = ToDescriptiveItems(dataset.NameFi, dataset.NameSv, dataset.NameEn),
                DatasetDescription = ToDescriptiveItems(dataset.DescriptionFi, dataset.DescriptionSv, dataset.DescriptionEn),
                DatasetCreatedDate = ToPartialDate(dataset.DatasetCreated),
                RoleInDataset = dataset.Actor.Select(a => new ReferenceData
                {
                    CodeValue = a.actorRole.ToString(CultureInfo.InvariantCulture),
                    CodeLabel = ToLanguageVariant(a.actorRoleNameFi, a.actorRoleNameSv, a.actorRoleNameEn)
                }).ToList(),
                DatasetIdentifier = dataset.PreferredIdentifiers.Select(p => new Identifier { PidContent = p.PidContent, PidType = p.PidType }).ToList()
            };
        }

        public string GetUsernameFromNationalIdentificationNumber(string nationalIdentificationNumber)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(nationalIdentificationNumber.ToUpper());
            byte[] hashBytes = MD5.HashData(inputBytes);
            StringBuilder sb = new StringBuilder(2 * hashBytes.Length);
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
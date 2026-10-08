using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using api.Models.Ttv;
using api.Models.Keycloak;
using api.Models.Log;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using ResearchFi.PersonPublicApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace api.Services
{
    public class PublicApiService : IPublicApiService
    {
        private readonly TtvContext _ttvContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PublicApiService> _logger;
        private readonly IUserProfileService _userProfileService;


        public PublicApiService(
            TtvContext ttvContext,
            IHttpClientFactory httpClientFactory,
            ILogger<PublicApiService> logger,
            IUserProfileService userProfileService)
        {
            _ttvContext = ttvContext;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _userProfileService = userProfileService;
        }

        // Get profile data for public API. Only published (show=true) items are included.
        public async Task<ProfileDataResponse> GetProfileDataForPublicApi(string personKeyIdentifier)
        {
            // Profiles created before username was introduced have an empty username, never match those.
            DimUserProfile dimUserProfile = string.IsNullOrWhiteSpace(personKeyIdentifier)
                ? null
                : await _userProfileService.GetUserprofileByUsername(GetUsernameFromNationalIdentificationNumber(personKeyIdentifier));

            if (dimUserProfile == null || dimUserProfile.Hidden)
            {
                return new ProfileDataResponse { PersonProfileRecognized = false, PersonProfileData = null };
            }

            ProfileEditorDataResponse profileData = await _userProfileService.GetProfileData(
                userprofileId: dimUserProfile.Id,
                logUserIdentification: new LogUserIdentification(orcid: dimUserProfile.OrcidId),
                forElasticsearch: true);

            return new ProfileDataResponse
            {
                PersonProfileRecognized = true,
                PersonProfileData = new PersonProfileData
                {
                    HasGrantedFunding = profileData.activity.fundingDecisions.Select(ToGrantedFunding).ToList(),
                    DescriptionOfActivities = ToDescriptionOfActivities(profileData.personal.researcherDescriptions),
                    IsNamedBy = ToPersonName(profileData.personal.names.FirstOrDefault()),
                    IsReachableFrom = ToContactInformation(profileData.personal),
                    ContributesToOutput = new List<Output>
                    {
                        new Output
                        {
                            ContributesToWrittenPublication = profileData.activity.publications.Select(ToWrittenPublication).ToList(),
                            ContributesToDataset = profileData.activity.researchDatasets.Select(ToResearchDataset).ToList(),
                            // TODO: not handled yet. Profile data has no source for these output types.
                            ContributesToApplication = new List<Software>(),
                            ContributesToAudiovisualPublication = new List<AudiovisualPublication>(),
                            ContributesToArtpublication = new List<ArtPublication>()
                        }
                    },
                    Affiliations = profileData.activity.affiliations.Select(ToAffiliation).ToList(),
                    PersonIsIdentifiedByORCID = dimUserProfile.OrcidId,
                    CollaborationInterests = await GetCollaborationInterests(dimUserProfile.Id),
                    HasEducations = profileData.activity.educations.Select(ToEducation).ToList()
                },
                // TODO: not handled yet. Consent and funder recognition are not checked.
                ConsentGranted = true,
                RecognizedFunder = true
            };
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
                .Where(c => int.TryParse(c.CodeValue, out _))
                .Select(c => new ReferenceData
                {
                    CodeValue = int.Parse(c.CodeValue),
                    CodeLabel = ToLanguageVariant(c.NameFi, c.NameSv, c.NameEn)
                }).ToList();
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
            // TODO: not handled yet: PartialDate.DatePartial.
            return new PartialDate
            {
                Year = date.Year,
                Month = date.Month > 0 ? date.Month : null,
                Day = date.Day > 0 ? date.Day : null
            };
        }

        private static PartialDate ToPartialDate(int? year)
        {
            return year > 0 ? new PartialDate { Year = year.Value } : null;
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

        // TODO: not handled yet: phone numbers and emails are untyped objects in the contract, plain strings are used for now.
        private static ContactInformation ToContactInformation(ProfileEditorDataPersonal personal)
        {
            return new ContactInformation
            {
                PhoneNumber = personal.telephoneNumbers.Select(t => (object)t.Value).ToList(),
                Email = personal.emails.Select(e => (object)e.Value).ToList(),
                Homepage = personal.webLinks.Select(w => new WebLink { LinkLabel = w.LinkLabel, LinkURL = w.Url }).ToList()
            };
        }

        // TODO: not handled yet: Organization identifiers (OrganizationIsIdentifiedBy) of the funder.
        private static GrantedFunding ToGrantedFunding(ProfileEditorFundingDecision funding)
        {
            return new GrantedFunding
            {
                FundingStartsOn = ToPartialDate(funding.FundingStartYear),
                FundingEndsOn = ToPartialDate(funding.FundingEndYear),
                IsFundedBy = new Organization
                {
                    OrganizationIsIdentifiedBy = new List<Identifier>(),
                    OrganizationName = ToLanguageVariant(funding.FunderNameFi, funding.FunderNameSv, funding.FunderNameEn)
                },
                GrantedFundingName = ToLanguageVariant(funding.ProjectNameFi, funding.ProjectNameSv, funding.ProjectNameEn)
            };
        }

        // TODO: not handled yet: department name, organization identifiers, parent organization and organization sector.
        private static Affiliation ToAffiliation(ProfileEditorAffiliation affiliation)
        {
            return new Affiliation
            {
                AffiliationStartsOn = ToPartialDate(affiliation.StartDate),
                AffiliationEndsOn = ToPartialDate(affiliation.EndDate),
                AffiliationType = ToLanguageVariant(affiliation.AffiliationTypeFi, affiliation.AffiliationTypeSv, affiliation.AffiliationTypeEn),
                AffiliationOrganization = new Organization
                {
                    OrganizationIsIdentifiedBy = new List<Identifier>(),
                    OrganizationName = ToLanguageVariant(affiliation.OrganizationNameFi, affiliation.OrganizationNameSv, affiliation.OrganizationNameEn)
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

        // TODO: not handled yet: identifiers other than DOI, JuFo ids, ISSN, publication series, parent publication issue number.
        private static WrittenPublication ToWrittenPublication(ProfileEditorPublication publication)
        {
            List<Identifier> identifiers = new();
            if (!string.IsNullOrWhiteSpace(publication.Doi))
            {
                identifiers.Add(new Identifier { PidContent = publication.Doi, PidType = "DOI" });
            }

            Publisher publisher = string.IsNullOrWhiteSpace(publication.PublisherName) ? null : new Publisher { PublisherName = publication.PublisherName };
            Journal journal = string.IsNullOrWhiteSpace(publication.JournalName) ? null : new Journal { JournalName = publication.JournalName, Volume = NullIfBlank(publication.Volume) };
            ConferenceEvent conference = string.IsNullOrWhiteSpace(publication.ConferenceName) ? null : new ConferenceEvent { ConferenceName = publication.ConferenceName };
            PublicationChannel channel = publisher == null && journal == null && conference == null
                ? null
                : new PublicationChannel { IsPublishedBy = publisher, PublishedInJournal = journal, RelatesToConference = conference };

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
                WrittenPublicationIsIdentifiedBy = identifiers,
                WrittenPublicationIsPartOfParentPublication = parentPublication,
                PublicationName = publication.PublicationName,
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
                    CodeValue = a.actorRole,
                    CodeLabel = ToLanguageVariant(a.actorRoleNameFi, a.actorRoleNameSv, a.actorRoleNameEn)
                }).ToList(),
                DatasetIsIdentifiedBy = dataset.PreferredIdentifiers.Select(p => new Identifier { PidContent = p.PidContent, PidType = p.PidType }).ToList()
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
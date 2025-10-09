using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using api.Models.Ai;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Text.RegularExpressions;

namespace api.Services
{
    public class AiPocService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILogger<UserProfileService> _logger;


        public AiPocService(
            TtvContext ttvContext,
            ILogger<UserProfileService> logger)
        {
            _ttvContext = ttvContext;
            _logger = logger;
        }

        public async Task<AittaModel?> GetProfileDataForPromt(string orcidId)
        {
            List<FactFieldValue> factFieldValues = await _ttvContext.FactFieldValues
                // DimUserProfile
                .Include(ffv => ffv.DimUserProfile)
                    .Where(ffv => ffv.DimUserProfile != null && ffv.DimUserProfile.OrcidId == orcidId && ffv.Show == true)
                // DimName
                .Include(ffv => ffv.DimName)
                // DimResearcherDescription
                .Include(ffv => ffv.DimResearcherDescription)
                // DimAffiliation
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.DimOrganization)
                        .ThenInclude(aff_org => aff_org.DimOrganizationBroaderNavigation)
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.StartDateNavigation)
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.EndDateNavigation)
                .Include(ffv => ffv.DimEducation)
                /*
                // DimPublication
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.DimKeywords)
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.DimDescriptiveItems)
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.FactDimReferencedataFieldOfSciences)
                        .ThenInclude(fdrfs => fdrfs.DimReferencedata)
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.PublicationTypeCodeNavigation)
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.TargetAudienceCodeNavigation)
                // DimProfileOnlyPublication
                .Include(ffv => ffv.DimProfileOnlyPublication)
                // DimResearchDataset
                .Include(ffv => ffv.DimResearchDataset)
                    .ThenInclude(rd => rd.DimKeywords)
                .Include(ffv => ffv.DimResearchDataset)
                    .ThenInclude(rd => rd.FactDimReferencedataFieldOfSciences)
                        .ThenInclude(fdrfs => fdrfs.DimReferencedata)
                // DimFundingDecision
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimDateIdStartNavigation)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimDateIdEndNavigation)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimOrganizationIdFunderNavigation)
                        .ThenInclude(org => org.DimOrganizationBroaderNavigation)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimKeywords)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.FactDimReferencedataFieldOfSciences)
                        .ThenInclude(fdrfs => fdrfs.DimReferencedata)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimTypeOfFunding)
                // DimProfileOnlyFundingDecision
                .Include(ffv => ffv.DimProfileOnlyFundingDecision)
                    .ThenInclude(pofd => pofd.DimDateIdStartNavigation)
                .Include(ffv => ffv.DimProfileOnlyFundingDecision)
                    .ThenInclude(pofd => pofd.DimDateIdEndNavigation)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(pofd => pofd.DimOrganizationIdFunderNavigation)
                        .ThenInclude(org => org.DimOrganizationBroaderNavigation)
                .Include(ffv => ffv.DimProfileOnlyFundingDecision)
                    .ThenInclude(pofd => pofd.DimTypeOfFunding)
                // DimResearchActivity
                .Include(ffv => ffv.DimResearchActivity)
                    .ThenInclude(ra => ra.DimStartDateNavigation)
                .Include(ffv => ffv.DimResearchActivity)
                    .ThenInclude(ra => ra.DimEndDateNavigation)
                // DimProfileOnlyResearchActivity
                .Include(ffv => ffv.DimProfileOnlyResearchActivity)
                    .ThenInclude(pora => pora.DimDateIdStartNavigation)
                .Include(ffv => ffv.DimProfileOnlyResearchActivity)
                    .ThenInclude(pora => pora.DimDateIdEndNavigation)
                // DimreferencedataActorRole (for DimProfileOnlyResearchActivity)
                .Include(ffv => ffv.DimReferencedataActorRole)
*/
                // Query execution
                .AsNoTracking().AsSplitQuery().ToListAsync();

            AittaModel aittaModel = new AittaModel();

            if (factFieldValues.Count == 0)
            {
                _logger.LogWarning("No data found for ORCID {orcidId}", orcidId);
                return null;
            }

            // DimName
            foreach (DimName n in factFieldValues.Where(ffv => ffv.DimName != null && ffv.DimNameId > 0 && (!string.IsNullOrWhiteSpace(ffv.DimName.FirstNames) || !string.IsNullOrWhiteSpace(ffv.DimName.LastName))).Select(ffv => ffv.DimName).ToList())
            {
                aittaModel.PersonName = $"{n.FirstNames} {n.LastName}";
            }

            // DimResearcherDescription
            foreach (DimResearcherDescription rd in factFieldValues.Where(ffv => ffv.DimResearcherDescription != null && ffv.DimResearcherDescriptionId > 0).Select(ffv => ffv.DimResearcherDescription).ToList())
            {
                aittaModel.ResearcherDescription.Add(rd.ResearchDescriptionEn);
            }

            // DimAffiliation
            foreach (DimAffiliation a in factFieldValues.Where(ffv => ffv.DimAffiliation != null && ffv.DimAffiliationId > 0).Select(ffv => ffv.DimAffiliation).ToList())
            {
                aittaModel.HasAffiliation.Add(new AittaAffiliation
                {
                    AffiliationType = LanguageFilter(en: a.AffiliationTypeEn, fi: a.AffiliationTypeFi, sv: a.AffiliationTypeSv),
                    PositionTitle = LanguageFilter(en: a.PositionNameEn, fi: a.PositionNameFi, sv: a.PositionNameSv),
                    Organization = new AittaOrganization
                    {
                        OrganizationName = LanguageFilter(en: a.DimOrganization.NameEn, fi: a.DimOrganization.NameFi, sv: a.DimOrganization.NameSv),
                        IsPartOfOrganization = a.DimOrganization.DimOrganizationBroader != null ?
                            new AittaOrganization
                            {
                                OrganizationName = LanguageFilter(
                                    en: a.DimOrganization.DimOrganizationBroaderNavigation.NameEn,
                                    fi: a.DimOrganization.DimOrganizationBroaderNavigation.NameFi,
                                    sv: a.DimOrganization.DimOrganizationBroaderNavigation.NameSv
                                )
                            }
                            : null
                    },
                    StartsOn = a.StartDateNavigation != null && a.StartDate > 0 ?
                        new AittaDate { Year = a.StartDateNavigation.Year, Month = a.StartDateNavigation.Month } : null,
                    EndsOn = a.EndDateNavigation != null && a.EndDate > 0 ?
                        new AittaDate { Year = a.EndDateNavigation.Year, Month = a.EndDateNavigation.Month } : null
                });

            }

            // DimEducation
            foreach (DimEducation e in factFieldValues.Where(ffv => ffv.DimEducation != null && ffv.DimEducationId > 0).Select(ffv => ffv.DimEducation).ToList())
            {
                aittaModel.HasCompleted.Add(new AittaEducation
                {
                    EducationName = LanguageFilter(en: e.NameEn, fi: e.NameFi, sv: e.NameSv),
                    DegreeGrantingInstitution = !string.IsNullOrWhiteSpace(e.DegreeGrantingInstitutionName) ? e.DegreeGrantingInstitutionName : null
                });
            }

            // DimPublication
            foreach (DimPublication p in factFieldValues.Where(ffv => ffv.DimPublication != null && ffv.DimPublicationId > 0).Select(ffv => ffv.DimPublication).ToList())
            {
                string? publicationAbstract = p.DimDescriptiveItems.Where(di => di.DescriptiveItemType == "Abstract").Select(di => di.DescriptiveItem).FirstOrDefault();
                if (publicationAbstract != null)
                {
                    // Take only part of the abstract to keep the prompt size reasonable
                    publicationAbstract = GetFirstNSentences(publicationAbstract, 1);
                }

                aittaModel.UserParticipatedPublication.Add(new AittaPublication
                {
                    Name = !string.IsNullOrWhiteSpace(p.PublicationName) ? p.PublicationName : null,
                    Year = p.PublicationYear > 0 ? p.PublicationYear : null,
                    Abstract = publicationAbstract != null ? publicationAbstract : null,
                    Keywords = p.DimKeywords.Count > 0 ? p.DimKeywords.Select(kw => kw.Keyword).ToList() : null,
                    FieldsOfScience = p.FactDimReferencedataFieldOfSciences.Select(fdrfs => fdrfs.DimReferencedata.NameEn).ToList(),
                    Type = p.PublicationTypeCodeNavigation != null ? p.PublicationTypeCodeNavigation.NameEn : null,
                    TargetAudience = p.TargetAudienceCodeNavigation != null ? p.TargetAudienceCodeNavigation.NameEn : null
                });
            }

            // DimProfileOnlyPublication
            foreach (DimProfileOnlyPublication p in factFieldValues.Where(ffv => ffv.DimProfileOnlyPublication != null && ffv.DimProfileOnlyPublicationId > 0).Select(ffv => ffv.DimProfileOnlyPublication).ToList())
            {
                aittaModel.UserParticipatedPublication.Add(new AittaPublication
                {
                    Name = !string.IsNullOrWhiteSpace(p.PublicationName) ? p.PublicationName : null,
                    Year = p.PublicationYear > 0 ? p.PublicationYear : null,
                    Abstract = null,
                    Keywords = null,
                    FieldsOfScience = null,
                    Type = null,
                    TargetAudience = null
                });
            }

            // DimResearchDataset
            foreach (DimResearchDataset rd in factFieldValues.Where(ffv => ffv.DimResearchDataset != null && ffv.DimResearchDatasetId > 0).Select(ffv => ffv.DimResearchDataset).ToList())
            {
                aittaModel.UserParticipatedDataset.Add(new AittaResearchDataset
                {
                    DatasetTitle = LanguageFilter(en: rd.NameEn, fi: rd.NameFi, sv: rd.NameSv),
                    Description = GetFirstNSentences(
                        LanguageFilter(en: rd.DescriptionEn, fi: rd.DescriptionFi, sv: rd.DescriptionSv),
                        1
                    ),
                    DatasetCreationDate = rd.DatasetCreated != null ? rd.DatasetCreated : null,
                    Theme = rd.DimKeywords.Count > 0 ? rd.DimKeywords.Where(kw => kw.Scheme == "Theme").Select(kw => kw.Keyword).ToList() : null,
                    Keywords = rd.DimKeywords.Count > 0 ? rd.DimKeywords.Where(kw => kw.Scheme == "Avainsana").Select(kw => kw.Keyword).ToList() : null,
                    FieldsOfScience = rd.FactDimReferencedataFieldOfSciences.Select(fdrfs => fdrfs.DimReferencedata.NameEn).ToList()
                });
            }

            // DimProfileOnlyResearchDataset
            // TODO

            // DimFundingDecision
            foreach (DimFundingDecision fd in factFieldValues.Where(ffv => ffv.DimFundingDecision != null && ffv.DimFundingDecisionId > 0).Select(ffv => ffv.DimFundingDecision).ToList())
            {
                aittaModel.UserParticipatedGrantedFunding.Add(new AittaGrantedFunding
                {
                    Name = LanguageFilter(en: fd.NameEn, fi: fd.NameFi, sv: fd.NameSv),
                    Description = GetFirstNSentences(
                        LanguageFilter(en: fd.DescriptionEn, fi: fd.DescriptionFi, sv: fd.DescriptionSv),
                        1
                    ),
                    StartsOn = fd.DimDateIdStartNavigation != null && fd.DimDateIdStart > 0 ?
                        new AittaDate { Year = fd.DimDateIdStartNavigation.Year, Month = fd.DimDateIdStartNavigation.Month } : null,
                    EndsOn = fd.DimDateIdEndNavigation != null && fd.DimDateIdEnd > 0 ?
                        new AittaDate { Year = fd.DimDateIdEndNavigation.Year, Month = fd.DimDateIdEndNavigation.Month } : null,
                    HasFunder = fd.DimOrganizationIdFunderNavigation != null ? new AittaOrganization
                    {
                        OrganizationName = LanguageFilter(
                            en: fd.DimOrganizationIdFunderNavigation.NameEn, fi: fd.DimOrganizationIdFunderNavigation.NameFi, sv: fd.DimOrganizationIdFunderNavigation.NameSv
                        ),
                        IsPartOfOrganization = fd.DimOrganizationIdFunderNavigation.DimOrganizationBroader != null ?
                            new AittaOrganization
                            {
                                OrganizationName = LanguageFilter(
                                    en: fd.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameEn,
                                    fi: fd.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameFi,
                                    sv: fd.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameSv
                                )
                            }
                            : null
                    } : null,
                    Keywords = fd.DimKeywords.Count > 0 ? fd.DimKeywords.Where(kw => kw.Scheme == "Avainsana").Select(kw => kw.Keyword).ToList() : null,
                    FieldsOfScience = fd.FactDimReferencedataFieldOfSciences.Select(fdrfs => fdrfs.DimReferencedata.NameEn).ToList(),
                    TypeOfFunding = fd.DimTypeOfFunding != null && fd.DimTypeOfFundingId > 0 ? fd.DimTypeOfFunding.NameEn : null,
                    FieldsOfResearch = fd.DimKeywords.Count > 0 ? fd.DimKeywords.Where(kw => kw.Scheme == "Tutkimusala").Select(kw => kw.Keyword).ToList() : null,
                    Theme = fd.DimKeywords.Count > 0 ? fd.DimKeywords.Where(kw => kw.Scheme == "Teema-ala").Select(kw => kw.Keyword).ToList() : null,
                });
            }

            // DimProfileOnlyFundingDecision
            foreach (DimProfileOnlyFundingDecision pofd in factFieldValues.Where(ffv => ffv.DimProfileOnlyFundingDecision != null && ffv.DimProfileOnlyFundingDecisionId > 0).Select(ffv => ffv.DimProfileOnlyFundingDecision).ToList())
            {
                aittaModel.UserParticipatedGrantedFunding.Add(new AittaGrantedFunding
                {
                    Name = LanguageFilter(pofd.NameEn, pofd.NameFi, pofd.NameSv),
                    Description = GetFirstNSentences(
                        LanguageFilter(pofd.DescriptionEn, pofd.DescriptionFi, pofd.DescriptionSv),
                        1
                    ),
                    StartsOn = pofd.DimDateIdStartNavigation != null && pofd.DimDateIdStart > 0 ?
                        new AittaDate { Year = pofd.DimDateIdStartNavigation.Year, Month = pofd.DimDateIdStartNavigation.Month } : null,
                    EndsOn = pofd.DimDateIdEndNavigation != null && pofd.DimDateIdEnd > 0 ?
                        new AittaDate { Year = pofd.DimDateIdEndNavigation.Year, Month = pofd.DimDateIdEndNavigation.Month } : null,
                    HasFunder = pofd.DimOrganizationIdFunderNavigation != null ? new AittaOrganization
                    {
                        OrganizationName = LanguageFilter(
                            en: pofd.DimOrganizationIdFunderNavigation.NameEn,
                            fi: pofd.DimOrganizationIdFunderNavigation.NameFi,
                            sv: pofd.DimOrganizationIdFunderNavigation.NameSv
                        ),
                        IsPartOfOrganization = pofd.DimOrganizationIdFunderNavigation.DimOrganizationBroader != null ?
                            new AittaOrganization {
                                OrganizationName = LanguageFilter(
                                    en: pofd.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameEn,
                                    fi: pofd.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameFi,
                                    sv: pofd.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameSv
                                )
                            }
                            : null
                    } : null,
                    Keywords = null,
                    FieldsOfScience = null,
                    TypeOfFunding = pofd.DimTypeOfFunding != null && pofd.DimTypeOfFundingId > 0 ?
                        LanguageFilter(en: pofd.DimTypeOfFunding.NameEn, fi: pofd.DimTypeOfFunding.NameFi, sv: pofd.DimTypeOfFunding.NameSv)
                        : null,
                    FieldsOfResearch = null,
                    Theme = null
                });
            }

            // DimResearchActivity
            foreach (DimResearchActivity ra in factFieldValues.Where(ffv => ffv.DimResearchActivity != null && ffv.DimResearchActivityId > 0).Select(ffv => ffv.DimResearchActivity).ToList())
            {
                aittaModel.UserParticipatedActivity.Add(new AittaResearchActivity
                {
                    ActivityTitle = LanguageFilter(en: ra.NameEn, fi: ra.NameFi, sv: ra.NameSv),
                    Description = LanguageFilter(en: ra.DescriptionEn, fi: ra.DescriptionFi, sv: ra.DescriptionSv),
                    StartsOn = ra.DimStartDateNavigation != null && ra.DimStartDate > 0 ?
                        new AittaDate { Year = ra.DimStartDateNavigation.Year, Month = ra.DimStartDateNavigation.Month } : null,
                    EndsOn = ra.DimEndDateNavigation != null && ra.DimEndDate > 0 ?
                        new AittaDate { Year = ra.DimEndDateNavigation.Year, Month = ra.DimEndDateNavigation.Month } : null,
                    Role = null,
                    Type = null
                });
            }

            // DimProfileOnlyResearchActivity
            foreach (DimProfileOnlyResearchActivity pora in factFieldValues.Where(ffv => ffv.DimProfileOnlyResearchActivity != null && ffv.DimProfileOnlyResearchActivityId > 0).Select(ffv => ffv.DimProfileOnlyResearchActivity).ToList())
            {
                aittaModel.UserParticipatedActivity.Add(new AittaResearchActivity
                {
                    ActivityTitle = LanguageFilter(en: pora.NameEn, fi: pora.NameFi, sv: pora.NameSv),
                    Description = LanguageFilter(en: pora.DescriptionEn, fi: pora.DescriptionFi, sv: pora.DescriptionSv),
                    StartsOn = pora.DimDateIdStartNavigation != null && pora.DimDateIdStart > 0 ?
                        new AittaDate { Year = pora.DimDateIdStartNavigation.Year, Month = pora.DimDateIdStartNavigation.Month } : null,
                    EndsOn = pora.DimDateIdEndNavigation != null && pora.DimDateIdEnd > 0 ?
                        new AittaDate { Year = pora.DimDateIdEndNavigation.Year, Month = pora.DimDateIdEndNavigation.Month } : null,
                    Role = pora.FactFieldValues.FirstOrDefault().DimReferencedataActorRole != null && pora.FactFieldValues.FirstOrDefault().DimReferencedataActorRoleId > 0 ?
                        LanguageFilter(
                            en: pora.FactFieldValues.FirstOrDefault().DimReferencedataActorRole.NameEn,
                            fi: pora.FactFieldValues.FirstOrDefault().DimReferencedataActorRole.NameFi,
                            sv: pora.FactFieldValues.FirstOrDefault().DimReferencedataActorRole.NameSv
                        )
                        : null,
                    Type = null
                });
            }

            return aittaModel;
        }

        /// <summary>
        /// Returns the first *n* sentences (ending with '.') from <paramref name="input"/>.
        /// Sentences that end with other punctuation (.!? etc.) are ignored.
        /// </summary>
        static string? GetFirstNSentences(string input, int n)
        {
            if (string.IsNullOrWhiteSpace(input) || n <= 0)
                return null;

            // 1. Split the text on periods, but keep the delimiter with each piece.
            //    The regex `(?<=[.])` is a zero‑width positive lookbehind that matches
            //    the position immediately after every period.
            var parts = Regex.Split(input, @"(?<=[.])");

            // 2. Trim whitespace and discard empty fragments (e.g., trailing newlines).
            var sentences = parts
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .Take(n);

            // 3. Join them back together with a single space (or any separator you prefer).
            return string.Join(" ", sentences);
        }
        
        static string? LanguageFilter(string? en, string? fi, string? sv)
        {
            if (!string.IsNullOrWhiteSpace(en))
            {
                return en;
            }
            else if (!string.IsNullOrWhiteSpace(fi))
            {
                return fi;
            }
            else if (!string.IsNullOrWhiteSpace(sv))
            {
                return sv;
            }
            else
            {
                return null;
            }
        }
    }
}
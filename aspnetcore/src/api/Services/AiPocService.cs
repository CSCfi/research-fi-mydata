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
                .Include(ffv => ffv.DimName)
                .Include(ffv => ffv.DimResearcherDescription)
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.DimOrganization)
                        .ThenInclude(aff_org => aff_org.DimOrganizationBroaderNavigation)
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.StartDateNavigation)
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.EndDateNavigation)
                .Include(ffv => ffv.DimEducation)
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
                .Include(ffv => ffv.DimResearchDataset)
                    .ThenInclude(rd => rd.DimKeywords)
                .Where(ffv => ffv.DimUserProfile != null && ffv.DimUserProfile.OrcidId == orcidId && ffv.Show == true)
                .AsNoTracking().AsSplitQuery().ToListAsync();

            AittaModel aittaModel = new AittaModel();

            if (factFieldValues.Count == 0)
            {
                _logger.LogWarning("No data found for ORCID {orcidId}", orcidId);
                return null;
            }

            // Names
            foreach (DimName n in factFieldValues.Where(ffv => ffv.DimNameId > 0 && (!string.IsNullOrWhiteSpace(ffv.DimName.FirstNames) || !string.IsNullOrWhiteSpace(ffv.DimName.LastName))).Select(ffv => ffv.DimName).ToList())
            {
                aittaModel.PersonName = $"{n.FirstNames} {n.LastName}";
            }

            // Researcher descriptions
            foreach (DimResearcherDescription rd in factFieldValues.Where(ffv => ffv.DimResearcherDescriptionId > 0).Select(ffv => ffv.DimResearcherDescription).ToList())
            {
                aittaModel.ResearcherDescription.Add(rd.ResearchDescriptionEn);
            }

            // Affiliations
            foreach (DimAffiliation a in factFieldValues.Where(ffv => ffv.DimAffiliationId > 0).Select(ffv => ffv.DimAffiliation).ToList())
            {
                aittaModel.HasAffiliation.Add(new AittaAffiliation
                {
                    AffiliationType = !string.IsNullOrWhiteSpace(a.AffiliationTypeEn) ? a.AffiliationTypeEn : null,
                    PositionTitle = !string.IsNullOrWhiteSpace(a.PositionNameEn) ? a.PositionNameEn : null,
                    Organization = new AittaOrganization
                    {
                        OrganizationName = a.DimOrganization.NameEn,
                        IsPartOfOrganization = a.DimOrganization.DimOrganizationBroader != null ? new AittaOrganization { OrganizationName = a.DimOrganization.DimOrganizationBroaderNavigation.NameEn } : null
                    },
                    StartsOn = a.StartDateNavigation != null ? new AittaDate { Year = a.StartDateNavigation.Year, Month = a.StartDateNavigation.Month } : null,
                    EndsOn = a.EndDateNavigation != null ? new AittaDate { Year = a.EndDateNavigation.Year, Month = a.EndDateNavigation.Month } : null
                });

            }

            // Educations
            foreach (DimEducation e in factFieldValues.Where(ffv => ffv.DimEducationId > 0).Select(ffv => ffv.DimEducation).ToList())
            {
                aittaModel.HasCompleted.Add(new AittaEducation
                {
                    EducationName = !string.IsNullOrWhiteSpace(e.NameEn) ? e.NameEn : null,
                    DegreeGrantingInstitution = !string.IsNullOrWhiteSpace(e.DegreeGrantingInstitutionName) ? e.DegreeGrantingInstitutionName : null
                });
            }

            // TTV publications
            foreach (DimPublication p in factFieldValues.Where(ffv => ffv.DimPublicationId > 0).Select(ffv => ffv.DimPublication).ToList())
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

            // ORCID publications
            foreach (DimProfileOnlyPublication p in factFieldValues.Where(ffv => ffv.DimProfileOnlyPublicationId > 0).Select(ffv => ffv.DimProfileOnlyPublication).ToList())
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

            // Datasets
            foreach (DimResearchDataset rd in factFieldValues.Where(ffv => ffv.DimResearchDatasetId > 0).Select(ffv => ffv.DimResearchDataset).ToList())
            {
                aittaModel.UserParticipatedDataset.Add(new AittaResearchDataset
                {
                    DatasetTitle = !string.IsNullOrWhiteSpace(rd.NameEn) ? rd.NameEn : null,
                    DatasetDescription = !string.IsNullOrWhiteSpace(rd.DescriptionEn) ? rd.DescriptionEn : null,
                    DatasetCreationDate = rd.DatasetCreated != null ? rd.DatasetCreated : null,
                    Theme = null,
                    Keywords = rd.DimKeywords.Count > 0 ? rd.DimKeywords.Select(kw => kw.Keyword).ToList() : null,
                    FieldsOfScience = null
                });
            }

            return aittaModel;
        }
        
        /// <summary>
        /// Returns the first *n* sentences (ending with '.') from <paramref name="input"/>.
        /// Sentences that end with other punctuation (.!? etc.) are ignored.
        /// </summary>
        static string GetFirstNSentences(string input, int n)
        {
            if (string.IsNullOrWhiteSpace(input) || n <= 0)
                return string.Empty;

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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using api.Models.Ai;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

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
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.DimDescriptiveItems)
                .Include(ffv => ffv.DimProfileOnlyPublication)
                .Include(ffv => ffv.DimResearchDataset)
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
                    publicationAbstract = $"{publicationAbstract.Substring(0, Math.Min(200, publicationAbstract.Length))}";
                }

                aittaModel.UserParticipatedPublication.Add(new AittaPublication
                {
                    PublicationName = !string.IsNullOrWhiteSpace(p.PublicationName) ? p.PublicationName : null,
                    PublicationYear = p.PublicationYear > 0 ? p.PublicationYear : null,
                    Abstract = publicationAbstract != null ? new AittaDescriptiveItem { DescriptiveContent = publicationAbstract } : null,
                    Avainsana = p.DimKeywords.Select(kw => kw.Keyword).ToList(),
                    Tieteenala2010 = p.FactDimReferencedataFieldOfSciences.Select(fdrfs => new AittaReferenceData { CodeName = fdrfs.DimReferencedata.CodeScheme, CodeValue = fdrfs.DimReferencedata.CodeValue }).ToList(),
                    Julkaisutyyppiluokitus = p.PublicationTypeCodeNavigation != null ? new AittaReferenceData { CodeName = p.PublicationTypeCodeNavigation.CodeScheme, CodeValue = p.PublicationTypeCodeNavigation.CodeValue } : null,
                    Julkaisunyleiso = p.TargetAudienceCodeNavigation != null ? new AittaReferenceData { CodeName = p.TargetAudienceCodeNavigation.CodeScheme, CodeValue = p.TargetAudienceCodeNavigation.CodeValue } : null
                });
            }

            // ORCID publications
            foreach (DimProfileOnlyPublication p in factFieldValues.Where(ffv => ffv.DimProfileOnlyPublicationId > 0).Select(ffv => ffv.DimProfileOnlyPublication).ToList())
            {
                aittaModel.UserParticipatedPublication.Add(new AittaPublication
                {
                    PublicationName = !string.IsNullOrWhiteSpace(p.PublicationName) ? p.PublicationName : null,
                    PublicationYear = p.PublicationYear > 0 ? p.PublicationYear : null,
                    Abstract = null,
                    Avainsana = null,
                    Tieteenala2010 = null,
                    Julkaisutyyppiluokitus = null,
                    Julkaisunyleiso = null
                });
            }

            // Activities
            foreach (DimResearchDataset rd in factFieldValues.Where(ffv => ffv.DimResearchDatasetId > 0).Select(ffv => ffv.DimResearchDataset).ToList())
            {
                aittaModel.UserParticipatedDataset.Add(new AittaResearchDataset
                {
                    DatasetTitle = !string.IsNullOrWhiteSpace(rd.NameEn) ? new AittaDescriptiveItem { DescriptiveContent = rd.NameEn } : null,
                    DatasetDescription = !string.IsNullOrWhiteSpace(rd.DescriptionEn) ? new AittaDescriptiveItem { DescriptiveContent = rd.DescriptionEn } : null,
                    DatasetCreationDate = rd.DatasetCreated != null ? rd.DatasetCreated : null,
                    Theme = null,
                    Avainsana = null,
                    Tieteenala2010 = null
                });
            }

            return aittaModel;
        }
    }
}
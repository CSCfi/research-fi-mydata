using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using api.Models.Aitta;
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



        public async Task<AittaProfileData?> GetProfileDataForPromt(string orcidId)
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
                .Where(ffv => ffv.DimUserProfile != null && ffv.DimUserProfile.OrcidId == orcidId && ffv.Show == true)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            AittaProfileData profileData = new AittaProfileData();

            if (factFieldValues.Count == 0)
            {
                _logger.LogWarning("No data found for ORCID {orcidId}", orcidId);
                return null;
            }

            // Names
            foreach (DimName n in factFieldValues.Where(ffv => ffv.DimNameId > 0 && (!string.IsNullOrWhiteSpace(ffv.DimName.FirstNames) || !string.IsNullOrWhiteSpace(ffv.DimName.LastName))).Select(ffv => ffv.DimName).ToList())
            {
                profileData.Name.Add($"{n.FirstNames} {n.LastName}");
            }

            // Researcher descriptions
            foreach (DimResearcherDescription rd in factFieldValues.Where(ffv => ffv.DimResearcherDescriptionId > 0 ).Select(ffv => ffv.DimResearcherDescription).ToList())
            {
                profileData.ResearcherDescription.Add(rd.ResearchDescriptionEn);
            }

            // Affiliations
            foreach (DimAffiliation a in factFieldValues.Where(ffv => ffv.DimAffiliationId > 0).Select(ffv => ffv.DimAffiliation).ToList())
            {
                string organizationName = "";
                if (a.DimOrganization.DimOrganizationBroader != null)
                {
                    organizationName = a.DimOrganization.DimOrganizationBroaderNavigation.NameEn;
                }
                else
                {
                    organizationName = a.DimOrganization.NameEn;
                }
                profileData.Affiliations.Add(new Affiliation
                {
                    Position = a.PositionNameEn,
                    Organization = organizationName,
                    StartDate = a.StartDateNavigation != null ? new ProfileDataDate { Year = a.StartDateNavigation.Year, Month = a.StartDateNavigation.Month } : null,
                    EndDate = a.EndDateNavigation != null ? new ProfileDataDate { Year = a.EndDateNavigation.Year, Month = a.EndDateNavigation.Month } : null
                });
            }

            // Educations
            foreach (DimEducation e in factFieldValues.Where(ffv => ffv.DimEducationId > 0).Select(ffv => ffv.DimEducation).ToList())
            {
                profileData.Educations.Add(new Education
                {
                    Name = e.NameEn
                });
            }

            // TTV publications
            foreach (DimPublication p in factFieldValues.Where(ffv => ffv.DimPublicationId > 0).Select(ffv => ffv.DimPublication).ToList())
            {
                string? publicationAbstract = p.DimDescriptiveItems.Where(di => di.DescriptiveItemType == "Abstract").Select(di => di.DescriptiveItem).FirstOrDefault();
                if (publicationAbstract != null)
                {
                    publicationAbstract = $"{publicationAbstract.Substring(0, Math.Min(200, publicationAbstract.Length))}";
                }
                else
                {
                    publicationAbstract = "";
                }
                profileData.Publications.Add(new Publication
                {
                    Name = p.PublicationName,
                    Abstract = publicationAbstract,
                    Year = p.PublicationYear
                });
            }

            // ORCID publications
            foreach (DimProfileOnlyPublication p in factFieldValues.Where(ffv => ffv.DimProfileOnlyPublicationId > 0).Select(ffv => ffv.DimProfileOnlyPublication).ToList())
            {
                profileData.Publications.Add(new Publication
                {
                    Name = p.PublicationName,
                    Year = p.PublicationYear
                });
            }

            return profileData;
        }
    }
}
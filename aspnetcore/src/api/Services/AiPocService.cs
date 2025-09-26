using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
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



        public async Task<string?> GetProfileDataForPromt(string orcidId)
        {
            List<FactFieldValue> factFieldValues = await _ttvContext.FactFieldValues
                .Include(ffv => ffv.DimName)
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.DimOrganization)
                        .ThenInclude(aff_org => aff_org.DimOrganizationBroaderNavigation)
                .Include(ffv => ffv.DimEducation)
                .Include(ffv => ffv.DimPublication)
                .Include(ffv => ffv.DimProfileOnlyPublication)
                .Where(ffv => ffv.DimUserProfile != null && ffv.DimUserProfile.OrcidId == orcidId && ffv.Show == true)
                .AsNoTracking()
                .ToListAsync();

            const string separator = "; ";

            if (factFieldValues.Count == 0)
            {
                _logger.LogWarning("No data found for ORCID {orcidId}", orcidId);
                return null;
            }
            string promptData = "";

            // Names
            foreach (DimName n in factFieldValues.Where(ffv => ffv.DimNameId > 0 && (!string.IsNullOrWhiteSpace(ffv.DimName.FirstNames) || !string.IsNullOrWhiteSpace(ffv.DimName.LastName))).Select(ffv => ffv.DimName).ToList())
            {
                promptData += $"{n.FirstNames} {n.LastName}{separator}";
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

                promptData += $"{a.PositionNameEn} {organizationName}{separator}";
            }

            // Educations
            foreach (DimEducation e in factFieldValues.Where(ffv => ffv.DimEducationId > 0).Select(ffv => ffv.DimEducation).ToList())
            {
                promptData += $"{e.NameEn}{separator}";
            }

            // TTV publications
            foreach (DimPublication p in factFieldValues.Where(ffv => ffv.DimPublicationId > 0).Select(ffv => ffv.DimPublication).ToList())
            {
                promptData += $"{p.PublicationName} {p.Abstract.Substring(0, Math.Min(100, p.Abstract.Length))} ({p.PublicationYear}){separator}";
            }

            // ORCID publications
            foreach (DimProfileOnlyPublication p in factFieldValues.Where(ffv => ffv.DimProfileOnlyPublicationId > 0).Select(ffv => ffv.DimProfileOnlyPublication).ToList())
            {
                promptData += $"{p.PublicationName} ({p.PublicationYear}){separator}";
            }

            return promptData;
        }
    }
}
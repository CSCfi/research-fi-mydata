using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Orcid;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    /*
     * OrganizationHandlerService implements logic related to DimOrganization.
     */
    public class OrganizationHandlerService
    {
        private readonly TtvContext _ttvContext;
        private readonly UtilityService _utilityService;

        public OrganizationHandlerService(TtvContext ttvContext, UtilityService utilityService)
        {
            _ttvContext = ttvContext;
            _utilityService = utilityService;
        }

        /*
         * Map ORCID field disambiguation-source to pid_type in DimPid
         */
        private string MapOrcidDisambiguationSourceToPidType(string orcidDisambiguationSource)
        {
            string pidType = "";
            switch (orcidDisambiguationSource)
            {
                case "ROR":
                    pidType = "ror";
                    break;
                case "FUNDREF":
                    pidType = "fundref";
                    break;
                case "RINGGOLD":
                    pidType = "ringgold";
                    break;
                default:
                    break;
            }
            return pidType;
        }

        /*
         * Find organization by ORCID disambiguation identifier.
         */
        public async Task<int?> FindOrganizationIdByOrcidDisambiguationIdentifier(string orcidDisambiguationSource, string orcidDisambiguatedOrganizationIdentifier)
        {
            // Map ORCID field disambiguation-source to pid_type in DimPid. 
            string pidType = MapOrcidDisambiguationSourceToPidType(orcidDisambiguationSource);

            // Exit immediately, if either of the search conditions is empty.
            if (pidType == "" || orcidDisambiguatedOrganizationIdentifier == "")
            {
                return null;
            }

            // Find matching DimPid.
            DimPid dimPid = await _ttvContext.DimPids.Include(dp => dp.DimOrganization).Where(dp => dp.PidType == pidType && dp.PidContent == orcidDisambiguatedOrganizationIdentifier).AsNoTracking().FirstOrDefaultAsync();

            // Check object validity and return related DimOrganization.
            if (dimPid == null || dimPid.Id == -1 || dimPid.DimOrganizationId == -1)
            {
                return null;
            }
            else
            {
                return dimPid.DimOrganization.Id;
            }
        }

        /*
         * Create entity DimIdentifierlessData for organization name.
         */
        public DimIdentifierlessDatum CreateIdentifierlessData_OrganizationName(string nameFi, string nameEn, string nameSv)
        {
            DateTime currentDateTime = _utilityService.GetCurrentDateTime();
            return new DimIdentifierlessDatum()
            {
                Type = "organization_name",
                DimIdentifierlessDataId = -1,
                ValueFi = nameFi,
                ValueEn = nameEn,
                ValueSv = nameSv,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.ORCID,
                Created = currentDateTime,
                Modified = currentDateTime,
                UnlinkedIdentifier = null
            };
        }

        /*
         * Create entity DimIdentifierlessData for department name.
         */
        public DimIdentifierlessDatum CreateIdentifierlessData_DepartmentName(int? dimIdentifierlessDataId, string nameFi, string nameEn, string nameSv)
        {
            int convertedDimIdentifierlessDataId = (dimIdentifierlessDataId == null || dimIdentifierlessDataId < 1) ? -1 : (int)dimIdentifierlessDataId;
            DateTime currentDateTime = _utilityService.GetCurrentDateTime();
            return new DimIdentifierlessDatum()
            {
                Type = "organization_unit",
                DimIdentifierlessDataId = convertedDimIdentifierlessDataId,
                ValueFi = nameFi,
                ValueEn = nameEn,
                ValueSv = nameSv,
                SourceId = Constants.SourceIdentifiers.PROFILE_API,
                SourceDescription = Constants.SourceDescriptions.ORCID,
                Created = currentDateTime,
                Modified = currentDateTime,
                UnlinkedIdentifier = null
            };
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Common;
using api.Models.ProfileEditor;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    /*
     * OrganizationHandlerService implements logic related to DimOrganization.
     */
    public class OrganizationHandlerService : IOrganizationHandlerService
    {
        private readonly TtvContext _ttvContext;
        private readonly IUtilityService _utilityService;

        public OrganizationHandlerService(TtvContext ttvContext, IUtilityService utilityService)
        {
            _ttvContext = ttvContext;
            _utilityService = utilityService;
        }

        // For unit test
        public OrganizationHandlerService() { }

        /*
         * Map ORCID field disambiguation-source to pid_type in DimPid
         */
        public string MapOrcidDisambiguationSourceToPidType(string orcidDisambiguationSource)
        {
            // TODO: add more mappings, when both ORCID disambiguation-source and DimPid.PidType values are known
            switch (orcidDisambiguationSource)
            {
                case "GRID":
                    return "GRIDID";
                case "RINGGOLD":
                    return "RinggoldID";
                case "ROR":
                    return "RORID";
            }
            // By default return empty string, do not return original disambiguation-source.
            // This prevents unneccessary searches for DimPid.
            return "";
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
         * Create entity DimIdentifierlessData for organization unit.
         */
        public DimIdentifierlessDatum CreateIdentifierlessData_OrganizationUnit(DimIdentifierlessDatum parentDimIdentifierlessData, string nameFi, string nameEn, string nameSv)
        {
            DateTime currentDateTime = _utilityService.GetCurrentDateTime();
            return new DimIdentifierlessDatum()
            {
                Type = "organization_unit",
                DimIdentifierlessData = parentDimIdentifierlessData,
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
         * Get affiliation department name from FactFieldValue related DimIdentifierlessData.
         */

        // TODO: Remove this version?
        public string GetAffiliationDepartmentNameFromFactFieldValue(FactFieldValue factFieldValue)
        {
            if (factFieldValue.DimIdentifierlessDataId > 0 && factFieldValue.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
            {
                return factFieldValue.DimIdentifierlessData.ValueEn;
            }
            else if (
                factFieldValue.DimIdentifierlessDataId > 0 && factFieldValue.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME &&
                factFieldValue.DimIdentifierlessData.InverseDimIdentifierlessData.Count > 0 && factFieldValue.DimIdentifierlessData.InverseDimIdentifierlessData.First().Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT
            )
            {
                return factFieldValue.DimIdentifierlessData.InverseDimIdentifierlessData.First().ValueEn;
            }

            return "";
        }
    }
}
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
     * OrganizationHandlerService implements logic related to organizations.
     * They can be known entities, which are stored in table dim_organization,
     * or they can be unknown, which are stored in table dim_identifierless_data.
     * 
     * For example, when importing data from ORCID, the organization is searched by
     * disambiguation source and in a good case it will be mapped to a row in dim_organization.
     * If match is not found, then organization info is added to dim_identifierless_data.
     * 
     * In table dim_identifierless_data organization name is stored to one row, and
     * organization unit to another row. The "unit row" is a child to "name row".
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
        public OrganizationHandlerService(IUtilityService utilityService) { _utilityService = utilityService; }

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
                Type = Constants.IdentifierlessDataTypes.ORGANIZATION_NAME,
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
                Type = Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT,
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


        public void DepartmentNameHandling(FactFieldValue ffv, string departmentNameFi, string departmentNameEn, string departmentNameSv)
        {
            // ORCID employment contains 'department-name'

            // Check if FactFieldValue has related DimIdentifierlessData.
            //     If exists, check if type is 'organization_name' or 'organization_unit'
            //         If type is 'organization_name', check if it has related DimIdentifierlessData of type 'organization_unit'. If exists, update value. If does not exist, create new.
            //         If type is 'organization_unit, update value.
            //     If does not exist, create new using type 'organization_unit'.
            DateTime currentDateTime = _utilityService.GetCurrentDateTime();
            if (ffv.DimIdentifierlessData != null)
            {
                // DimIdentifierlessData exists
                // Check type.
                if (ffv.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                {
                    // Type is 'organization_name'. Check if it has related DimIdentifierlessData of type 'organization_unit'
                    if (
                        ffv.DimIdentifierlessData.InverseDimIdentifierlessData.Count > 0 &&
                        ffv.DimIdentifierlessData.InverseDimIdentifierlessData.First().Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT
                    )
                    {
                        // Has related DimIdentifierlessData of type 'organization_unit'. Update.
                        ffv.DimIdentifierlessData.InverseDimIdentifierlessData.First().ValueFi = departmentNameFi;
                        ffv.DimIdentifierlessData.InverseDimIdentifierlessData.First().ValueEn = departmentNameEn;
                        ffv.DimIdentifierlessData.InverseDimIdentifierlessData.First().ValueSv = departmentNameSv;
                        ffv.DimIdentifierlessData.InverseDimIdentifierlessData.First().Modified = currentDateTime;
                    }
                    else
                    {
                        // Does not have related DimIdentifierlessData of type 'organization_unit'. Add new. Set as child of DimIdentifierlessData of type 'organization_name'
                        DimIdentifierlessDatum dimIdentifierlessData_organizationUnit =
                            CreateIdentifierlessData_OrganizationUnit(
                                parentDimIdentifierlessData: ffv.DimIdentifierlessData,
                                nameFi: departmentNameFi,
                                nameEn: departmentNameEn,
                                nameSv: departmentNameSv
                            );
                        _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessData_organizationUnit);
                    }
                }
                else if (ffv.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                {
                    // Type is 'organization_unit'. Update
                    ffv.DimIdentifierlessData.ValueFi = departmentNameFi;
                    ffv.DimIdentifierlessData.ValueEn = departmentNameEn;
                    ffv.DimIdentifierlessData.ValueSv = departmentNameSv;
                    ffv.DimIdentifierlessData.Modified = currentDateTime;
                }
            }
            else
            {
                // DimIdentifierlessData does not exist. Create new if any of the language strings contains a value.
                // Do not set parent DimIdentifierlessData, instead link to FactFieldValue
                if (!(String.IsNullOrWhiteSpace(departmentNameFi) && String.IsNullOrWhiteSpace(departmentNameEn) && String.IsNullOrWhiteSpace(departmentNameSv)))
                {
                    DimIdentifierlessDatum dimIdentifierlessData_organizationUnit =
                        CreateIdentifierlessData_OrganizationUnit(
                            parentDimIdentifierlessData: null,
                            nameFi: departmentNameFi,
                            nameEn: departmentNameEn,
                            nameSv: departmentNameSv
                        );
                    _ttvContext.DimIdentifierlessData.Add(dimIdentifierlessData_organizationUnit);
                    ffv.DimIdentifierlessData = dimIdentifierlessData_organizationUnit;
                }
            }
        }

    }
}
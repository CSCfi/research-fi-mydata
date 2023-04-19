using System.Threading.Tasks;
using api.Models.Ttv;

namespace api.Services
{
    public interface IOrganizationHandlerService
    {
        DimIdentifierlessDatum CreateIdentifierlessData_OrganizationName(string nameFi, string nameEn, string nameSv);
        DimIdentifierlessDatum CreateIdentifierlessData_OrganizationUnit(DimIdentifierlessDatum parentDimIdentifierlessData, string nameFi, string nameEn, string nameSv);
        void DepartmentNameHandling(FactFieldValue ffv, string departmentNameFi, string departmentNameEn, string departmentNameSv);
        Task<int?> FindOrganizationIdByOrcidDisambiguationIdentifier(string orcidDisambiguationSource, string orcidDisambiguatedOrganizationIdentifier);
        string MapOrcidDisambiguationSourceToPidType(string orcidDisambiguationSource);
    }
}
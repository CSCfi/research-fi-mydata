using api.Models.ProfileEditor;
using api.Models.Ttv;

namespace api.Services
{
    public interface ITtvSqlService
    {
        string GetFactFieldValuesFKColumnNameFromFieldIdentifier(int fieldIdentifier);
        string GetSqlQuery_Delete_BrGrantedPermissions(int userprofileId);
        string GetSqlQuery_Delete_DimFieldDisplaySettings(int userprofileId);
        string GetSqlQuery_Delete_DimIdentifierlessData_Children(int dimIdentifierlessDataId);
        string GetSqlQuery_Delete_DimIdentifierlessData_Parent(int id);
        string GetSqlQuery_Delete_DimPid_ORCID_PutCode(int id);
        string GetSqlQuery_Delete_DimUserChoices(int userprofileId);
        string GetSqlQuery_Delete_DimUserProfile(int userprofileId);
        string GetSqlQuery_Delete_FactFieldValueRelatedData(FactFieldValue ffv);
        string GetSqlQuery_Delete_FactFieldValues(int userprofileId);
        string GetSqlQuery_ProfileData(int userprofileId);
        string GetSqlQuery_Select_FactFieldValues(int userprofileId);
        string GetSqlQuery_Update_FactFieldValues(int dimUserProfileId, ProfileEditorItemMeta profileEditorItemMeta);
    }
}
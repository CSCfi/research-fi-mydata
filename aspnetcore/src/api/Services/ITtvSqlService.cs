using api.Models.ProfileEditor;
using api.Models.Ttv;

namespace api.Services
{
    public interface ITtvSqlService
    {
        string GetFactFieldValuesFKColumnNameFromFieldIdentifier(int fieldIdentifier);
        string GetSqlQuery_Delete_FactFieldValueRelatedData(FactFieldValue ffv);
        string GetSqlQuery_ProfileData(int userprofileId);
        string GetSqlQuery_Update_FactFieldValues(int dimUserProfileId, ProfileEditorItemMeta profileEditorItemMeta);
    }
}
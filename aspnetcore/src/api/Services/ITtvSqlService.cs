using api.Models.ProfileEditor;

namespace api.Services
{
    public interface ITtvSqlService
    {
        string GetFactFieldValuesFKColumnNameFromFieldIdentifier(int fieldIdentifier);
        string GetSqlQuery_Update_FactFieldValues(int dimUserProfileId, ProfileEditorItemMeta profileEditorItemMeta);
    }
}
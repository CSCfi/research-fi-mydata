using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.ProfileEditor;
using api.Models.Ttv;

namespace api.Services
{
    public interface ISharingService
    {
        Task AddPermissions(int userprofileId, List<ProfileEditorSharingPermissionToAddOrDelete> permissionsToAdd);
        Task DeleteAllGrantedPermissionsFromUserprofile(int userprofileId);
        Task DeletePermissions(int userprofileId, List<ProfileEditorSharingPermissionToAddOrDelete> permissionsToDelete);
        Task<List<BrGrantedPermission>> GetDefaultSharingPermissionsListForUserProfile(DimUserProfile dimUserProfile);
        string GetDimReferenceDataCodeScheme();
        List<string> GetDimReferenceDataCodeValues();
        Task<ProfileEditorSharingPermissionsResponse> GetProfileEditorSharingPermissionsResponse();
        Task<ProfileEditorSharingPurposesResponse> GetProfileEditorSharingPurposesResponse();
        Task<ProfileEditorSharingGivenPermissionsResponse> GetProfileEditorSharingResponse(int userprofileId);
    }
}
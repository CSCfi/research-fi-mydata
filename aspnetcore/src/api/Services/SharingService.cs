using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Common;
using api.Models.ProfileEditor;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    /*
     * SharingService implements user profile sharing related functionality.
     */
    public class SharingService
    {
        private readonly TtvContext _ttvContext;
        private readonly DataSourceHelperService _dataSourceHelperService;
        private readonly LanguageService _languageService;

        public SharingService(TtvContext ttvContext,
            DataSourceHelperService dataSourceHelperService,
            LanguageService languageService)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
            _languageService = languageService;
        }

        // For unit test
        public SharingService() { }

        /*
         * Get DimReferenceData code_scheme for sharing.
         */
        public string GetDimReferenceDataCodeScheme()
        {
            return Constants.ReferenceDataCodeSchemes.PROFILE_SHARING;
        }

        /*s
         * Get DimReferenceData code_values for sharing.
         */
        public List<string> GetDimReferenceDataCodeValues()
        {
            return new List<string>()
            {
                Constants.ReferenceDataCodeValues.PROFILE_SHARING_PROFILE_INFORMATION,
                Constants.ReferenceDataCodeValues.PROFILE_SHARING_EMAIL_ADDRESS,
                Constants.ReferenceDataCodeValues.PROFILE_SHARING_PHONE_NUMBER,
                Constants.ReferenceDataCodeValues.PROFILE_SHARING_AFFILIATION_AND_EDUCATION,
                Constants.ReferenceDataCodeValues.PROFILE_SHARING_PUBLICATIONS,
                Constants.ReferenceDataCodeValues.PROFILE_SHARING_DATASETS,
                Constants.ReferenceDataCodeValues.PROFILE_SHARING_GRANTS,
                Constants.ReferenceDataCodeValues.PROFILE_SHARING_ACTIVITIES_AND_DISTINCTIONS
            };
        }

        /*
         * Get a list of default user profile sharing permissions.
         */
        public async Task<List<BrGrantedPermission>> GetDefaultSharingPermissionsListForUserProfile(int userprofileId)
        {
            List<BrGrantedPermission> defaultSharingPermissions = new();
            foreach (string sharingGroupIdentifier in GetDimReferenceDataCodeValues())
            {
                DimReferencedatum dimReferencedata = await _ttvContext.DimReferencedata.AsNoTracking().FirstOrDefaultAsync(dr => dr.CodeScheme == GetDimReferenceDataCodeScheme() && dr.CodeValue == sharingGroupIdentifier);

                if (dimReferencedata != null)
                {
                    defaultSharingPermissions.Add(
                        new BrGrantedPermission()
                        {
                            DimUserProfileId = userprofileId,
                            DimExternalServiceId = _dataSourceHelperService.DimPurposeId_TTV,
                            DimPermittedFieldGroup = dimReferencedata.Id
                        }
                    );
                }
            }
            return defaultSharingPermissions;
        }

        /*
         * Delete all granted permissions.
         */
        public async Task DeleteAllGrantedPermissionsFromUserprofile(int userprofileId)
        {
            List<BrGrantedPermission> grantedPermissions = await _ttvContext.BrGrantedPermissions.Where(bgp => bgp.DimUserProfileId == userprofileId).ToListAsync();
            _ttvContext.BrGrantedPermissions.RemoveRange(grantedPermissions);
        }

        /*
         * Get list of sharing purposes.
         */
        public async Task<ProfileEditorSharingPurposesResponse> GetProfileEditorSharingPurposesResponse()
        {
            List<ProfileEditorSharingPurposeItem> profileEditorSharingPurposeItems = new();

            foreach (DimPurpose dimPurpose in await _ttvContext.DimPurposes.AsNoTracking().ToListAsync())
            {
                // Translate purpose name
                NameTranslation nameTranslation = _languageService.GetNameTranslation(
                    nameFi: dimPurpose.NameFi,
                    nameEn: dimPurpose.NameEn,
                    nameSv: dimPurpose.NameSv
                );

                // Translate purpose description
                NameTranslation descriptionTranslation = _languageService.GetNameTranslation(
                    nameFi: dimPurpose.DescriptionFi,
                    nameEn: dimPurpose.DescriptionEn,
                    nameSv: dimPurpose.DescriptionSv
                );

                profileEditorSharingPurposeItems.Add(
                    new ProfileEditorSharingPurposeItem()
                    {
                        PurposeId = dimPurpose.Id,
                        NameFi = nameTranslation.NameFi,
                        NameEn = nameTranslation.NameEn,
                        NameSv = nameTranslation.NameSv,
                        DescriptionFi = descriptionTranslation.NameFi,
                        DescriptionEn = descriptionTranslation.NameEn,
                        DescriptionSv = descriptionTranslation.NameSv
                    }
                );
            }

            return new ProfileEditorSharingPurposesResponse(items: profileEditorSharingPurposeItems);
        }

        /*
         * Get list of sharing permissions.
         */
        public async Task<ProfileEditorSharingPermissionsResponse> GetProfileEditorSharingPermissionsResponse()
        {
            List<ProfileEditorSharingPermissionItem> profileEditorSharingPermissionItems = new();

            foreach (DimReferencedatum dimReferencedata in await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == GetDimReferenceDataCodeScheme()).AsNoTracking().ToListAsync())
            {
                // Translate permission name
                NameTranslation nameTranslation = _languageService.GetNameTranslation(
                    nameFi: dimReferencedata.NameFi,
                    nameEn: dimReferencedata.NameEn,
                    nameSv: dimReferencedata.NameSv
                );

                profileEditorSharingPermissionItems.Add(
                    new ProfileEditorSharingPermissionItem()
                    {
                        PermissionId = dimReferencedata.Id,
                        NameFi = nameTranslation.NameFi,
                        NameEn = nameTranslation.NameEn,
                        NameSv = nameTranslation.NameSv,
                    }
                );
            }

            return new ProfileEditorSharingPermissionsResponse(items: profileEditorSharingPermissionItems);
        }

        /*
         * Get list of given permissions.
         */
        public async Task<ProfileEditorSharingGivenPermissionsResponse> GetProfileEditorSharingResponse(int userprofileId)
        {
            List<ProfileEditorSharingItem> profileEditorSharingItems = new();

            // Get all BrGrantedPermissions related to the user profile
            List<BrGrantedPermission> grantedPermissions = await _ttvContext.BrGrantedPermissions
                .Include(bgp => bgp.DimExternalService).AsNoTracking()
                .Include(bgp => bgp.DimPermittedFieldGroupNavigation).AsNoTracking()
                .Where(bgp => bgp.DimUserProfileId == userprofileId).AsNoTracking().ToListAsync();

            // Group BrGrantedPermissions by DimExternalServiceId
            IEnumerable<IGrouping<int, BrGrantedPermission>> groupedBrGrantedPermissions = grantedPermissions.GroupBy(bgp => bgp.DimExternalServiceId);

            foreach (IGrouping<int, BrGrantedPermission> permissionGroup in groupedBrGrantedPermissions)
            {
                // Since grouping is done based on DimExternalServiceId, all items in the group
                // are related to the same DimExternalService. Therefore the properties of
                // DimExternalService can be taken from any item, here First() is used.

                // Translate purpose name
                NameTranslation purposeNameTranslation = _languageService.GetNameTranslation(
                    nameFi: permissionGroup.First().DimExternalService.NameFi,
                    nameEn: permissionGroup.First().DimExternalService.NameEn,
                    nameSv: permissionGroup.First().DimExternalService.NameSv
                );

                // Translate purpose description
                NameTranslation purposeDescriptionTranslation = _languageService.GetNameTranslation(
                    nameFi: permissionGroup.First().DimExternalService.DescriptionFi,
                    nameEn: permissionGroup.First().DimExternalService.DescriptionEn,
                    nameSv: permissionGroup.First().DimExternalService.DescriptionSv
                );

                ProfileEditorSharingItem profileEditorSharingItem = new()
                {
                    Purpose = new ProfileEditorSharingPurposeItem()
                    {
                        PurposeId = permissionGroup.First().DimExternalServiceId,
                        NameFi = purposeNameTranslation.NameFi,
                        NameEn = purposeNameTranslation.NameEn,
                        NameSv = purposeNameTranslation.NameSv,
                        DescriptionFi = purposeDescriptionTranslation.NameFi,
                        DescriptionEn = purposeDescriptionTranslation.NameEn,
                        DescriptionSv = purposeDescriptionTranslation.NameSv
                    },
                    Permissions = new List<ProfileEditorSharingPermissionItem>()
                };

                // Loop group to collect related permission items.
                foreach (BrGrantedPermission permission in permissionGroup)
                {
                    // Translate permission name
                    NameTranslation permissionNameTranslation = _languageService.GetNameTranslation(
                        nameFi: permission.DimPermittedFieldGroupNavigation.NameFi,
                        nameEn: permission.DimPermittedFieldGroupNavigation.NameEn,
                        nameSv: permission.DimPermittedFieldGroupNavigation.NameSv
                    );

                    profileEditorSharingItem.Permissions.Add(
                        new ProfileEditorSharingPermissionItem()
                        {
                            PermissionId = permission.DimPermittedFieldGroup,
                            NameFi = permissionNameTranslation.NameFi,
                            NameEn = permissionNameTranslation.NameEn,
                            NameSv = permissionNameTranslation.NameSv
                        }
                    );
                }
                profileEditorSharingItems.Add(profileEditorSharingItem);
            }

            return new ProfileEditorSharingGivenPermissionsResponse(items: profileEditorSharingItems);
        }


        /*
         * Add permissions.
         */
        public async Task AddPermissions(int userprofileId, List<ProfileEditorSharingPermissionToAddOrDelete> permissionsToAdd)
        {
            foreach (ProfileEditorSharingPermissionToAddOrDelete permission in permissionsToAdd)
            {
                // Validate purpose ID
                DimPurpose dimPurpose = await _ttvContext.DimPurposes.Where(dp => dp.Id == permission.PurposeId).AsNoTracking().FirstOrDefaultAsync();
                if (dimPurpose == null)
                {
                    break;
                }

                // Validate permission ID
                DimReferencedatum dimReferencedata = await _ttvContext.DimReferencedata.Where(dr => dr.Id == permission.PermissionId).AsNoTracking().FirstOrDefaultAsync();
                if (dimReferencedata == null)
                {
                    break;
                }

                // Check that permission is not already added
                BrGrantedPermission brGrantedPermissionExisting = await _ttvContext.BrGrantedPermissions.Where(
                    bgp => bgp.DimUserProfileId == userprofileId &&
                    bgp.DimExternalServiceId == permission.PurposeId &&
                    bgp.DimPermittedFieldGroup == permission.PermissionId
                ).FirstOrDefaultAsync();

                // Add permission
                if (brGrantedPermissionExisting == null)
                {
                    _ttvContext.BrGrantedPermissions.Add(
                        new BrGrantedPermission()
                        {
                            DimUserProfileId = userprofileId,
                            DimExternalServiceId = permission.PurposeId,
                            DimPermittedFieldGroup = permission.PermissionId
                        }
                    );
                }
            }
        }

        /*
         * Delete permissions.
         */
        public async Task DeletePermissions(int userprofileId, List<ProfileEditorSharingPermissionToAddOrDelete> permissionsToDelete)
        {
            foreach (ProfileEditorSharingPermissionToAddOrDelete permission in permissionsToDelete)
            {
                BrGrantedPermission brGrantedPermission = await _ttvContext.BrGrantedPermissions.Where(
                    bgp => bgp.DimUserProfileId == userprofileId &&
                    bgp.DimExternalServiceId == permission.PurposeId &&
                    bgp.DimPermittedFieldGroup == permission.PermissionId
                ).FirstOrDefaultAsync();

                if (brGrantedPermission != null)
                {
                    _ttvContext.BrGrantedPermissions.Remove(brGrantedPermission);
                }
            }
        }
    }
}
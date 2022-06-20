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
         * Get DimReferenceData containing permission values.
         */
        public async Task<List<DimReferencedatum>> GetDimReferenceData()
        {
            List<DimReferencedatum> dimReferenceDataList = new();
            foreach (string sharingGroupIdentifier in GetDimReferenceDataCodeValues())
            {
                DimReferencedatum dimReferencedata = await _ttvContext.DimReferencedata.AsNoTracking().FirstOrDefaultAsync(dr => dr.CodeScheme == GetDimReferenceDataCodeScheme() && dr.CodeValue == sharingGroupIdentifier);
                if (dimReferencedata != null)
                {
                    dimReferenceDataList.Add(dimReferencedata);
                }
            }
            return dimReferenceDataList;
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
         * Delete all granted permissions from user profile.
         */
        public async Task DeleteAllGrantedPermissionsFromUserprofile(int userprofileId)
        {
            List<BrGrantedPermission> grantedPermissions = await _ttvContext.BrGrantedPermissions.Where(bgp => bgp.DimUserProfileId == userprofileId).ToListAsync();
            _ttvContext.BrGrantedPermissions.RemoveRange(grantedPermissions);
        }

        /*
         * Get list of sharing purposes for profile editor.
         */
        public async Task<ProfileEditorSharingPurposesResponse> GetProfileEditorSharingPurposesResponse()
        {
            List<ProfileEditorSharingPurposeItem> profileEditorSharingPurposeItems = new();

            foreach (DimPurpose dimPurpose in await _ttvContext.DimPurposes.AsNoTracking().ToListAsync())
            {
                NameTranslation nameTranslation = _languageService.GetNameTranslation(
                    nameFi: dimPurpose.NameFi,
                    nameEn: dimPurpose.NameEn,
                    nameSv: dimPurpose.NameSv
                );

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
         * Get list of sharing permissions for profile editor.
         */
        public async Task<ProfileEditorSharingPermissionsResponse> GetProfileEditorSharingPermissionsResponse()
        {
            List<ProfileEditorSharingPermissionItem> profileEditorSharingPermissionItems = new();

            foreach (DimReferencedatum dimReferencedata in await _ttvContext.DimReferencedata.Where(dr => dr.CodeScheme == GetDimReferenceDataCodeScheme()).AsNoTracking().ToListAsync())
            {
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
         * Get list of sharing items for profile editor.
         */
        public async Task<ProfileEditorSharingResponse> GetProfileEditorSharingResponse(int userprofileId)
        {
            List<ProfileEditorSharingItem> profileEditorSharingItems = new();

            foreach (BrGrantedPermission brGrantedPermission in await _ttvContext.BrGrantedPermissions
                .Include(bgp => bgp.DimExternalService).AsNoTracking()
                .Where(bgp => bgp.DimUserProfileId == userprofileId).AsNoTracking().ToListAsync())
            {
                NameTranslation nameTranslation = _languageService.GetNameTranslation(
                    nameFi: brGrantedPermission.DimExternalService.NameFi,
                    nameEn: brGrantedPermission.DimExternalService.NameEn,
                    nameSv: brGrantedPermission.DimExternalService.NameSv
                );

                NameTranslation descriptionTranslation = _languageService.GetNameTranslation(
                    nameFi: brGrantedPermission.DimExternalService.DescriptionFi,
                    nameEn: brGrantedPermission.DimExternalService.DescriptionEn,
                    nameSv: brGrantedPermission.DimExternalService.DescriptionSv
                );


                profileEditorSharingItems.Add(
                    new ProfileEditorSharingItem()
                    {
                        NameFi = nameTranslation.NameFi,
                        NameEn = nameTranslation.NameEn,
                        NameSv = nameTranslation.NameSv,
                        DescriptionFi = descriptionTranslation.NameFi,
                        DescriptionEn = descriptionTranslation.NameEn,
                        DescriptionSv = descriptionTranslation.NameSv,
                        Meta = new ProfileEditorSharingItemMeta(purposeId: brGrantedPermission.DimExternalServiceId,
                            permittedFieldGroupId: brGrantedPermission.DimPermittedFieldGroup)
                    }
                );
            }

            return new ProfileEditorSharingResponse(items: profileEditorSharingItems);
        }
    }
}
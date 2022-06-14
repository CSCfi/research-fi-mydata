using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
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

        public SharingService(TtvContext ttvContext,
            DataSourceHelperService dataSourceHelperService)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
        }

        // For unit test
        public SharingService(){}

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
         * Delete all granted permissions from user profile.
         */
        public async Task DeleteAllGrantedPermissionsFromUserprofile(int userprofileId)
        {
            List<BrGrantedPermission> grantedPermissions = await _ttvContext.BrGrantedPermissions.Where(bgp => bgp.DimUserProfileId == userprofileId).ToListAsync();
            _ttvContext.BrGrantedPermissions.RemoveRange(grantedPermissions);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using api.Models.ProfileEditor;
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
         * Get SharingGroupIdentifiers.
         */
        public List<int> GetSharingGroupIdentifiers()
        {
            return new List<int>()
            {
                Constants.SharingGroupIdentifiers.PROFILE_INFORMATION,
                Constants.SharingGroupIdentifiers.EMAIL_ADDRESS,
                Constants.SharingGroupIdentifiers.PHONE_NUMBER,
                Constants.SharingGroupIdentifiers.AFFILIATION_AND_EDUCATION,
                Constants.SharingGroupIdentifiers.PUBLICATIONS,
                Constants.SharingGroupIdentifiers.DATASETS,
                Constants.SharingGroupIdentifiers.GRANTS,
                Constants.SharingGroupIdentifiers.ACTIVITIES_AND_DISTINCTIONS
            };
        }

        /*
         * Grant default permission set for a new user profile.
         */
        public List<BrGrantedPermission> GrantDefaultPermissions(int userprofileId)
        {
            List<BrGrantedPermission> grantedPermissions = new();

            // Get DimPurpose for Tiedejatutkimus.fi portal

            foreach (int sharingGroupIdentifier in GetSharingGroupIdentifiers())
            {
                grantedPermissions.Add(
                    new BrGrantedPermission()
                    {
                        DimUserProfileId = userprofileId,
                        DimExternalServiceId = -1,
                        DimPermittedFieldGroup = sharingGroupIdentifier
                    }
                );
            }

            return grantedPermissions;
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
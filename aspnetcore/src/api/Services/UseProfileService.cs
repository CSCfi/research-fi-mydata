using System;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{ 
    public class UserProfileService
    {
        private readonly TtvContext _ttvContext;

        public UserProfileService(TtvContext ttvContext)
        {
            _ttvContext = ttvContext;
        }

        public async Task<int> GetUserprofileId(String orcidId)
        {
            var dimPid = await _ttvContext.DimPids
                .Include(i => i.DimKnownPerson)
                    .ThenInclude(kp => kp.DimUserProfiles).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(p => p.PidContent == orcidId && p.PidType == "ORCID");

            if (dimPid == null || dimPid.DimKnownPerson == null || dimPid.DimKnownPerson.DimUserProfiles.Count() == 0)
            {
                return -1;
            }
            else
            {
                return dimPid.DimKnownPerson.DimUserProfiles.FirstOrDefault().Id;
            }
        }

        public async Task<int> GetOrcidRegisteredDataSourceId()
        {
            var orcidRegisteredDataSource = await _ttvContext.DimRegisteredDataSources.AsNoTracking().FirstOrDefaultAsync(p => p.Name == "ORCID");
            if (orcidRegisteredDataSource == null)
            {
                return -1;
            }
            else
            {
                return orcidRegisteredDataSource.Id;
            }
        }

        public async Task<DimName> AddOrUpdateDimName(String lastName, String firstNames, int dimKnownPersonId, int dimRegisteredDataSourceId)
        {
            var dimName = await _ttvContext.DimNames.FirstOrDefaultAsync(dn => dn.DimKnownPersonIdConfirmedIdentityNavigation.Id == dimKnownPersonId && dn.DimRegisteredDataSourceId == dimRegisteredDataSourceId);
            if (dimName == null)
            {
                dimName = new DimName()
                {
                    LastName = lastName,
                    FirstNames = firstNames,
                    DimKnownPersonIdConfirmedIdentity = dimKnownPersonId,
                    DimKnownPersonidFormerNames = -1,
                    SourceId = "",
                    Created = DateTime.Now,
                    DimRegisteredDataSourceId = dimRegisteredDataSourceId
                };
                _ttvContext.DimNames.Add(dimName);
            }
            else
            {
                dimName.LastName = lastName;
                dimName.FirstNames = firstNames;
                dimName.Modified = DateTime.Now;
            }
            await _ttvContext.SaveChangesAsync();
            return dimName;
        }
    }
}
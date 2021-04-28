using System;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.Models.Api;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public sealed class ConsentService : IConsentService
    {
        private readonly TtvContext _ttvContext;

        public ConsentService(TtvContext ttvContext)
        {
            _ttvContext = ttvContext;
        }

        public async Task<IReadOnlyList<ConsentTargetResponse>> GetTargetsAsync(string rorId, CancellationToken cancellationToken)
        {
            IQueryable<DimConsentTarget> query = _ttvContext.DimConsentTargets.AsNoTracking();
            if (rorId != null)
            {
                string normalizedRorId = rorId.Trim().ToUpperInvariant();
                if (normalizedRorId.Length == 0)
                {
                    return Array.Empty<ConsentTargetResponse>();
                }

                query = query.Where(target => _ttvContext.DimPids.Any(pid =>
                    pid.DimOrganizationId == target.ControllerOrganization.DimOrganizationId
                    && pid.PidType.ToUpper() == "RORID"
                    && pid.PidContent.ToUpper() == normalizedRorId));
            }

            return await query
                .OrderBy(target => target.ControllerOrganizationId)
                .ThenBy(target => target.Id)
                .Select(target => new ConsentTargetResponse(
                    target.Id,
                    target.ControllerOrganization.DimOrganization.NameFi,
                    target.ControllerOrganization.DimOrganization.NameSv,
                    target.ControllerOrganization.DimOrganization.NameEn,
                    target.ControllerOrganization.DimOrganization.DimPids
                        .Where(pid => pid.PidType == "RORID")
                        .Select(pid => pid.PidContent)
                        .FirstOrDefault(),
                    target.PurposeCode.NameFi,
                    target.PurposeCode.NameEn,
                    target.PurposeCode.NameSv))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ConsentResponse>> GetConsentsAsync(int userProfileId, CancellationToken cancellationToken)
        {
            DateTime utcNow = DateTime.UtcNow;
            return await _ttvContext.FactProfileDataSharingConsents
                .AsNoTracking()
                .Where(consent =>
                    consent.DimUserProfileId == userProfileId
                    && consent.ConsentWithdrawnDate == null
                    && (consent.ConsentExpireDate == null || consent.ConsentExpireDate >= utcNow))
                .OrderBy(consent => consent.ControllerOrganizationId)
                .Select(consent => new ConsentResponse(
                    _ttvContext.DimConsentTargets
                        .Where(target => target.ControllerOrganizationId == consent.ControllerOrganizationId)
                        .Select(target => target.Id)
                        .FirstOrDefault(),
                    consent.ControllerOrganization.DimOrganization.NameFi,
                    consent.ControllerOrganization.DimOrganization.NameSv,
                    consent.ControllerOrganization.DimOrganization.NameEn,
                    consent.ControllerOrganization.DimOrganization.DimPids
                        .Where(pid => pid.PidType == "RORID")
                        .Select(pid => pid.PidContent)
                        .FirstOrDefault(),
                    consent.ConsentGrantedDate,
                    consent.ConsentExpireDate))
                .ToListAsync(cancellationToken);
        }

        public async Task<ConsentGrantResult> GrantConsentAsync(int userProfileId, int consentTargetId, CancellationToken cancellationToken)
        {
            int? controllerOrganizationId = await _ttvContext.DimConsentTargets
                .AsNoTracking()
                .Where(target => target.Id == consentTargetId)
                .Select(target => (int?)target.ControllerOrganizationId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!controllerOrganizationId.HasValue)
            {
                return ConsentGrantResult.TargetNotFound;
            }

            bool consentAlreadyExists = await _ttvContext.FactProfileDataSharingConsents
                .AnyAsync(consent =>
                    consent.DimUserProfileId == userProfileId
                    && consent.ControllerOrganizationId == controllerOrganizationId.Value,
                    cancellationToken);
            if (consentAlreadyExists)
            {
                return ConsentGrantResult.ConsentAlreadyExists;
            }

            DateTime grantedDate = DateTime.UtcNow;
            var consent = new FactProfileDataSharingConsent
            {
                DimUserProfileId = userProfileId,
                ControllerOrganizationId = controllerOrganizationId.Value,
                ConsentGrantedDate = grantedDate,
                ConsentExpireDate = grantedDate.AddYears(5),
                ConsentWithdrawnDate = null,
                Created = grantedDate
            };
            _ttvContext.FactProfileDataSharingConsents.Add(consent);
            try
            {
                await _ttvContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                _ttvContext.Entry(consent).State = EntityState.Detached;
                bool concurrentConsentExists = await _ttvContext.FactProfileDataSharingConsents
                    .AnyAsync(item =>
                        item.DimUserProfileId == userProfileId
                        && item.ControllerOrganizationId == controllerOrganizationId.Value,
                        cancellationToken);
                if (concurrentConsentExists)
                {
                    return ConsentGrantResult.ConsentAlreadyExists;
                }

                throw;
            }

            return ConsentGrantResult.Created;
        }

        public async Task<bool> WithdrawConsentAsync(int userProfileId, int consentTargetId, CancellationToken cancellationToken)
        {
            int? controllerOrganizationId = await _ttvContext.DimConsentTargets
                .AsNoTracking()
                .Where(target => target.Id == consentTargetId)
                .Select(target => (int?)target.ControllerOrganizationId)
                .FirstOrDefaultAsync(cancellationToken);
            if (!controllerOrganizationId.HasValue)
            {
                return false;
            }

            FactProfileDataSharingConsent consent = await _ttvContext.FactProfileDataSharingConsents
                .FirstOrDefaultAsync(
                    item => item.DimUserProfileId == userProfileId
                        && item.ControllerOrganizationId == controllerOrganizationId.Value,
                    cancellationToken);
            if (consent == null)
            {
                return false;
            }

            _ttvContext.FactProfileDataSharingConsents.Remove(consent);
            await _ttvContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

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

        public async Task<IReadOnlyList<ConsentTargetResponse>> GetTargetsAsync(string rorId, int? userProfileId, CancellationToken cancellationToken)
        {
            IQueryable<DimConsentTarget> query = GetTargetsWithRor();
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
                    && pid.PidContent != null
                    && pid.PidContent.Trim().ToUpper() == normalizedRorId));
            }

            var results = await (
                from target in query
                join consent in ActiveConsents(userProfileId, DateTime.UtcNow)
                    on target.ControllerOrganizationId equals consent.ControllerOrganizationId into matchingConsents
                from consent in matchingConsents.DefaultIfEmpty()
                orderby target.ControllerOrganizationId, target.Id
                select new { Target = target, Consent = consent }
            ).ToListAsync(cancellationToken);

            return results.Where(result => HasRor(result.Target))
                .Select(result => MapTarget(result.Target, result.Consent)).ToList();
        }

        public async Task<IReadOnlyList<ConsentTargetResponse>> GetConsentsAsync(int userProfileId, CancellationToken cancellationToken)
        {
            DateTime utcNow = DateTime.UtcNow;
            var results = await (
                from target in GetTargetsWithRor()
                join consent in ActiveConsents(userProfileId, utcNow)
                    on target.ControllerOrganizationId equals consent.ControllerOrganizationId
                orderby target.ControllerOrganizationId, target.Id
                select new { Target = target, Consent = consent }
            ).ToListAsync(cancellationToken);

            return results.Where(result => HasRor(result.Target))
                .Select(result => MapTarget(result.Target, result.Consent)).ToList();
        }

        private IQueryable<FactProfileDataSharingConsent> ActiveConsents(int? userProfileId, DateTime utcNow) =>
            _ttvContext.FactProfileDataSharingConsents.AsNoTracking()
                .Where(consent => consent.DimUserProfileId == userProfileId
                    && consent.DateConsentWithdrawn == null
                    && (consent.DateConsentExpire == null || consent.DateConsentExpire >= utcNow));

        private IQueryable<DimConsentTarget> GetTargetsWithRor()
        {
            return _ttvContext.DimConsentTargets
                .AsNoTracking()
                .Where(target => target.ControllerOrganization.DimOrganization.DimPids.Any(pid =>
                    pid.PidType.ToUpper() == "RORID"
                    && pid.PidContent != null
                    && pid.PidContent.Trim() != ""))
                .Include(target => target.PurposeCode)
                .Include(target => target.ControllerOrganization)
                    .ThenInclude(controller => controller.DimOrganization)
                    .ThenInclude(organization => organization.DimPids.Where(pid => pid.PidType.ToUpper() == "RORID"));
        }

        private static bool HasRor(DimConsentTarget target) =>
            target.ControllerOrganization.DimOrganization.DimPids.Any(pid => !string.IsNullOrWhiteSpace(pid.PidContent));

        private static ConsentTargetResponse MapTarget(DimConsentTarget target, FactProfileDataSharingConsent consent)
        {
            var organization = target.ControllerOrganization.DimOrganization;
            string ror = organization.DimPids
                .Where(pid => string.Equals(pid.PidType, "RORID", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(pid.PidContent))
                .OrderBy(pid => pid.Id)
                .First().PidContent.Trim();
            var purpose = target.PurposeCode;
            return new ConsentTargetResponse(
                target.Id,
                new ConsentControllerResponse(new ConsentLanguageVariant(organization.NameFi, organization.NameSv, organization.NameEn), ror),
                new[] { new ConsentPurposeResponse(new ConsentLanguageVariant(purpose.NameFi, purpose.NameSv, purpose.NameEn), purpose.CodeValue) },
                new ConsentDetailsResponse(consent?.DateConsentGranted, consent?.DateConsentWithdrawn, consent?.DateConsentExpire));
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
                DateConsentGranted = grantedDate,
                DateConsentExpire = grantedDate.AddYears(5),
                DateConsentWithdrawn = null,
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

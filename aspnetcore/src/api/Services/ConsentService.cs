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
                orderby target.ControllerOrganizationId, target.PurposeCodeId
                select new { Target = target, Consent = consent }
            ).ToListAsync(cancellationToken);

            return results.Where(result => HasRor(result.Target))
                .GroupBy(result => result.Target.ControllerOrganizationId)
                .OrderBy(group => group.Key)
                .Select(group => MapTarget(
                    group.Select(result => result.Target).ToList(),
                    group.First().Consent))
                .ToList();
        }

        public async Task<IReadOnlyList<ConsentTargetResponse>> GetConsentsAsync(int userProfileId, CancellationToken cancellationToken)
        {
            DateTime utcNow = DateTime.UtcNow;
            var results = await (
                from target in GetTargetsWithRor()
                join consent in ActiveConsents(userProfileId, utcNow).Where(consent => consent.ConsentActive)
                    on target.ControllerOrganizationId equals consent.ControllerOrganizationId
                orderby target.ControllerOrganizationId, target.PurposeCodeId
                select new { Target = target, Consent = consent }
            ).ToListAsync(cancellationToken);

            return results.Where(result => HasRor(result.Target))
                .GroupBy(result => result.Target.ControllerOrganizationId)
                .OrderBy(group => group.Key)
                .Select(group => MapTarget(
                    group.Select(result => result.Target).ToList(),
                    group.First().Consent))
                .ToList();
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

        private static ConsentTargetResponse MapTarget(
            IReadOnlyList<DimConsentTarget> targets,
            FactProfileDataSharingConsent consent)
        {
            DimConsentTarget target = targets[0];
            var organization = target.ControllerOrganization.DimOrganization;
            string ror = organization.DimPids
                .Where(pid => string.Equals(pid.PidType, "RORID", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(pid.PidContent))
                .OrderBy(pid => pid.Id)
                .First().PidContent.Trim();
            return new ConsentTargetResponse(
                target.ControllerOrganizationId,
                new ConsentControllerResponse(new ConsentLanguageVariant(organization.NameFi, organization.NameSv, organization.NameEn), ror),
                targets.OrderBy(item => item.PurposeCodeId)
                    .Select(item => new ConsentPurposeResponse(
                        new ConsentLanguageVariant(item.PurposeCode.NameFi, item.PurposeCode.NameSv, item.PurposeCode.NameEn),
                        item.PurposeCode.CodeValue))
                    .ToList(),
                new ConsentDetailsResponse(consent?.DateConsentGranted, consent?.DateConsentWithdrawn, consent?.DateConsentExpire));
        }

        public async Task<ConsentGrantResult> GrantConsentAsync(int userProfileId, int controllerOrganizationId, CancellationToken cancellationToken)
        {
            bool targetExists = await _ttvContext.DimConsentTargets
                .AsNoTracking()
                .AnyAsync(target => target.ControllerOrganizationId == controllerOrganizationId, cancellationToken);
            if (!targetExists)
            {
                return ConsentGrantResult.TargetNotFound;
            }

            FactProfileDataSharingConsent existingConsent = await _ttvContext.FactProfileDataSharingConsents
                .FirstOrDefaultAsync(consent =>
                    consent.DimUserProfileId == userProfileId
                    && consent.ControllerOrganizationId == controllerOrganizationId,
                    cancellationToken);
            if (existingConsent?.ConsentActive == true)
            {
                return ConsentGrantResult.ConsentAlreadyExists;
            }

            DateTime grantedDate = DateTime.UtcNow;
            if (existingConsent != null)
            {
                existingConsent.DateConsentGranted = grantedDate;
                existingConsent.DateConsentExpire = grantedDate.AddYears(5);
                existingConsent.DateConsentWithdrawn = null;
                existingConsent.ConsentActive = true;
                existingConsent.Modified = grantedDate;
                await _ttvContext.SaveChangesAsync(cancellationToken);
                return ConsentGrantResult.Created;
            }

            var consent = new FactProfileDataSharingConsent
            {
                DimUserProfileId = userProfileId,
                ControllerOrganizationId = controllerOrganizationId,
                DateConsentGranted = grantedDate,
                DateConsentExpire = grantedDate.AddYears(5),
                DateConsentWithdrawn = null,
                ConsentActive = true,
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
                        && item.ControllerOrganizationId == controllerOrganizationId,
                        cancellationToken);
                if (concurrentConsentExists)
                {
                    return ConsentGrantResult.ConsentAlreadyExists;
                }

                throw;
            }

            return ConsentGrantResult.Created;
        }

        public async Task<bool> WithdrawConsentAsync(int userProfileId, int controllerOrganizationId, CancellationToken cancellationToken)
        {
            bool targetExists = await _ttvContext.DimConsentTargets
                .AsNoTracking()
                .AnyAsync(target => target.ControllerOrganizationId == controllerOrganizationId, cancellationToken);
            if (!targetExists)
            {
                return false;
            }

            FactProfileDataSharingConsent consent = await _ttvContext.FactProfileDataSharingConsents
                .FirstOrDefaultAsync(
                    item => item.DimUserProfileId == userProfileId
                        && item.ControllerOrganizationId == controllerOrganizationId
                        && item.ConsentActive,
                    cancellationToken);
            if (consent == null)
            {
                return false;
            }

            DateTime withdrawnDate = DateTime.UtcNow;
            consent.ConsentActive = false;
            consent.DateConsentWithdrawn = withdrawnDate;
            consent.Modified = withdrawnDate;
            await _ttvContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using api.Models.Common;
using api.Models.ProfileEditor;
using api.Models.Ttv;
using api.Models.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Serilog.Core;

namespace api.Services
{
    /*
     * AdminService implements admin command related functionality.
     */
    public class AdminService : IAdminService
    {
        private readonly TtvContext _ttvContext;
        private readonly IOrcidApiService _orcidApiService;
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<AdminService> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private string logPrefix = "AdminService: ";
        private const int delayMillisecondsBetweenOrcidApiRequests = 1000;

        public AdminService(
            TtvContext ttvContext,
            ILogger<AdminService> logger,
            IOrcidApiService orcidApiService,
            IUserProfileService userProfileService,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory)
        {
            _ttvContext = ttvContext;
            _logger = logger;
            _orcidApiService = orcidApiService;
            _userProfileService = userProfileService;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /*
         * Register ORCID webhook for a single user profile.
         */
        public async Task RegisterOrcidWebhookForSingleUserprofile(string webhookOrcidId)
        {
            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogInformation($"{logPrefix}ORCID webhook feature disabled in configuration");
                return;
            }

            // Check that user profile exists
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: webhookOrcidId))
            {
                _logger.LogError($"{logPrefix}ORCID webhook registration failed for {webhookOrcidId}: user profile not found");
                return;
            }

            // Register ORCID webhook.
            try
            {
                bool success = await _orcidApiService.RegisterOrcidWebhook(orcidId: webhookOrcidId);
                if (success)
                {
                    _logger.LogInformation($"{logPrefix}ORCID webhook registered for {webhookOrcidId}");
                }
                else
                {
                    _logger.LogError($"{logPrefix}ORCID webhook registration failed for {webhookOrcidId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logPrefix}ORCID webhook registration failed for {webhookOrcidId}: {ex}");
            }
        }

        /*
         * Unregister ORCID webhook for a single user profile.
         */
        public async Task UnregisterOrcidWebhookForSingleUserprofile(string webhookOrcidId)
        {
            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogInformation($"{logPrefix}ORCID webhook feature disabled in configuration");
                return;
            }

            // Check that user profile exists
            if (!await _userProfileService.UserprofileExistsForOrcidId(orcidId: webhookOrcidId))
            {
                _logger.LogError($"{logPrefix}ORCID webhook registration failed for {webhookOrcidId}: user profile not found");
                return;
            }

            // UnRegister ORCID webhook
            try
            {
                bool success = await _orcidApiService.UnregisterOrcidWebhook(orcidId: webhookOrcidId);
                if (success)
                {
                    _logger.LogInformation($"{logPrefix}ORCID webhook unregistered for {webhookOrcidId}");
                }
                else
                {
                    _logger.LogError($"{logPrefix}ORCID webhook unregistration failed for {webhookOrcidId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logPrefix}ORCID webhook unregistration failed for {webhookOrcidId}: {ex}");
            }
        }

        /*
         * Register ORCID webhook for all user profiles.
         */
        public async Task RegisterOrcidWebhookForAllUserprofiles()
        {
            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogInformation($"{logPrefix}ORCID webhook feature disabled in configuration");
                return;
            }

            // Get all user profiles
            List<DimUserProfile> dimUserProfiles = await _ttvContext.DimUserProfiles.Where(dup => !string.IsNullOrWhiteSpace(dup.OrcidId)).AsNoTracking().ToListAsync();

            // Loop user profiles
            foreach (DimUserProfile dimUserProfile in dimUserProfiles)
            {
                // Register ORCID webhook
                try
                {
                    bool success = await _orcidApiService.RegisterOrcidWebhook(orcidId: dimUserProfile.OrcidId);
                    if (success)
                    {
                        _logger.LogInformation($"{logPrefix}ORCID webhook registered for {dimUserProfile.OrcidId}");
                    }
                    else
                    {
                        _logger.LogError($"{logPrefix}ORCID webhook registration failed for {dimUserProfile.OrcidId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{logPrefix}ORCID webhook registration failed for {dimUserProfile.OrcidId}: {ex}");
                }

                // Prevent flooding ORCID API
                await Task.Delay(delayMillisecondsBetweenOrcidApiRequests);
            }
        }

        /*
         * Unregister ORCID webhook for all user profiles.
         */
        public async Task UnregisterOrcidWebhookForAllUserprofiles()
        {
            // Check that ORCID webhook feature is enabled
            if (!_orcidApiService.IsOrcidWebhookEnabled())
            {
                _logger.LogInformation($"{logPrefix}ORCID webhook feature disabled in configuration");
                return;
            }

            // Get all user profiles
            List<DimUserProfile> dimUserProfiles = await _ttvContext.DimUserProfiles.Where(dup => !string.IsNullOrWhiteSpace(dup.OrcidId)).AsNoTracking().ToListAsync();

            // Loop user profiles
            foreach (DimUserProfile dimUserProfile in dimUserProfiles)
            {
                // Unregister ORCID webhook
                try
                {
                    bool success = await _orcidApiService.UnregisterOrcidWebhook(orcidId: dimUserProfile.OrcidId);
                    if (success)
                    {
                        _logger.LogInformation($"{logPrefix}ORCID webhook unregistered for {dimUserProfile.OrcidId}");
                    }
                    else
                    {
                        _logger.LogError($"{logPrefix}ORCID webhook unregistration failed for {dimUserProfile.OrcidId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{logPrefix}ORCID webhook unregistration failed for {dimUserProfile.OrcidId}: {ex}");
                }

                // Prevent flooding ORCID API
                await Task.Delay(delayMillisecondsBetweenOrcidApiRequests);
            }
        }

        /*
         * Add new TTV data in user profile.
         * This is a background task, therefore all service dependencies must be taken from local scope.
         */
        public async Task<bool> AddNewTtvDataInUserProfileBackground(int dimUserProfileId, LogUserIdentification logUserIdentification)
        {
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                // Create service scope and get required services.
                // Do not use services from controller scope in a background task.
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                TtvContext localTtvContext = scope.ServiceProvider.GetRequiredService<TtvContext>();
                ITtvSqlService localTtvSqlService = scope.ServiceProvider.GetRequiredService<ITtvSqlService>();
                ILogger<UserProfileService> localUserProfileServiceLogger = scope.ServiceProvider.GetRequiredService<ILogger<UserProfileService>>();
                IUserProfileService localUserProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileService>();

                DimUserProfile dimUserProfile = await localTtvContext.DimUserProfiles.Where(dup => dup.Id == dimUserProfileId)
                    .Include(dup => dup.DimKnownPerson)
                        .ThenInclude(dkp => dkp.DimNames)
                            .ThenInclude(dn => dn.DimRegisteredDataSource).AsNoTracking()
                    .Include(dup => dup.DimFieldDisplaySettings).AsNoTracking().FirstOrDefaultAsync();

                if (dimUserProfile == null)
                {
                    // If matching user profile is not found, log error and exit.
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_ADD_TTV_DATA,
                            state: LogContent.ActionState.FAILED,
                            error: true,
                            message: $"{LogContent.ErrorMessage.USER_PROFILE_NOT_FOUND} (dim_user_profile.id={dimUserProfileId})"));
                }
                else
                {
                    // Set ORCID ID in log message
                    logUserIdentification.Orcid = dimUserProfile.OrcidId;

                    // Add TTV data
                    await localUserProfileService.AddTtvDataToUserProfile(
                        dimKnownPerson: dimUserProfile.DimKnownPerson,
                        dimUserProfile: dimUserProfile,
                        logUserIdentification: logUserIdentification);
                }
            });

            return true;
        }
    }
}
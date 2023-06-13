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
using System.Net.Http;
using System.Threading;

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
        private readonly IHttpClientFactory _httpClientFactory;
        private string logPrefix = "AdminService: ";
        private const int delayMillisecondsBetweenOrcidApiRequests = 1000;

        public AdminService(
            TtvContext ttvContext,
            ILogger<AdminService> logger,
            IOrcidApiService orcidApiService,
            IUserProfileService userProfileService,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory,
            IHttpClientFactory httpClientFactory)
        {
            _ttvContext = ttvContext;
            _logger = logger;
            _orcidApiService = orcidApiService;
            _userProfileService = userProfileService;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _httpClientFactory = httpClientFactory;
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



        /*
         * Update user profile in Elasticsearch
         */
        public async Task<bool> UpdateUserprofileInElasticsearch(int dimUserProfileId, LogUserIdentification logUserIdentification)
        {
            // Get DimUserProfile
            DimUserProfile dimUserProfile = await _userProfileService.GetUserprofileById(dimUserProfileId);

            // Check that user profile exists
            if (dimUserProfile == null)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_ELASTICSEARCH_PROFILE_UPDATE,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: $"profile not found (dim_user_profile.id={dimUserProfileId})"));
                return false;
            }

            // Store ORCID ID for background process
            string orcidId = dimUserProfile.OrcidId;
            logUserIdentification.Orcid = orcidId;

            await _userProfileService.UpdateProfileInElasticsearch(
                orcidId: orcidId,
                userprofileId: dimUserProfileId,
                logUserIdentification: logUserIdentification,
                logAction: LogContent.Action.ADMIN_ELASTICSEARCH_PROFILE_UPDATE);

            return true;
        }


        /*
         * Update all user profiles in Elasticsearch.
         * 
         * Get all user profiles and for each of them make an API call to admin API endpoint, which updates a single user profile in Elasticsearch.
         * Implemented using separate HTTP requests because calling methods directly in a loop in a background task causes problems
         * in database context handling.
         */
        public async Task UpdateAllUserprofilesInElasticsearch(LogUserIdentification logUserIdentification, string requestScheme, HostString requestHost)
        {
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_ELASTICSEARCH_PROFILE_UPDATE_ALL,
                        state: LogContent.ActionState.START));

                HttpClient adminApiHttpClient = _httpClientFactory.CreateClient("ADMIN_API");
                adminApiHttpClient.BaseAddress = new Uri($"{requestScheme}://{requestHost}");

                // Create local database context
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                TtvContext localTtvContext = scope.ServiceProvider.GetRequiredService<TtvContext>();

                // Get all user profiles which are not hidden.
                // Update Elasticsearch by calling admin API for each profile.
                List<DimUserProfile> dimUserProfiles = await localTtvContext.DimUserProfiles.Where(dup => dup.Id > 0 && dup.Hidden == false).AsNoTracking().ToListAsync();
                int progressCount = 1;
                foreach (DimUserProfile dimUserProfile in dimUserProfiles)
                {
                    logUserIdentification.Orcid = dimUserProfile.OrcidId;
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ADMIN_ELASTICSEARCH_PROFILE_UPDATE_ALL,
                            state: LogContent.ActionState.IN_PROGRESS,
                            message: $"progress {progressCount}/{dimUserProfiles.Count()}, dim_user_profile.id={dimUserProfile.Id}"));

                    try
                    {
                        HttpRequestMessage request = new(
                            method: HttpMethod.Post,
                            requestUri: $"/admin/elasticsearch/updateprofile/{dimUserProfile.Id}"
                        );
                        HttpResponseMessage response = await adminApiHttpClient.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ADMIN_ELASTICSEARCH_PROFILE_UPDATE_ALL,
                                state: LogContent.ActionState.IN_PROGRESS,
                                message: $"{ex.ToString()}"));
                    }

                    progressCount += 1;
                    // Delay between individual API calls.
                    await Task.Delay(api.Models.Common.Constants.Delays.ADMIN_UPDATE_ALL_PROFILES_IN_ELASTICSEARCH_DELAY_BETWEEN_API_CALLS_MS);
                }

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_ELASTICSEARCH_PROFILE_UPDATE_ALL,
                        state: LogContent.ActionState.COMPLETE));
            });
        }


        /*
         * Update ORCID data for all user profiles.
         * 
         * Get all user profiles and for each of them make an API call to ORCID webhook API endpoint, which updates ORCID data for a single user.
         * Implemented using separate HTTP requests because calling methods directly in a loop in a background task causes problems
         * in database context handling.
         */
        public async Task UpdateOrcidDataForAllUserprofiles(LogUserIdentification logUserIdentification, string requestScheme, HostString requestHost)
        {
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_ORCID_UPDATE_ALL,
                        state: LogContent.ActionState.START));

                HttpClient adminApiHttpClient = _httpClientFactory.CreateClient("ADMIN_API");
                adminApiHttpClient.BaseAddress = new Uri($"{requestScheme}://{requestHost}");

                // Create local database context
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                TtvContext localTtvContext = scope.ServiceProvider.GetRequiredService<TtvContext>();

                // Get all user profiles which are not hidden.
                // Update Elasticsearch by calling admin API for each profile.
                List<DimUserProfile> dimUserProfiles = await localTtvContext.DimUserProfiles.Where(dup => dup.Id > 0).AsNoTracking().ToListAsync();
                int progressCount = 1;
                foreach (DimUserProfile dimUserProfile in dimUserProfiles)
                {
                    logUserIdentification.Orcid = dimUserProfile.OrcidId;
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.ADMIN_ORCID_UPDATE_ALL,
                            state: LogContent.ActionState.IN_PROGRESS,
                            message: $"progress {progressCount}/{dimUserProfiles.Count()}, dim_user_profile.id={dimUserProfile.Id}"));

                    try
                    {
                        HttpRequestMessage request = new(
                            method: HttpMethod.Post,
                            requestUri: $"api/webhook/orcid/{dimUserProfile.OrcidId}"
                        );
                        HttpResponseMessage response = await adminApiHttpClient.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: LogContent.Action.ADMIN_ORCID_UPDATE_ALL,
                                state: LogContent.ActionState.IN_PROGRESS,
                                message: $"{ex.ToString()}"));
                    }

                    progressCount += 1;
                    // Delay between individual API calls.
                    await Task.Delay(api.Models.Common.Constants.Delays.ADMIN_UPDATE_ALL_PROFILES_ORCID_DATA_DELAY_BETWEEN_API_CALLS_MS);
                }

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.ADMIN_ORCID_UPDATE_ALL,
                        state: LogContent.ActionState.COMPLETE));
            });
        }
    }
}
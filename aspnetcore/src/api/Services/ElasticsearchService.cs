using System;
using System.Net.Http;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using api.Models.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace api.Services
{
    /*
     * ElasticsearchService handles person index update.
     */
    public class ElasticsearchService
    {
        public ElasticClient ESclient;
        public IConfiguration _configuration { get; }
        private readonly ILogger<ElasticsearchService> _logger;
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;

        // Check if Elasticsearch synchronization is enabled and related configuration is valid.
        public Boolean IsElasticsearchSyncEnabled()
        {
            return _configuration["ELASTICSEARCH:ENABLED"] != null
                && _configuration["ELASTICSEARCH:ENABLED"] == "true"
                && _configuration["ELASTICSEARCH:URL"] != null
                && Uri.IsWellFormedUriString(
                    _configuration["ELASTICSEARCH:URL"],
                    UriKind.Absolute
                );
        }

        // Constructor.
        // Do not setup HttpClient unless configuration values are ok.
        public ElasticsearchService(TtvContext ttvContext, IConfiguration configuration, HttpClient client, ILogger<ElasticsearchService> logger, UserProfileService userProfileService)
        {
            _configuration = configuration;
            _logger = logger;
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;

            if (this.IsElasticsearchSyncEnabled())
            {
                var settings = new ConnectionSettings(new Uri(_configuration["ELASTICSEARCH:URL"]))
                    .DefaultIndex("person")
                    .BasicAuthentication(_configuration["ELASTICSEARCH:USERNAME"], _configuration["ELASTICSEARCH:PASSWORD"]);
                ESclient = new ElasticClient(settings);
            }
        }
            

        // Update entry in Elasticsearch person index
        // TODO: When 3rd party sharing feature is implemented, check that TTV share is enabled in user profile.
        public async Task UpdateEntryInElasticsearchPersonIndex(string orcidId, int userprofileId)
        {
            if (!this.IsElasticsearchSyncEnabled())
            {
                return;
            }

            // Get DimUserProfile and related entities
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings.Where(dfds => dfds.FactFieldValues.Count > 0 && (dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME || dfds.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS)))
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                        .ThenInclude(br => br.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimName).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimEmailAddrress).AsNoTracking()
                .AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);

            var person = new Person(orcidId)
            {
            };

            // foreach (DimFieldDisplaySetting dfds in dimUserProfile.DimFieldDisplaySettings)
            foreach (DimFieldDisplaySetting dfds in dimUserProfile.DimFieldDisplaySettings)
            {
                // FieldIdentifier defines what type of data the field contains.
                switch (dfds.FieldIdentifier)
                {
                    case Constants.FieldIdentifiers.PERSON_NAME:
                        var nameGroup = new GroupName()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemName>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        {
                            nameGroup.items.Add(
                                new ItemName()
                                {
                                    FirstNames = ffv.DimName.FirstNames,
                                    LastName = ffv.DimName.LastName,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (nameGroup.items.Count > 0)
                        {
                            person.personal.nameGroups.Add(nameGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                        var emailGroup = new GroupEmail()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemEmail>() { },
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            emailGroup.items.Add(
                                new ItemEmail()
                                {
                                    Value = ffv.DimEmailAddrress.Email,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (emailGroup.items.Count > 0)
                        {
                            person.personal.emailGroups.Add(emailGroup);
                        }
                        break;
                }
            }


            var asyncIndexResponse = await ESclient.IndexDocumentAsync(person);

            if (!asyncIndexResponse.IsValid)
            {
                _logger.LogInformation("Elasticsearch: ERROR: " + orcidId + ": " + asyncIndexResponse.OriginalException.Message);
            }
            else
            {
                _logger.LogInformation("Elasticsearch: Updated: " + orcidId);
            }
        }

        // Delete entry from Elasticsearch person index
        public async Task DeleteEntryFromElasticsearchPersonIndex(String orcidId)
        {
            if (!this.IsElasticsearchSyncEnabled())
            {
                return;
            }

            var asyncDeleteResponse = await ESclient.DeleteAsync<ElasticPerson>(orcidId);

            if (!asyncDeleteResponse.IsValid)
            {
                _logger.LogInformation("Elasticsearch: ERROR: " + orcidId + ": " + asyncDeleteResponse.OriginalException.Message);
            }
            else
            {
                _logger.LogInformation("Elasticsearch: Deleted: " + orcidId);
            }
        }
    }
}
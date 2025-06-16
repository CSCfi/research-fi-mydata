using System.Threading.Tasks;
using api.Models.Log;
using api.Models.Ttv;
using api.Models.Common;
using api.Models.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using api.Models.ProfileEditor.Items;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace api.Services
{
    /*
     * BackgroundProfiledata gets user profile data and constructs an entry for Elasticsearch person index.
     *
     * In normal controller code the request context has access to database via ttvContext.
     * In a background task that is not available, since it is disposed when the response is sent.
     * Here a local scope is created and database context can be taken from that scope.
     */
    public class BackgroundProfiledata : IBackgroundProfiledata
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BackgroundProfiledata(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        /*
         * Get userprofile data from TTV database and construct entry for Elasticsearch person index.
         */
        public async Task<ElasticsearchPerson> GetProfiledataForElasticsearch(string orcidId, int userprofileId, LogUserIdentification logUserIdentification)
        {
            // Create a scope and get TtvContext for data query.
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IUserProfileService localUserProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
            IMapper mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            TtvContext localTtvContext = scope.ServiceProvider.GetRequiredService<TtvContext>();

            // Get profile data
            ProfileEditorDataResponse profileEditorDataResponse =
                await localUserProfileService.GetProfileDataAsync(
                    userprofileId: userprofileId,
                    logUserIdentification: logUserIdentification,
                    forElasticsearch: true);

            // Convert profile editor model into Elasticsearch model using Automapper.
            // Set id to ORCID ID
            ElasticsearchPerson elasticsearchPerson = mapper.Map<ElasticsearchPerson>(profileEditorDataResponse);
            elasticsearchPerson.id = orcidId;

            // Add updated timestamp
            DateTimeDTO userProfileModified = await localTtvContext.DimUserProfiles.Where(dup => dup.Id == userprofileId).AsNoTracking().Select(dimUserProfile => new DateTimeDTO()  
                {  
                    Value = dimUserProfile.Modified
                }).FirstOrDefaultAsync();
            elasticsearchPerson.updated = userProfileModified.Value;

            return elasticsearchPerson;
        }
    }
}
using api.Models.ProfileEditor.Items;
using api.Models.Elasticsearch;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProfileEditorDataResponse, ElasticsearchPerson>();
        CreateMap<ProfileEditorDataPersonal, ElasticsearchPersonal>();
        CreateMap<ProfileEditorDataActivity, ElasticsearchActivity>();
        CreateMap<ProfileEditorSource, ElasticsearchSource>();
        CreateMap<ProfileEditorItem, ElasticsearchItem>();
        CreateMap<ProfileEditorItemMeta, ElasticsearchItemMeta>();
        CreateMap<ProfileEditorAffiliation, ElasticsearchAffiliation>();
        CreateMap<ProfileEditorSector, ElasticsearchSector>();
        CreateMap<ProfileEditorSectorOrganization, ElasticsearchSectorOrganization>();
        CreateMap<ProfileEditorEducation, ElasticsearchEducation>();
        CreateMap<ProfileEditorDate, ElasticsearchDate>();
        CreateMap<ProfileEditorEmail, ElasticsearchEmail>();
        CreateMap<ProfileEditorExternalIdentifier, ElasticsearchExternalIdentifier>();
        CreateMap<ProfileEditorFieldOfScience, ElasticsearchFieldOfScience>();
        CreateMap<ProfileEditorFundingDecision, ElasticsearchFundingDecision>();
        CreateMap<ProfileEditorKeyword, ElasticsearchKeyword>();
        CreateMap<ProfileEditorName, ElasticsearchName>();
        CreateMap<ProfileEditorPublication, ElasticsearchPublication>();
        CreateMap<ProfileEditorResearchDataset, ElasticsearchResearchDataset>();
        CreateMap<ProfileEditorResearcherDescription, ElasticsearchResearcherDescription>();
        CreateMap<ProfileEditorTelephoneNumber, ElasticsearchTelephoneNumber>();
        CreateMap<ProfileEditorWebLink, ElasticsearchWebLink>();
        CreateMap<ProfileEditorActor, ElasticsearchActor>();
        CreateMap<ProfileEditorPreferredIdentifier, ElasticsearchPreferredIdentifier>();
        CreateMap<ProfileEditorActivityAndReward, ElasticsearchActivityAndReward>();
    }
}
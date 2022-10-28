using api.Models.ProfileEditor;
using api.Models.Elasticsearch;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProfileEditorDataResponse2, ElasticsearchPerson>();
        CreateMap<ProfileEditorDataPersonal2, ElasticsearchPersonal>();
        CreateMap<ProfileEditorDataActivity2, ElasticsearchActivity>();
        CreateMap<ProfileEditorSource, ElasticsearchSource>();
        CreateMap<ProfileEditorItem2, ElasticsearchItem>();
        CreateMap<ProfileEditorItemMeta, ElasticsearchItemMeta>();
        CreateMap<ProfileEditorAffiliation, ElasticsearchAffiliation>();
        CreateMap<ProfileEditorEducation, ElasticsearchEducation>();
        CreateMap<ProfileEditorItemDate, ElasticsearchDate>();
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
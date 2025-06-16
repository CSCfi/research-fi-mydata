using api.Models.ProfileEditor.Items;
using api.Models.Elasticsearch;
using AutoMapper;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using System.Linq;

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
        CreateMap<ProfileEditorPublicationPeerReviewed, ElasticsearchPublicationPeerReviewed>();
        CreateMap<ProfileEditorResearchDataset, ElasticsearchResearchDataset>();
        CreateMap<ProfileEditorResearcherDescription, ElasticsearchResearcherDescription>();
        CreateMap<ProfileEditorTelephoneNumber, ElasticsearchTelephoneNumber>();
        CreateMap<ProfileEditorWebLink, ElasticsearchWebLink>();
        CreateMap<ProfileEditorActor, ElasticsearchActor>();
        CreateMap<ProfileEditorPreferredIdentifier, ElasticsearchPreferredIdentifier>();
        CreateMap<ProfileEditorActivityAndReward, ElasticsearchActivityAndReward>();
        CreateMap<DimReferencedatum, ProfileEditorCooperationItem>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.DimUserChoices.First().Id))
            .ForMember(dst => dst.Selected, opt => opt.MapFrom(src => src.DimUserChoices.First().UserChoiceValue));

        CreateMap<DimUserChoice, ElasticsearchCooperation>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.DimReferencedataIdAsUserChoiceLabelNavigation.Id))
            .ForMember(dst => dst.NameFi, opt => opt.MapFrom(src => src.DimReferencedataIdAsUserChoiceLabelNavigation.NameFi))
            .ForMember(dst => dst.NameEn, opt => opt.MapFrom(src => src.DimReferencedataIdAsUserChoiceLabelNavigation.NameEn))
            .ForMember(dst => dst.NameSv, opt => opt.MapFrom(src => src.DimReferencedataIdAsUserChoiceLabelNavigation.NameSv))
            .ForMember(dst => dst.Order, opt => opt.MapFrom(src => src.DimReferencedataIdAsUserChoiceLabelNavigation.Order));
        CreateMap<ProfileSettings, ElasticsearchProfileSettings>();
        CreateMap<ProfileEditorCooperationItem, ElasticsearchCooperation>();
    }
}
using api.Models.ProfileEditor.Items;
using api.Models.Elasticsearch;
using api.Models.ProfileEditor;
using System.Linq;
using System.Collections.Generic;

namespace api.CustomMapper;
public static class ElasticsearchMapper
{
    public static ElasticsearchPerson MapToElasticsearchPerson(ProfileEditorDataResponse src, string orcidId = null)
    {
        if (src == null) return null;
        return new ElasticsearchPerson(orcidId)
        {
            id = orcidId ?? "",
            personal = MapToElasticsearchPersonal(src.personal),
            activity = MapToElasticsearchActivity(src.activity),
            settings = MapToElasticsearchProfileSettings(src.settings),
            cooperation = MapToElasticsearchCooperation(src.cooperation),
            uniqueDataSources = MapToElasticsearchSource(src.uniqueDataSources)
        };
    }
    public static ElasticsearchPersonal MapToElasticsearchPersonal(ProfileEditorDataPersonal src)
    {
        if (src == null) return null;
        return new ElasticsearchPersonal
        {
            names = MapToElasticsearchPersonal_Names(src.names),
            otherNames = MapToElasticsearchPersonal_OtherNames(src.otherNames),
            emails = MapToElasticsearchPersonal_Emails(src.emails),
            telephoneNumbers = MapToElasticsearchPersonal_TelephoneNumbers(src.telephoneNumbers),
            webLinks = MapToElasticsearchPersonal_WebLinks(src.webLinks),
            keywords = MapToElasticsearchPersonal_Keywords(src.keywords),
            fieldOfSciences = MapToElasticsearchPersonal_FieldOfSciences(src.fieldOfSciences),
            researcherDescriptions = MapToElasticsearchPersonal_ResearcherDescriptions(src.researcherDescriptions),
            externalIdentifiers = MapToElasticsearchPersonal_ExternalIdentifiers(src.externalIdentifiers)
        };
    }

    public static ElasticsearchActivity MapToElasticsearchActivity(ProfileEditorDataActivity src)
    {
        if (src == null) return null;
        return new ElasticsearchActivity
        {
            affiliations = MapToElasticsearchActivity_Affiliations(src.affiliations),
            educations = MapToElasticsearchActivity_Educations(src.educations),
            publications = MapToElasticsearchActivity_Publications(src.publications),
            fundingDecisions = MapToElasticsearchActivity_FundingDecisions(src.fundingDecisions),
            researchDatasets = MapToElasticsearchActivity_ResearchDatasets(src.researchDatasets),
            activitiesAndRewards = MapToElasticsearchActivity_ActivitiesAndRewards(src.activitiesAndRewards)
        };
    }

    public static List<ElasticsearchName> MapToElasticsearchPersonal_Names(List<ProfileEditorName> src)
    {
        return src?.Select(name => new ElasticsearchName
        {
            FirstNames = name.FirstNames,
            LastName = name.LastName,
            FullName = name.FullName
        }).ToList();
    }

    public static List<ElasticsearchName> MapToElasticsearchPersonal_OtherNames(List<ProfileEditorName> src)
    {
        return src?.Select(name => new ElasticsearchName
        {
            FirstNames = name.FirstNames,
            LastName = name.LastName,
            FullName = name.FullName
        }).ToList();
    }

    public static List<ElasticsearchEmail> MapToElasticsearchPersonal_Emails(List<ProfileEditorEmail> src)
    {
        return src?.Select(email => new ElasticsearchEmail
        {
            Value = email.Value
        }).ToList();
    }

    public static List<ElasticsearchTelephoneNumber> MapToElasticsearchPersonal_TelephoneNumbers(List<ProfileEditorTelephoneNumber> src)
    {
        return src?.Select(tel => new ElasticsearchTelephoneNumber
        {
            Value = tel.Value
        }).ToList();
    }

    public static List<ElasticsearchWebLink> MapToElasticsearchPersonal_WebLinks(List<ProfileEditorWebLink> src)
    {
        return src?.Select(link => new ElasticsearchWebLink
        {
            Url = link.Url,
            LinkLabel = link.LinkLabel
        }).ToList();
    }

    public static List<ElasticsearchKeyword> MapToElasticsearchPersonal_Keywords(List<ProfileEditorKeyword> src)
    {
        return src?.Select(keyword => new ElasticsearchKeyword
        {
            Value = keyword.Value
        }).ToList();
    }

    public static List<ElasticsearchFieldOfScience> MapToElasticsearchPersonal_FieldOfSciences(List<ProfileEditorFieldOfScience> src)
    {
        return src?.Select(fos => new ElasticsearchFieldOfScience
        {
            NameFi = fos.NameFi,
            NameEn = fos.NameEn,
            NameSv = fos.NameSv
        }).ToList();
    }

    public static List<ElasticsearchResearcherDescription> MapToElasticsearchPersonal_ResearcherDescriptions(List<ProfileEditorResearcherDescription> src)
    {
        return src?.Select(desc => new ElasticsearchResearcherDescription
        {
            ResearchDescriptionFi = desc.ResearchDescriptionFi,
            ResearchDescriptionEn = desc.ResearchDescriptionEn,
            ResearchDescriptionSv = desc.ResearchDescriptionSv
        }).ToList();
    }

    public static List<ElasticsearchExternalIdentifier> MapToElasticsearchPersonal_ExternalIdentifiers(List<ProfileEditorExternalIdentifier> src)
    {
        return src?.Select(extId => new ElasticsearchExternalIdentifier
        {
            PidContent = extId.PidContent,
            PidType = extId.PidType
        }).ToList();
    }

    public static ElasticsearchProfileSettings MapToElasticsearchProfileSettings(ProfileSettings src)
    {
        if (src == null) return null;
        return new ElasticsearchProfileSettings
        {
            HighlightOpeness = src.HighlightOpeness
        };
    }

    public static List<ElasticsearchCooperation> MapToElasticsearchCooperation(List<ProfileEditorCooperationItem> src)
    {
        if (src == null) return null;
        return src
            .Where(item => item.Selected) // Include only selected items
            .Select(item => new ElasticsearchCooperation
            {
                Id = item.Id,
                NameFi = item.NameFi,
                NameEn = item.NameEn,
                NameSv = item.NameSv,
                Order = item.Order
            }).ToList();
    }

    public static List<ElasticsearchSource> MapToElasticsearchSource(List<ProfileEditorSource> src)
    {
        if (src == null) return null;
        return src?.Select(ds => new ElasticsearchSource
        {
            RegisteredDataSource = ds.RegisteredDataSource,
            Organization = ds.Organization
        }).ToList();
    }

    public static ElasticsearchEmail MapToElasticsearchEmail(ProfileEditorEmail src)
    {
        if (src == null) return null;
        return new ElasticsearchEmail
        {
            Value = src.Value
        };
    }

    public static List<ElasticsearchAffiliation> MapToElasticsearchActivity_Affiliations(List<ProfileEditorAffiliation> src)
    {
        return src?.Select(aff => new ElasticsearchAffiliation
        {
            OrganizationNameFi = aff.OrganizationNameFi,
            OrganizationNameSv = aff.OrganizationNameSv,
            OrganizationNameEn = aff.OrganizationNameEn,
            DepartmentNameFi = aff.DepartmentNameFi,
            DepartmentNameSv = aff.DepartmentNameSv,
            DepartmentNameEn = aff.DepartmentNameEn,
            PositionNameFi = aff.PositionNameFi,
            PositionNameSv = aff.PositionNameSv,
            PositionNameEn = aff.PositionNameEn,
            AffiliationTypeFi = aff.AffiliationTypeFi,
            AffiliationTypeEn = aff.AffiliationTypeEn,
            AffiliationTypeSv = aff.AffiliationTypeSv,
            StartDate = MapToElasticsearchDate(aff.StartDate),
            EndDate = MapToElasticsearchDate(aff.EndDate),
            sector = MapToElasticsearchSector(aff.sector)
        }).ToList();
    }

    public static List<ElasticsearchEducation> MapToElasticsearchActivity_Educations(List<ProfileEditorEducation> src)
    {
        return src?.Select(edu => new ElasticsearchEducation
        {
            NameFi = edu.NameFi,
            NameEn = edu.NameEn,
            NameSv = edu.NameSv,
            DegreeGrantingInstitutionName = edu.DegreeGrantingInstitutionName,
            StartDate = MapToElasticsearchDate(edu.StartDate),
            EndDate = MapToElasticsearchDate(edu.EndDate)
        }).ToList();
    }

    public static List<ElasticsearchPublication> MapToElasticsearchActivity_Publications(List<ProfileEditorPublication> src)
    {
        return src?.Select(pub => new ElasticsearchPublication
        {
            AuthorsText = pub.AuthorsText,
            Doi = pub.Doi,
            ConferenceName = pub.ConferenceName,
            JournalName = pub.JournalName,
            OpenAccess = pub.OpenAccess,
            ParentPublicationName = pub.ParentPublicationName,
            PublicationId = pub.PublicationId,
            PublicationName = pub.PublicationName,
            PublicationTypeCode = pub.PublicationTypeCode,
            PublicationYear = pub.PublicationYear,
            SelfArchivedAddress = pub.SelfArchivedAddress,
            SelfArchivedCode = pub.SelfArchivedCode
        }).ToList();
    }

    public static List<ElasticsearchFundingDecision> MapToElasticsearchActivity_FundingDecisions(List<ProfileEditorFundingDecision> src)
    {
        return src?.Select(fd => new ElasticsearchFundingDecision
        {
            ProjectAcronym = fd.ProjectAcronym,
            ProjectNameFi = fd.ProjectNameFi,
            ProjectNameSv = fd.ProjectNameSv,
            ProjectNameEn = fd.ProjectNameEn,
            ProjectDescriptionFi = fd.ProjectDescriptionFi,
            ProjectDescriptionSv = fd.ProjectDescriptionSv,
            ProjectDescriptionEn = fd.ProjectDescriptionEn,
            FunderNameFi = fd.FunderNameFi,
            FunderNameSv = fd.FunderNameSv,
            FunderNameEn = fd.FunderNameEn,
            FunderProjectNumber = fd.FunderProjectNumber,
            TypeOfFundingNameFi = fd.TypeOfFundingNameFi,
            TypeOfFundingNameSv = fd.TypeOfFundingNameSv,
            TypeOfFundingNameEn = fd.TypeOfFundingNameEn,
            CallProgrammeNameFi = fd.CallProgrammeNameFi,
            CallProgrammeNameSv = fd.CallProgrammeNameSv,
            CallProgrammeNameEn = fd.CallProgrammeNameEn,
            FundingStartYear = fd.FundingStartYear,
            FundingEndYear = fd.FundingEndYear,
            AmountInEur = fd.AmountInEur,
            AmountInFundingDecisionCurrency = fd.AmountInFundingDecisionCurrency,
            FundingDecisionCurrencyAbbreviation = fd.FundingDecisionCurrencyAbbreviation,
            Url = fd.Url
        }).ToList();
    }

    public static List<ElasticsearchResearchDataset> MapToElasticsearchActivity_ResearchDatasets(List<ProfileEditorResearchDataset> src)
    {
        return src?.Select(ds => new ElasticsearchResearchDataset
        {
            AccessType = ds.AccessType,
            Actor = MapToElasticsearchActor(ds.Actor),
            FairdataUrl = ds.FairdataUrl,
            Identifier = ds.Identifier,
            NameFi = ds.NameFi,
            NameSv = ds.NameSv,
            NameEn = ds.NameEn,
            DescriptionFi = ds.DescriptionFi,
            DescriptionSv = ds.DescriptionSv,
            DescriptionEn = ds.DescriptionEn,
            Url = ds.Url,
            DatasetCreated = ds.DatasetCreated,
            PreferredIdentifiers = MapToElasticsearchPreferredIdentifier(ds.PreferredIdentifiers)
        }).ToList();
    }

    public static List<ElasticsearchActivityAndReward> MapToElasticsearchActivity_ActivitiesAndRewards(List<ProfileEditorActivityAndReward> src)
    {
        return src?.Select(ar => new ElasticsearchActivityAndReward
        {
            NameFi = ar.NameFi,
            NameEn = ar.NameEn,
            NameSv = ar.NameSv,
            DescriptionFi = ar.DescriptionFi,
            DescriptionEn = ar.DescriptionEn,
            DescriptionSv = ar.DescriptionSv,
            InternationalCollaboration = ar.InternationalCollaboration,
            StartDate = MapToElasticsearchDate(ar.StartDate),
            EndDate = MapToElasticsearchDate(ar.EndDate),
            ActivityTypeCode = ar.ActivityTypeCode,
            ActivityTypeNameFi = ar.ActivityTypeNameFi,
            ActivityTypeNameEn = ar.ActivityTypeNameEn,
            ActivityTypeNameSv = ar.ActivityTypeNameSv,
            RoleCode = ar.RoleCode,
            RoleNameFi = ar.RoleNameFi,
            RoleNameEn = ar.RoleNameEn,
            RoleNameSv = ar.RoleNameSv,
            OrganizationNameFi = ar.OrganizationNameFi,
            OrganizationNameSv = ar.OrganizationNameSv,
            OrganizationNameEn = ar.OrganizationNameEn,
            DepartmentNameFi = ar.DepartmentNameFi,
            DepartmentNameSv = ar.DepartmentNameSv,
            DepartmentNameEn = ar.DepartmentNameEn,
            Url = ar.Url,
            sector = MapToElasticsearchSector(ar.sector)
        }).ToList();
    }

    public static ElasticsearchDate MapToElasticsearchDate(ProfileEditorDate src)
    {
        if (src == null) return null;
        return new ElasticsearchDate
        {
            Year = src.Year,
            Month = src.Month,
            Day = src.Day
        };
    }

    public static List<ElasticsearchSector> MapToElasticsearchSector(List<ProfileEditorSector> src)
    {
        if (src == null) return null;
        return src.Select(sector => new ElasticsearchSector
        {
            sectorId = sector.sectorId,
            nameFiSector = sector.nameFiSector,
            nameEnSector = sector.nameEnSector,
            nameSvSector = sector.nameSvSector,
            organization = sector.organization?.Select(MapToElasticsearchSectorOrganization).ToList() ?? new List<ElasticsearchSectorOrganization>()
        }).ToList();
    }

    public static ElasticsearchSectorOrganization MapToElasticsearchSectorOrganization(ProfileEditorSectorOrganization src)
    {
        if (src == null) return null;
        return new ElasticsearchSectorOrganization
        {
            organizationId = src.organizationId,
            OrganizationNameFi = src.OrganizationNameFi,
            OrganizationNameEn = src.OrganizationNameEn,
            OrganizationNameSv = src.OrganizationNameSv
        };
    }

    public static List<ElasticsearchActor> MapToElasticsearchActor(List<ProfileEditorActor> src)
    {
        if (src == null) return null;
        return src.Select(actor => new ElasticsearchActor
        {
            actorRole = actor.actorRole,
            actorRoleNameFi = actor.actorRoleNameFi,
            actorRoleNameSv = actor.actorRoleNameSv,
            actorRoleNameEn = actor.actorRoleNameEn
        }).ToList();
    }

    public static List<ElasticsearchPreferredIdentifier> MapToElasticsearchPreferredIdentifier(List<ProfileEditorPreferredIdentifier> src)
    {
        if (src == null) return null;
        return src.Select(item => new ElasticsearchPreferredIdentifier
        {
            PidContent = item.PidContent,
            PidType = item.PidType
        }).ToList();
    }
}
using api.Models.ProfileEditor.Items;
using api.Models.Elasticsearch;
using api.Models.ProfileEditor;
using System.Linq;
using System.Collections.Generic;

namespace api.CustomMapper;

// ElasticsearchMapper maps profile editor model to Elasticsearch model.
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

    public static ElasticsearchItemMeta MapToElasticsearchItemMeta(ProfileEditorItemMeta src)
    {
        if (src == null) return null;
        return new ElasticsearchItemMeta
        {
            PrimaryValue = src.PrimaryValue ?? false
        };
    }

    public static List<ElasticsearchName> MapToElasticsearchPersonal_Names(List<ProfileEditorName> src)
    {
        return src?
            .Where(name => name.itemMeta != null && name.itemMeta.Show == true)
            .Select(name => new ElasticsearchName
            {
                FirstNames = name.FirstNames,
                LastName = name.LastName,
                FullName = name.FullName,
                itemMeta = MapToElasticsearchItemMeta(name.itemMeta),
                DataSources = MapToElasticsearchSource(name.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchName> MapToElasticsearchPersonal_OtherNames(List<ProfileEditorName> src)
    {
        return src?
            .Where(otherName => otherName.itemMeta != null && otherName.itemMeta.Show == true)
            .Select(otherName => new ElasticsearchName
            {
                FirstNames = otherName.FirstNames,
                LastName = otherName.LastName,
                FullName = otherName.FullName,
                itemMeta = MapToElasticsearchItemMeta(otherName.itemMeta),
                DataSources = MapToElasticsearchSource(otherName.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchEmail> MapToElasticsearchPersonal_Emails(List<ProfileEditorEmail> src)
    {
        return src?
            .Where(email => email.itemMeta != null && email.itemMeta.Show == true)
            .Select(email => new ElasticsearchEmail
            {
                Value = email.Value,
                itemMeta = MapToElasticsearchItemMeta(email.itemMeta),
                DataSources = MapToElasticsearchSource(email.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchTelephoneNumber> MapToElasticsearchPersonal_TelephoneNumbers(List<ProfileEditorTelephoneNumber> src)
    {
        return src?
            .Where(tel => tel.itemMeta != null && tel.itemMeta.Show == true)
            .Select(tel => new ElasticsearchTelephoneNumber
            {
                Value = tel.Value,
                itemMeta = MapToElasticsearchItemMeta(tel.itemMeta),
                DataSources = MapToElasticsearchSource(tel.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchWebLink> MapToElasticsearchPersonal_WebLinks(List<ProfileEditorWebLink> src)
    {
        return src?
            .Where(link => link.itemMeta != null && link.itemMeta.Show == true)
            .Select(link => new ElasticsearchWebLink
            {
                Url = link.Url,
                LinkLabel = link.LinkLabel,
                itemMeta = MapToElasticsearchItemMeta(link.itemMeta),
                DataSources = MapToElasticsearchSource(link.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchKeyword> MapToElasticsearchPersonal_Keywords(List<ProfileEditorKeyword> src)
    {
        return src?
            .Where(keyword => keyword.itemMeta != null && keyword.itemMeta.Show == true)
            .Select(keyword => new ElasticsearchKeyword
            {
                Value = keyword.Value,
                itemMeta = MapToElasticsearchItemMeta(keyword.itemMeta),
                DataSources = MapToElasticsearchSource(keyword.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchFieldOfScience> MapToElasticsearchPersonal_FieldOfSciences(List<ProfileEditorFieldOfScience> src)
    {
        return src?
            .Where(fos => fos.itemMeta != null && fos.itemMeta.Show == true)
            .Select(fos => new ElasticsearchFieldOfScience
            {
                NameFi = fos.NameFi,
                NameEn = fos.NameEn,
                NameSv = fos.NameSv,
                itemMeta = MapToElasticsearchItemMeta(fos.itemMeta),
                DataSources = MapToElasticsearchSource(fos.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchResearcherDescription> MapToElasticsearchPersonal_ResearcherDescriptions(List<ProfileEditorResearcherDescription> src)
    {
        return src?
            .Where(desc => desc.itemMeta != null && desc.itemMeta.Show == true)
            .Select(desc => new ElasticsearchResearcherDescription
            {
                ResearchDescriptionFi = desc.ResearchDescriptionFi,
                ResearchDescriptionEn = desc.ResearchDescriptionEn,
                ResearchDescriptionSv = desc.ResearchDescriptionSv,
                itemMeta = MapToElasticsearchItemMeta(desc.itemMeta),
                DataSources = MapToElasticsearchSource(desc.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchExternalIdentifier> MapToElasticsearchPersonal_ExternalIdentifiers(List<ProfileEditorExternalIdentifier> src)
    {
        return src?
            .Where(extId => extId.itemMeta != null && extId.itemMeta.Show == true)
            .Select(extId => new ElasticsearchExternalIdentifier
            {
                PidContent = extId.PidContent,
                PidType = extId.PidType,
                itemMeta = MapToElasticsearchItemMeta(extId.itemMeta),
                DataSources = MapToElasticsearchSource(extId.DataSources)
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
            Value = src.Value,
            itemMeta = MapToElasticsearchItemMeta(src.itemMeta)
        };
    }

    public static List<ElasticsearchAffiliation> MapToElasticsearchActivity_Affiliations(List<ProfileEditorAffiliation> src)
    {
        return src?
            .Where(aff => aff.itemMeta != null && aff.itemMeta.Show == true)
            .Select(aff => new ElasticsearchAffiliation
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
                sector = MapToElasticsearchSector(aff.sector),
                itemMeta = MapToElasticsearchItemMeta(aff.itemMeta),
                DataSources = MapToElasticsearchSource(aff.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchEducation> MapToElasticsearchActivity_Educations(List<ProfileEditorEducation> src)
    {
        return src?
            .Where(edu => edu.itemMeta != null && edu.itemMeta.Show == true)
            .Select(edu => new ElasticsearchEducation
            {
                NameFi = edu.NameFi,
                NameEn = edu.NameEn,
                NameSv = edu.NameSv,
                DegreeGrantingInstitutionName = edu.DegreeGrantingInstitutionName,
                StartDate = MapToElasticsearchDate(edu.StartDate),
                EndDate = MapToElasticsearchDate(edu.EndDate),
                itemMeta = MapToElasticsearchItemMeta(edu.itemMeta),
                DataSources = MapToElasticsearchSource(edu.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchPublication> MapToElasticsearchActivity_Publications(List<ProfileEditorPublication> src)
    {
        return src?
            .Where(pub => pub.itemMeta != null && pub.itemMeta.Show == true)
            .Select(pub => new ElasticsearchPublication
            {
                AuthorsText = pub.AuthorsText,
                Doi = pub.Doi,
                ConferenceName = pub.ConferenceName,
                JournalName = pub.JournalName,
                OpenAccess = pub.OpenAccess,
                ParentPublicationName = pub.ParentPublicationName,
                PeerReviewed = pub.PeerReviewed.Select(pr => new ElasticsearchPublicationPeerReviewed
                {
                    Id = pr.Id,
                    NameFiPeerReviewed = pr.NameFiPeerReviewed,
                    NameSvPeerReviewed = pr.NameSvPeerReviewed,
                    NameEnPeerReviewed = pr.NameEnPeerReviewed
                }).ToList(),
                PublicationId = pub.PublicationId,
                PublicationName = pub.PublicationName,
                PublicationTypeCode = pub.PublicationTypeCode,
                PublicationYear = pub.PublicationYear,
                SelfArchivedAddress = pub.SelfArchivedAddress,
                SelfArchivedCode = pub.SelfArchivedCode,
                itemMeta = MapToElasticsearchItemMeta(pub.itemMeta),
                DataSources = MapToElasticsearchSource(pub.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchFundingDecision> MapToElasticsearchActivity_FundingDecisions(List<ProfileEditorFundingDecision> src)
    {
        return src?
            .Where(fd => fd.itemMeta != null && fd.itemMeta.Show == true)
            .Select(fd => new ElasticsearchFundingDecision
            {
                ProjectId = fd.ProjectId,
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
                Url = fd.Url,
                itemMeta = MapToElasticsearchItemMeta(fd.itemMeta),
                DataSources = MapToElasticsearchSource(fd.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchResearchDataset> MapToElasticsearchActivity_ResearchDatasets(List<ProfileEditorResearchDataset> src)
    {
        return src?
            .Where(ds => ds.itemMeta != null && ds.itemMeta.Show == true)
            .Select(ds => new ElasticsearchResearchDataset
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
                PreferredIdentifiers = MapToElasticsearchPreferredIdentifier(ds.PreferredIdentifiers),
                itemMeta = MapToElasticsearchItemMeta(ds.itemMeta),
                DataSources = MapToElasticsearchSource(ds.DataSources)
            }).ToList();
    }

    public static List<ElasticsearchActivityAndReward> MapToElasticsearchActivity_ActivitiesAndRewards(List<ProfileEditorActivityAndReward> src)
    {
        return src?
            .Where(ar => ar.itemMeta != null && ar.itemMeta.Show == true)
            .Select(ar => new ElasticsearchActivityAndReward
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
                sector = MapToElasticsearchSector(ar.sector),
                itemMeta = MapToElasticsearchItemMeta(ar.itemMeta),
                DataSources = MapToElasticsearchSource(ar.DataSources)
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
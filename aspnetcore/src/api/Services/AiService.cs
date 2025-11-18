using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using api.Models.Ai;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace api.Services
{
    public class AiService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILogger<UserProfileService> _logger;


        public AiService(
            TtvContext ttvContext,
            ILogger<UserProfileService> logger)
        {
            _ttvContext = ttvContext;
            _logger = logger;
        }

        public async Task<string?> GetProfileDataForPromt(string orcidId)
        {
            AittaModel aittaModel = new AittaModel();

            // DimName
            aittaModel.PersonName = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimName != null && ffv.DimNameId > 0 && ffv.Show == true)
                .Include(ffv => ffv.DimName)
                .Select(ffv => ffv.DimName.FirstNames + " " + ffv.DimName.LastName)
                .AsNoTracking().FirstOrDefaultAsync();

            // DimResearcherDescription
            aittaModel.ResearcherDescription = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimResearcherDescription != null && ffv.DimResearcherDescriptionId > 0 && ffv.Show == true)
                .Include(ffv => ffv.DimResearcherDescription)
                .Select(ffv => LanguageFilter(ffv.DimResearcherDescription.ResearchDescriptionEn, ffv.DimResearcherDescription.ResearchDescriptionFi, ffv.DimResearcherDescription.ResearchDescriptionSv))
                .AsNoTracking().ToListAsync();

            // DimAffiliation
            List<FactFieldValue> affiliationFfvs = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimAffiliation != null && ffv.DimAffiliationId > 0 && ffv.Show == true)
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.DimOrganization)
                        .ThenInclude(aff_org => aff_org.DimOrganizationBroaderNavigation)
                .Include(ffv => ffv.DimIdentifierlessData)
                    .ThenInclude(di => di.InverseDimIdentifierlessData)
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.StartDateNavigation)
                .Include(ffv => ffv.DimAffiliation)
                    .ThenInclude(affiliation => affiliation.EndDateNavigation)
                .AsNoTracking().ToListAsync();
            foreach (FactFieldValue ffv in affiliationFfvs)
            {
                // Affiliation organization
                // 1. dim_affiliation.dim_organization_id
                // 2. fact_field_values.dim_identifierless_data_id

                AittaOrganization affiliationOrganization = new();

                if (ffv.DimAffiliation.DimOrganization != null && ffv.DimAffiliation.DimOrganizationId > 0)
                {
                    // Organization from dim_affiliation.dim_organization_id
                    affiliationOrganization.OrganizationName =
                        LanguageFilter(
                            en: ffv.DimAffiliation.DimOrganization.NameEn,
                            fi: ffv.DimAffiliation.DimOrganization.NameFi,
                            sv: ffv.DimAffiliation.DimOrganization.NameSv
                        );
                    if (ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation != null && ffv.DimAffiliation.DimOrganization.DimOrganizationBroader > 0)
                    {
                        affiliationOrganization.IsPartOfOrganization = new AittaOrganization
                        {
                            OrganizationName = LanguageFilter(
                                en: ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.NameEn,
                                fi: ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.NameFi,
                                sv: ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.NameSv
                            )
                        };
                    }
                    else
                    {
                        affiliationOrganization.IsPartOfOrganization = null;
                    }
                }
                else if (ffv.DimIdentifierlessData != null && ffv.DimIdentifierlessDataId > 0)
                {
                    // Organization from fact_field_values.dim_identifierless_data_id
                    affiliationOrganization = AittaOrganizationFromIdentifierlessData(ffv.DimIdentifierlessData);
                }

                aittaModel.HasAffiliation.Add(new AittaAffiliation
                {
                    AffiliationType = LanguageFilter(ffv.DimAffiliation.AffiliationTypeEn, ffv.DimAffiliation.AffiliationTypeFi, ffv.DimAffiliation.AffiliationTypeSv),
                    PositionTitle = LanguageFilter(ffv.DimAffiliation.PositionNameEn, ffv.DimAffiliation.PositionNameFi, ffv.DimAffiliation.PositionNameSv),
                    Organization = affiliationOrganization,
                    StartsOn = ffv.DimAffiliation.StartDateNavigation != null && ffv.DimAffiliation.StartDate > 0 && ffv.DimAffiliation.StartDateNavigation.Year > 0 ?
                        new AittaDate { Year = ffv.DimAffiliation.StartDateNavigation.Year, Month = ffv.DimAffiliation.StartDateNavigation.Month } : null,
                    EndsOn = ffv.DimAffiliation.EndDateNavigation != null && ffv.DimAffiliation.EndDate > 0 && ffv.DimAffiliation.EndDateNavigation.Year > 0 ?
                        new AittaDate { Year = ffv.DimAffiliation.EndDateNavigation.Year, Month = ffv.DimAffiliation.EndDateNavigation.Month } : null
                });
            }

            // DimEducation
            aittaModel.HasCompleted = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimEducation != null && ffv.DimEducationId > 0 && ffv.Show == true)
                .Include(ffv => ffv.DimEducation)
                .Select(ffv => new AittaEducation
                {
                    EducationName = LanguageFilter(ffv.DimEducation.NameEn, ffv.DimEducation.NameFi, ffv.DimEducation.NameSv),
                    DegreeGrantingInstitution = !string.IsNullOrWhiteSpace(ffv.DimEducation.DegreeGrantingInstitutionName) ? ffv.DimEducation.DegreeGrantingInstitutionName : null
                })
                .AsNoTracking().ToListAsync();

            // DimPublication
            aittaModel.UserParticipatedPublication = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimPublication != null && ffv.DimPublicationId > 0 && ffv.Show == true)
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.DimKeywords)
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.DimDescriptiveItems)
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.FactDimReferencedataFieldOfSciences)
                        .ThenInclude(fdrfs => fdrfs.DimReferencedata)
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.PublicationTypeCodeNavigation)
                .Include(ffv => ffv.DimPublication)
                    .ThenInclude(pub => pub.TargetAudienceCodeNavigation)
                .Select(ffv => new AittaPublication
                {
                    Name = !string.IsNullOrWhiteSpace(ffv.DimPublication.PublicationName) ? ffv.DimPublication.PublicationName : null,
                    Year = ffv.DimPublication.PublicationYear > 0 ? ffv.DimPublication.PublicationYear : null,
                    //Abstract = ffv.DimPublication.DimDescriptiveItems.Where(di => di.DescriptiveItemType == "Abstract").Select(di => GetFirstNSentences(di.DescriptiveItem, 1)).FirstOrDefault(),
                    Keywords = ffv.DimPublication.DimKeywords.Count > 0 ? ffv.DimPublication.DimKeywords.Select(kw => kw.Keyword).ToList() : null,
                    FieldsOfScience = ffv.DimPublication.FactDimReferencedataFieldOfSciences.Select(fdrfs => fdrfs.DimReferencedata.NameEn).ToList(),
                    Type = ffv.DimPublication.PublicationTypeCodeNavigation != null ? ffv.DimPublication.PublicationTypeCodeNavigation.NameEn : null,
                    TargetAudience = ffv.DimPublication.TargetAudienceCodeNavigation != null ? ffv.DimPublication.TargetAudienceCodeNavigation.NameEn : null
                })
                .AsNoTracking().ToListAsync();

            // DimProfileOnlyPublication
            aittaModel.UserParticipatedPublication.AddRange(
                await _ttvContext.FactFieldValues
                    .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimProfileOnlyPublication != null && ffv.DimProfileOnlyPublicationId > 0 && ffv.Show == true)
                    .Include(ffv => ffv.DimProfileOnlyPublication)
                    .Select(ffv => new AittaPublication
                    {
                        Name = !string.IsNullOrWhiteSpace(ffv.DimProfileOnlyPublication.PublicationName) ? ffv.DimProfileOnlyPublication.PublicationName : null,
                        Year = ffv.DimProfileOnlyPublication.PublicationYear > 0 ? ffv.DimProfileOnlyPublication.PublicationYear : null,
                        //Abstract = null,
                        Keywords = null,
                        FieldsOfScience = null,
                        Type = null,
                        TargetAudience = null
                    })
                    .AsNoTracking().ToListAsync()
            );

            // DimResearchDataset
            aittaModel.UserParticipatedDataset = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimResearchDataset != null && ffv.DimResearchDatasetId > 0 && ffv.Show == true)
                .Include(ffv => ffv.DimResearchDataset)
                    .ThenInclude(rd => rd.DimKeywords)
                .Include(ffv => ffv.DimResearchDataset)
                    .ThenInclude(rd => rd.FactDimReferencedataFieldOfSciences)
                        .ThenInclude(fdrfs => fdrfs.DimReferencedata)
                .Select(ffv => new AittaResearchDataset
                {
                    DatasetTitle = LanguageFilter(
                        ffv.DimResearchDataset.NameEn,
                        ffv.DimResearchDataset.NameFi,
                        ffv.DimResearchDataset.NameSv
                    ),
                    Description = GetFirstNSentences(
                        LanguageFilter(
                            ffv.DimResearchDataset.DescriptionEn,
                            ffv.DimResearchDataset.DescriptionFi,
                            ffv.DimResearchDataset.DescriptionSv
                        ),
                        1
                    ),
                    DatasetCreationDate = ffv.DimResearchDataset.DatasetCreated != null ? ffv.DimResearchDataset.DatasetCreated : null,
                    Theme = ffv.DimResearchDataset.DimKeywords.Count > 0 ? ffv.DimResearchDataset.DimKeywords.Where(kw => kw.Scheme == "Theme").Select(kw => kw.Keyword).ToList() : null,
                    Keywords = ffv.DimResearchDataset.DimKeywords.Count > 0 ? ffv.DimResearchDataset.DimKeywords.Where(kw => kw.Scheme == "Avainsana").Select(kw => kw.Keyword).ToList() : null,
                    FieldsOfScience = ffv.DimResearchDataset.FactDimReferencedataFieldOfSciences.Select(fdrfs => fdrfs.DimReferencedata.NameEn).ToList()
                })
                .AsNoTracking().ToListAsync();

            // DimProfileOnlyResearchDataset
            aittaModel.UserParticipatedDataset.AddRange(
                await _ttvContext.FactFieldValues
                    .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimProfileOnlyDataset != null && ffv.DimProfileOnlyDatasetId > 0 && ffv.Show == true)
                    .Include(ffv => ffv.DimProfileOnlyDataset)
                    .Select(ffv => new AittaResearchDataset
                    {
                        DatasetTitle = LanguageFilter(
                            ffv.DimProfileOnlyDataset.NameEn,
                            ffv.DimProfileOnlyDataset.NameFi,
                            ffv.DimProfileOnlyDataset.NameSv
                        ),
                        Description = GetFirstNSentences(
                            LanguageFilter(
                                ffv.DimProfileOnlyDataset.DescriptionEn,
                                ffv.DimProfileOnlyDataset.DescriptionFi,
                                ffv.DimProfileOnlyDataset.DescriptionSv
                            ),
                            1
                        ),
                        DatasetCreationDate = ffv.DimProfileOnlyDataset.DatasetCreated != null ? ffv.DimProfileOnlyDataset.DatasetCreated : null,
                        Theme = null,
                        Keywords = null,
                        FieldsOfScience = null
                    })
                    .AsNoTracking().ToListAsync()
            );

            // DimFundingDecision
            aittaModel.UserParticipatedGrantedFunding = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimFundingDecision != null && ffv.DimFundingDecisionId > 0 && ffv.Show == true)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimDateIdStartNavigation)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimDateIdEndNavigation)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimOrganizationIdFunderNavigation)
                        .ThenInclude(org => org.DimOrganizationBroaderNavigation)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimKeywords)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.FactDimReferencedataFieldOfSciences)
                        .ThenInclude(fdrfs => fdrfs.DimReferencedata)
                .Include(ffv => ffv.DimFundingDecision)
                    .ThenInclude(fd => fd.DimTypeOfFunding)
                .Select(ffv => new AittaGrantedFunding
                {
                    Name = LanguageFilter(ffv.DimFundingDecision.NameEn, ffv.DimFundingDecision.NameFi, ffv.DimFundingDecision.NameSv),
                    Description = GetFirstNSentences(
                        LanguageFilter(ffv.DimFundingDecision.DescriptionEn, ffv.DimFundingDecision.DescriptionFi, ffv.DimFundingDecision.DescriptionSv),
                        1
                    ),
                    StartsOn = ffv.DimFundingDecision.DimDateIdStartNavigation != null && ffv.DimFundingDecision.DimDateIdStart > 0 && ffv.DimFundingDecision.DimDateIdStartNavigation.Year > 0 ?
                        new AittaDate { Year = ffv.DimFundingDecision.DimDateIdStartNavigation.Year, Month = ffv.DimFundingDecision.DimDateIdStartNavigation.Month } : null,
                    EndsOn = ffv.DimFundingDecision.DimDateIdEndNavigation != null && ffv.DimFundingDecision.DimDateIdEnd > 0 && ffv.DimFundingDecision.DimDateIdEndNavigation.Year > 0 ?
                        new AittaDate { Year = ffv.DimFundingDecision.DimDateIdEndNavigation.Year, Month = ffv.DimFundingDecision.DimDateIdEndNavigation.Month } : null,
                    HasFunder = ffv.DimFundingDecision.DimOrganizationIdFunderNavigation != null ? new AittaOrganization
                    {
                        OrganizationName = LanguageFilter(
                            ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameEn,
                            ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameFi,
                            ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameSv
                        ),
                        IsPartOfOrganization = ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.DimOrganizationBroader != null ?
                            new AittaOrganization
                            {
                                OrganizationName = LanguageFilter(
                                    ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameEn,
                                    ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameFi,
                                    ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameSv
                                )
                            }
                            : null
                    } : null,
                    Keywords = ffv.DimFundingDecision.DimKeywords.Count > 0 ? ffv.DimFundingDecision.DimKeywords.Where(kw => kw.Scheme == "Avainsana").Select(kw => kw.Keyword).ToList() : null,
                    FieldsOfScience = ffv.DimFundingDecision.FactDimReferencedataFieldOfSciences.Select(fdrfs => fdrfs.DimReferencedata.NameEn).ToList(),
                    TypeOfFunding = ffv.DimFundingDecision.DimTypeOfFunding != null && ffv.DimFundingDecision.DimTypeOfFundingId > 0 ? ffv.DimFundingDecision.DimTypeOfFunding.NameEn : null,
                    FieldsOfResearch = ffv.DimFundingDecision.DimKeywords.Count > 0 ? ffv.DimFundingDecision.DimKeywords.Where(kw => kw.Scheme == "Tutkimusala").Select(kw => kw.Keyword).ToList() : null,
                    Theme = ffv.DimFundingDecision.DimKeywords.Count > 0 ? ffv.DimFundingDecision.DimKeywords.Where(kw => kw.Scheme == "Teema-ala").Select(kw => kw.Keyword).ToList() : null
                }).AsNoTracking().ToListAsync();

            // DimProfileOnlyFundingDecision
            aittaModel.UserParticipatedGrantedFunding.AddRange(
                await _ttvContext.FactFieldValues
                    .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimProfileOnlyFundingDecision != null && ffv.DimProfileOnlyFundingDecisionId > 0 && ffv.Show == true)
                    .Include(ffv => ffv.DimProfileOnlyFundingDecision)
                        .ThenInclude(pofd => pofd.DimDateIdStartNavigation)
                    .Include(ffv => ffv.DimProfileOnlyFundingDecision)
                        .ThenInclude(pofd => pofd.DimDateIdEndNavigation)
                    .Include(ffv => ffv.DimProfileOnlyFundingDecision)
                        .ThenInclude(pofd => pofd.DimOrganizationIdFunderNavigation)
                            .ThenInclude(org => org.DimOrganizationBroaderNavigation)
                    .Include(ffv => ffv.DimProfileOnlyFundingDecision)
                        .ThenInclude(pofd => pofd.DimTypeOfFunding)
                    .Select(ffv => new AittaGrantedFunding
                    {
                        Name = LanguageFilter(ffv.DimProfileOnlyFundingDecision.NameEn, ffv.DimProfileOnlyFundingDecision.NameFi, ffv.DimProfileOnlyFundingDecision.NameSv),
                        Description = GetFirstNSentences(
                            LanguageFilter(ffv.DimProfileOnlyFundingDecision.DescriptionEn, ffv.DimProfileOnlyFundingDecision.DescriptionFi, ffv.DimProfileOnlyFundingDecision.DescriptionSv),
                            1
                        ),
                        StartsOn = ffv.DimProfileOnlyFundingDecision.DimDateIdStartNavigation != null && ffv.DimProfileOnlyFundingDecision.DimDateIdStart > 0 && ffv.DimProfileOnlyFundingDecision.DimDateIdStartNavigation.Year > 0 ?
                            new AittaDate { Year = ffv.DimProfileOnlyFundingDecision.DimDateIdStartNavigation.Year, Month = ffv.DimProfileOnlyFundingDecision.DimDateIdStartNavigation.Month } : null,
                        EndsOn = ffv.DimProfileOnlyFundingDecision.DimDateIdEndNavigation != null && ffv.DimProfileOnlyFundingDecision.DimDateIdEnd > 0 && ffv.DimProfileOnlyFundingDecision.DimDateIdEndNavigation.Year > 0 ?
                            new AittaDate { Year = ffv.DimProfileOnlyFundingDecision.DimDateIdEndNavigation.Year, Month = ffv.DimProfileOnlyFundingDecision.DimDateIdEndNavigation.Month } : null,
                        HasFunder = ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation != null ?
                            // DimProfileOnlyFundingDecision has relation to DimOrganization
                            new AittaOrganization
                            {
                                OrganizationName = LanguageFilter(
                                    ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.NameEn,
                                    ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.NameFi,
                                    ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.NameSv
                                ),
                                IsPartOfOrganization = ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.DimOrganizationBroader != null ?
                                    new AittaOrganization
                                    {
                                        OrganizationName = LanguageFilter(
                                            ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameEn,
                                            ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameFi,
                                            ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.DimOrganizationBroaderNavigation.NameSv
                                        )
                                    }
                                    : null
                            } :
                                // DimProfileOnlyFundingDecision has no relation to DimOrganization -> try to get organization from DimIdentifierlessData
                                ffv.DimIdentifierlessData != null && ffv.DimIdentifierlessDataId > 0 ? AittaOrganizationFromIdentifierlessData(ffv.DimIdentifierlessData) : null,
                        Keywords = null,
                        FieldsOfScience = null,
                        TypeOfFunding = ffv.DimProfileOnlyFundingDecision.DimTypeOfFunding != null && ffv.DimProfileOnlyFundingDecision.DimTypeOfFundingId > 0 ? ffv.DimProfileOnlyFundingDecision.DimTypeOfFunding.NameEn : null,
                        FieldsOfResearch = null,
                        Theme = null
                    }).AsNoTracking().ToListAsync()
            );

            // DimResearchActivity
            aittaModel.UserParticipatedActivity = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimResearchActivity != null && ffv.DimResearchActivityId > 0 && ffv.Show == true)
                .Include(ffv => ffv.DimResearchActivity)
                    .ThenInclude(ra => ra.DimStartDateNavigation)
                .Include(ffv => ffv.DimResearchActivity)
                    .ThenInclude(ra => ra.DimEndDateNavigation)
                .Select(ffv => new AittaResearchActivity
                {
                    ActivityTitle = LanguageFilter(ffv.DimResearchActivity.NameEn, ffv.DimResearchActivity.NameFi, ffv.DimResearchActivity.NameSv),
                    Description = GetFirstNSentences(
                        LanguageFilter(ffv.DimResearchActivity.DescriptionEn, ffv.DimResearchActivity.DescriptionFi, ffv.DimResearchActivity.DescriptionSv),
                        1
                    ),
                    Role = null,
                    Type = null,
                    StartsOn = ffv.DimResearchActivity.DimStartDateNavigation != null && ffv.DimResearchActivity.DimStartDate > 0 && ffv.DimResearchActivity.DimStartDateNavigation.Year > 0 ?
                        new AittaDate { Year = ffv.DimResearchActivity.DimStartDateNavigation.Year, Month = ffv.DimResearchActivity.DimStartDateNavigation.Month } : null,
                    EndsOn = ffv.DimResearchActivity.DimEndDateNavigation != null && ffv.DimResearchActivity.DimEndDate > 0 && ffv.DimResearchActivity.DimEndDateNavigation.Year > 0 ?
                        new AittaDate { Year = ffv.DimResearchActivity.DimEndDateNavigation.Year, Month = ffv.DimResearchActivity.DimEndDateNavigation.Month } : null
                })
                .AsNoTracking().ToListAsync();
                
            // DimProfileOnlyResearchActivity
            aittaModel.UserParticipatedActivity.AddRange(
                await _ttvContext.FactFieldValues
                    .Where(ffv => ffv.DimUserProfile.OrcidId == orcidId && ffv.DimProfileOnlyResearchActivity != null && ffv.DimProfileOnlyResearchActivityId > 0 && ffv.Show == true)
                    .Include(ffv => ffv.DimProfileOnlyResearchActivity)
                        .ThenInclude(pora => pora.DimDateIdStartNavigation)
                    .Include(ffv => ffv.DimProfileOnlyResearchActivity)
                        .ThenInclude(pora => pora.DimDateIdEndNavigation)
                    .Include(ffv => ffv.DimReferencedataActorRole)
                    .Select(ffv => new AittaResearchActivity
                    {
                        ActivityTitle = LanguageFilter(ffv.DimProfileOnlyResearchActivity.NameEn, ffv.DimProfileOnlyResearchActivity.NameFi, ffv.DimProfileOnlyResearchActivity.NameSv),
                        Description = GetFirstNSentences(
                            LanguageFilter(ffv.DimProfileOnlyResearchActivity.DescriptionEn, ffv.DimProfileOnlyResearchActivity.DescriptionFi, ffv.DimProfileOnlyResearchActivity.DescriptionSv),
                            1
                        ),
                        Role = ffv.DimReferencedataActorRole != null && ffv.DimReferencedataActorRoleId > 0 ? LanguageFilter(ffv.DimReferencedataActorRole.NameEn, ffv.DimReferencedataActorRole.NameFi, ffv.DimReferencedataActorRole.NameSv) : null,
                        Type = null,
                        StartsOn = ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation != null && ffv.DimProfileOnlyResearchActivity.DimDateIdStart > 0 && ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation.Year > 0 ?
                            new AittaDate { Year = ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation.Year, Month = ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation.Month } : null,
                        EndsOn = ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation != null && ffv.DimProfileOnlyResearchActivity.DimDateIdEnd > 0 && ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation.Year > 0 ?
                            new AittaDate { Year = ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation.Year, Month = ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation.Month } : null
                    })
                    .AsNoTracking().ToListAsync()
            );

            return JsonSerializer.Serialize(
                    aittaModel,
                    new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    }
                );
        }

        /// <summary>
        /// Returns the first *n* sentences (ending with '.') from <paramref name="input"/>.
        /// Sentences that end with other punctuation (.!? etc.) are ignored.
        /// </summary>
        static string? GetFirstNSentences(string input, int n)
        {
            if (string.IsNullOrWhiteSpace(input) || n <= 0)
                return null;

            // 1. Split the text on periods, but keep the delimiter with each piece.
            //    The regex `(?<=[.])` is a zero‑width positive lookbehind that matches
            //    the position immediately after every period.
            var parts = Regex.Split(input, @"(?<=[.])");

            // 2. Trim whitespace and discard empty fragments (e.g., trailing newlines).
            var sentences = parts
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .Take(n);

            // 3. Join them back together with a single space (or any separator you prefer).
            return string.Join(" ", sentences);
        }

        static string? LanguageFilter(string? en, string? fi, string? sv)
        {
            if (!string.IsNullOrWhiteSpace(en))
            {
                return en;
            }
            else if (!string.IsNullOrWhiteSpace(fi))
            {
                return fi;
            }
            else if (!string.IsNullOrWhiteSpace(sv))
            {
                return sv;
            }
            else
            {
                return null;
            }
        }
        
        static AittaOrganization? AittaOrganizationFromIdentifierlessData(DimIdentifierlessDatum identifierlessData)
        {
            string? organizationName = null;
            string? organizationUnit = null;

            if (identifierlessData.Type == "organization_name")
            {
                organizationName = LanguageFilter(
                    en: identifierlessData.ValueEn,
                    fi: identifierlessData.ValueFi,
                    sv: identifierlessData.ValueSv
                );

                if (identifierlessData.InverseDimIdentifierlessData != null && identifierlessData.InverseDimIdentifierlessData.Count > 0)
                {
                    organizationUnit = LanguageFilter(
                        en: identifierlessData.InverseDimIdentifierlessData.FirstOrDefault().ValueEn,
                        fi: identifierlessData.InverseDimIdentifierlessData.FirstOrDefault().ValueFi,
                        sv: identifierlessData.InverseDimIdentifierlessData.FirstOrDefault().ValueSv
                    );
                }
            }
            if (organizationName != null && organizationUnit != null)
            {
                return new AittaOrganization
                {
                    OrganizationName = organizationUnit,
                    IsPartOfOrganization = new AittaOrganization
                    {
                        OrganizationName = organizationName
                    }
                };
            }
            else if (organizationName != null)
            {
                return new AittaOrganization
                {
                    OrganizationName = organizationName,
                    IsPartOfOrganization = null
                };
            }
            return null;
        }

    }
}
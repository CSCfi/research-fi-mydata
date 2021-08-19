using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace api.Models.Ttv
{
    public partial class TtvContext : DbContext
    {
        public TtvContext()
        {
        }

        public TtvContext(DbContextOptions<TtvContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BrCallProgrammeDimCallProgramme> BrCallProgrammeDimCallProgrammes { get; set; }
        public virtual DbSet<BrEsfriDimInfrastructure> BrEsfriDimInfrastructures { get; set; }
        public virtual DbSet<BrFieldDisplaySettingsDimRegisteredDataSource> BrFieldDisplaySettingsDimRegisteredDataSources { get; set; }
        public virtual DbSet<BrFieldOfArtDimPublication> BrFieldOfArtDimPublications { get; set; }
        public virtual DbSet<BrFieldOfEducationDimPublication> BrFieldOfEducationDimPublications { get; set; }
        public virtual DbSet<BrFieldOfScienceDimFundingDecision> BrFieldOfScienceDimFundingDecisions { get; set; }
        public virtual DbSet<BrFieldOfScienceDimPublication> BrFieldOfScienceDimPublications { get; set; }
        public virtual DbSet<BrFundingConsortiumParticipation> BrFundingConsortiumParticipations { get; set; }
        public virtual DbSet<BrFundingDecisionDimFieldOfArt> BrFundingDecisionDimFieldOfArts { get; set; }
        public virtual DbSet<BrInfrastructureDimFieldOfScience> BrInfrastructureDimFieldOfSciences { get; set; }
        public virtual DbSet<BrKeywordDimFundingDecision> BrKeywordDimFundingDecisions { get; set; }
        public virtual DbSet<BrKeywordDimPublication> BrKeywordDimPublications { get; set; }
        public virtual DbSet<BrLanguageCodesForDataset> BrLanguageCodesForDatasets { get; set; }
        public virtual DbSet<BrMerilDimInfrastructure> BrMerilDimInfrastructures { get; set; }
        public virtual DbSet<BrOrganizationsFundCallProgramme> BrOrganizationsFundCallProgrammes { get; set; }
        public virtual DbSet<BrParticipatesInFundingGroup> BrParticipatesInFundingGroups { get; set; }
        public virtual DbSet<BrPredecessorOrganization> BrPredecessorOrganizations { get; set; }
        public virtual DbSet<BrPreviousFundingDecision> BrPreviousFundingDecisions { get; set; }
        public virtual DbSet<BrRelatedFundingDecision> BrRelatedFundingDecisions { get; set; }
        public virtual DbSet<BrResearchDatasetDimFieldOfScience> BrResearchDatasetDimFieldOfSciences { get; set; }
        public virtual DbSet<BrResearchDatasetDimKeyword> BrResearchDatasetDimKeywords { get; set; }
        public virtual DbSet<BrServiceSubscription> BrServiceSubscriptions { get; set; }
        public virtual DbSet<BrSuccessorOrganization> BrSuccessorOrganizations { get; set; }
        public virtual DbSet<DimAffiliation> DimAffiliations { get; set; }
        public virtual DbSet<DimCallProgramme> DimCallProgrammes { get; set; }
        public virtual DbSet<DimCompetence> DimCompetences { get; set; }
        public virtual DbSet<DimDate> DimDates { get; set; }
        public virtual DbSet<DimEducation> DimEducations { get; set; }
        public virtual DbSet<DimEmailAddrress> DimEmailAddrresses { get; set; }
        public virtual DbSet<DimEsfri> DimEsfris { get; set; }
        public virtual DbSet<DimEvent> DimEvents { get; set; }
        public virtual DbSet<DimExternalService> DimExternalServices { get; set; }
        public virtual DbSet<DimFieldDisplaySetting> DimFieldDisplaySettings { get; set; }
        public virtual DbSet<DimFieldOfArt> DimFieldOfArts { get; set; }
        public virtual DbSet<DimFieldOfEducation> DimFieldOfEducations { get; set; }
        public virtual DbSet<DimFieldOfScience> DimFieldOfSciences { get; set; }
        public virtual DbSet<DimFieldOfScienceDimResearchActivity> DimFieldOfScienceDimResearchActivities { get; set; }
        public virtual DbSet<DimFundingDecision> DimFundingDecisions { get; set; }
        public virtual DbSet<DimGeo> DimGeos { get; set; }
        public virtual DbSet<DimIdentifierlessDatum> DimIdentifierlessData { get; set; }
        public virtual DbSet<DimInfrastructure> DimInfrastructures { get; set; }
        public virtual DbSet<DimKeyword> DimKeywords { get; set; }
        public virtual DbSet<DimKnownPerson> DimKnownPeople { get; set; }
        public virtual DbSet<DimKnownPersonDimFieldOfScience> DimKnownPersonDimFieldOfSciences { get; set; }
        public virtual DbSet<DimLocallyReportedPubInfo> DimLocallyReportedPubInfos { get; set; }
        public virtual DbSet<DimMeril> DimMerils { get; set; }
        public virtual DbSet<DimName> DimNames { get; set; }
        public virtual DbSet<DimNewsFeed> DimNewsFeeds { get; set; }
        public virtual DbSet<DimNewsItem> DimNewsItems { get; set; }
        public virtual DbSet<DimOrcidPublication> DimOrcidPublications { get; set; }
        public virtual DbSet<DimOrganisationMedium> DimOrganisationMedia { get; set; }
        public virtual DbSet<DimOrganization> DimOrganizations { get; set; }
        public virtual DbSet<DimPid> DimPids { get; set; }
        public virtual DbSet<DimPublication> DimPublications { get; set; }
        public virtual DbSet<DimPublicationChannel> DimPublicationChannels { get; set; }
        public virtual DbSet<DimReferencedatum> DimReferencedata { get; set; }
        public virtual DbSet<DimRegisteredDataSource> DimRegisteredDataSources { get; set; }
        public virtual DbSet<DimResearchActivity> DimResearchActivities { get; set; }
        public virtual DbSet<DimResearchActivityDimKeyword> DimResearchActivityDimKeywords { get; set; }
        public virtual DbSet<DimResearchCommunity> DimResearchCommunities { get; set; }
        public virtual DbSet<DimResearchDataCatalog> DimResearchDataCatalogs { get; set; }
        public virtual DbSet<DimResearchDataset> DimResearchDatasets { get; set; }
        public virtual DbSet<DimResearcherDescription> DimResearcherDescriptions { get; set; }
        public virtual DbSet<DimResearcherToResearchCommunity> DimResearcherToResearchCommunities { get; set; }
        public virtual DbSet<DimSector> DimSectors { get; set; }
        public virtual DbSet<DimService> DimServices { get; set; }
        public virtual DbSet<DimServicePoint> DimServicePoints { get; set; }
        public virtual DbSet<DimTelephoneNumber> DimTelephoneNumbers { get; set; }
        public virtual DbSet<DimTypeOfFunding> DimTypeOfFundings { get; set; }
        public virtual DbSet<DimUserProfile> DimUserProfiles { get; set; }
        public virtual DbSet<DimWebLink> DimWebLinks { get; set; }
        public virtual DbSet<FactContribution> FactContributions { get; set; }
        public virtual DbSet<FactFieldValue> FactFieldValues { get; set; }
        public virtual DbSet<FactInfraKeyword> FactInfraKeywords { get; set; }
        public virtual DbSet<FactJufoClassCodesForPubChannel> FactJufoClassCodesForPubChannels { get; set; }
        public virtual DbSet<FactUpkeep> FactUpkeeps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;User Id=sa;Password=Test1234;database=Ttv;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BrCallProgrammeDimCallProgramme>(entity =>
            {
                entity.HasKey(e => new { e.DimCallProgrammeId, e.DimCallProgrammeId2 })
                    .HasName("PK__br_call___6F0CEDFB9005A738");

                entity.ToTable("br_call_programme_dim_call_programme");

                entity.Property(e => e.DimCallProgrammeId).HasColumnName("dim_call_programme_id");

                entity.Property(e => e.DimCallProgrammeId2).HasColumnName("dim_call_programme_id2");

                entity.HasOne(d => d.DimCallProgramme)
                    .WithMany(p => p.BrCallProgrammeDimCallProgrammeDimCallProgrammes)
                    .HasForeignKey(d => d.DimCallProgrammeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("belongs to / a part of ");

                entity.HasOne(d => d.DimCallProgrammeId2Navigation)
                    .WithMany(p => p.BrCallProgrammeDimCallProgrammeDimCallProgrammeId2Navigations)
                    .HasForeignKey(d => d.DimCallProgrammeId2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_call_pr785575");
            });

            modelBuilder.Entity<BrEsfriDimInfrastructure>(entity =>
            {
                entity.HasKey(e => new { e.DimEsfriId, e.DimInfrastructureId })
                    .HasName("PK__br_esfri__A4A0FE10228343FE");

                entity.ToTable("br_esfri_dim_infrastructure");

                entity.Property(e => e.DimEsfriId).HasColumnName("dim_esfri_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.HasOne(d => d.DimEsfri)
                    .WithMany(p => p.BrEsfriDimInfrastructures)
                    .HasForeignKey(d => d.DimEsfriId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_esfri_d559740");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.BrEsfriDimInfrastructures)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_esfri_d490989");
            });

            modelBuilder.Entity<BrFieldDisplaySettingsDimRegisteredDataSource>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldDisplaySettingsId, e.DimRegisteredDataSourceId })
                    .HasName("PK__br_field__6148A7723B475AB5");

                entity.ToTable("br_field_display_settings_dim_registered_data_source");

                entity.Property(e => e.DimFieldDisplaySettingsId).HasColumnName("dim_field_display_settings_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.HasOne(d => d.DimFieldDisplaySettings)
                    .WithMany(p => p.BrFieldDisplaySettingsDimRegisteredDataSources)
                    .HasForeignKey(d => d.DimFieldDisplaySettingsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_d783303");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.BrFieldDisplaySettingsDimRegisteredDataSources)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_d115264");
            });

            modelBuilder.Entity<BrFieldOfArtDimPublication>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfArtId, e.DimPublicationId })
                    .HasName("PK__br_field__809A87CD5F66E394");

                entity.ToTable("br_field_of_art_dim_publication");

                entity.Property(e => e.DimFieldOfArtId).HasColumnName("dim_field_of_art_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.HasOne(d => d.DimFieldOfArt)
                    .WithMany(p => p.BrFieldOfArtDimPublications)
                    .HasForeignKey(d => d.DimFieldOfArtId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o978876");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.BrFieldOfArtDimPublications)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o505394");
            });

            modelBuilder.Entity<BrFieldOfEducationDimPublication>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfEducationId, e.DimPublicationId })
                    .HasName("PK__br_field__6E377B2C93F2343F");

                entity.ToTable("br_field_of_education_dim_publication");

                entity.Property(e => e.DimFieldOfEducationId).HasColumnName("dim_field_of_education_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.HasOne(d => d.DimFieldOfEducation)
                    .WithMany(p => p.BrFieldOfEducationDimPublications)
                    .HasForeignKey(d => d.DimFieldOfEducationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o983513");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.BrFieldOfEducationDimPublications)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o449658");
            });

            modelBuilder.Entity<BrFieldOfScienceDimFundingDecision>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfScienceId, e.DimFundingDecisionId })
                    .HasName("PK__br_field__1A103AF75CAFC581");

                entity.ToTable("br_field_of_science_dim_funding_decision");

                entity.Property(e => e.DimFieldOfScienceId).HasColumnName("dim_field_of_science_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.BrFieldOfScienceDimFundingDecisions)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o643706");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrFieldOfScienceDimFundingDecisions)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o993952");
            });

            modelBuilder.Entity<BrFieldOfScienceDimPublication>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfScienceId, e.DimPublicationId })
                    .HasName("PK__br_field__5088B7764BF4E923");

                entity.ToTable("br_field_of_science_dim_publication");

                entity.Property(e => e.DimFieldOfScienceId).HasColumnName("dim_field_of_science_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.BrFieldOfScienceDimPublications)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o934749");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.BrFieldOfScienceDimPublications)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o201610");
            });

            modelBuilder.Entity<BrFundingConsortiumParticipation>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionId, e.DimOrganizationid })
                    .HasName("PK__br_fundi__3DB567F8FBE9330E");

                entity.ToTable("br_funding_consortium_participation");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimOrganizationid).HasColumnName("dim_organizationid");

                entity.Property(e => e.EndOfParticipation).HasColumnName("end_of_participation");

                entity.Property(e => e.RoleInConsortium)
                    .HasMaxLength(255)
                    .HasColumnName("role_in_consortium");

                entity.Property(e => e.ShareOfFundingInEur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("share_of_funding_in_EUR");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrFundingConsortiumParticipations)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_funding504308");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.BrFundingConsortiumParticipations)
                    .HasForeignKey(d => d.DimOrganizationid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_funding503907");
            });

            modelBuilder.Entity<BrFundingDecisionDimFieldOfArt>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionId, e.DimFieldOfArtId })
                    .HasName("PK__br_fundi__07CB586DD1342EED");

                entity.ToTable("br_funding_decision_dim_field_of_art");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimFieldOfArtId).HasColumnName("dim_field_of_art_id");

                entity.HasOne(d => d.DimFieldOfArt)
                    .WithMany(p => p.BrFundingDecisionDimFieldOfArts)
                    .HasForeignKey(d => d.DimFieldOfArtId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_funding154428");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrFundingDecisionDimFieldOfArts)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_funding281737");
            });

            modelBuilder.Entity<BrInfrastructureDimFieldOfScience>(entity =>
            {
                entity.HasKey(e => new { e.DimInfrastructureId, e.DimFieldOfScienceId })
                    .HasName("PK__br_infra__17B77C16BC82C13B");

                entity.ToTable("br_infrastructure_dim_field_of_science");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.DimFieldOfScienceId).HasColumnName("dim_field_of_science_id");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.BrInfrastructureDimFieldOfSciences)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_infrast565984");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.BrInfrastructureDimFieldOfSciences)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_infrast156732");
            });

            modelBuilder.Entity<BrKeywordDimFundingDecision>(entity =>
            {
                entity.HasKey(e => new { e.DimKeywordId, e.DimFundingDecisionId })
                    .HasName("PK__br_keywo__8C7B929B2049F4FB");

                entity.ToTable("br_keyword_dim_funding_decision");

                entity.Property(e => e.DimKeywordId).HasColumnName("dim_keyword_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrKeywordDimFundingDecisions)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_keyword955130");

                entity.HasOne(d => d.DimKeyword)
                    .WithMany(p => p.BrKeywordDimFundingDecisions)
                    .HasForeignKey(d => d.DimKeywordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_keyword224605");
            });

            modelBuilder.Entity<BrKeywordDimPublication>(entity =>
            {
                entity.HasKey(e => new { e.DimKeywordId, e.DimPublicationId })
                    .HasName("PK__br_keywo__C6E31F1A3BF7304E");

                entity.ToTable("br_keyword_dim_publication");

                entity.Property(e => e.DimKeywordId).HasColumnName("dim_keyword_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.HasOne(d => d.DimKeyword)
                    .WithMany(p => p.BrKeywordDimPublications)
                    .HasForeignKey(d => d.DimKeywordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_keyword944303");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.BrKeywordDimPublications)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_keyword634640");
            });

            modelBuilder.Entity<BrLanguageCodesForDataset>(entity =>
            {
                entity.HasKey(e => new { e.DimResearchDatasetId, e.DimReferencedataId })
                    .HasName("PK__br_langu__576647BF68B20345");

                entity.ToTable("br_language_codes_for_datasets");

                entity.Property(e => e.DimResearchDatasetId).HasColumnName("dim_research_dataset_id");

                entity.Property(e => e.DimReferencedataId).HasColumnName("dim_referencedata_id");

                entity.HasOne(d => d.DimReferencedata)
                    .WithMany(p => p.BrLanguageCodesForDatasets)
                    .HasForeignKey(d => d.DimReferencedataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_languag480770");

                entity.HasOne(d => d.DimResearchDataset)
                    .WithMany(p => p.BrLanguageCodesForDatasets)
                    .HasForeignKey(d => d.DimResearchDatasetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_languag34243");
            });

            modelBuilder.Entity<BrMerilDimInfrastructure>(entity =>
            {
                entity.HasKey(e => new { e.DimMerilId, e.DimInfrastructureId })
                    .HasName("PK__br_meril__A30C54DA625F830D");

                entity.ToTable("br_meril_dim_infrastructure");

                entity.Property(e => e.DimMerilId).HasColumnName("dim_meril_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.BrMerilDimInfrastructures)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_meril_d901766");

                entity.HasOne(d => d.DimMeril)
                    .WithMany(p => p.BrMerilDimInfrastructures)
                    .HasForeignKey(d => d.DimMerilId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_meril_d209645");
            });

            modelBuilder.Entity<BrOrganizationsFundCallProgramme>(entity =>
            {
                entity.HasKey(e => new { e.DimOrganizationid, e.DimCallProgrammeid })
                    .HasName("PK__br_organ__10F219BCDAF3E03A");

                entity.ToTable("br_organizations_fund_call_programmes");

                entity.Property(e => e.DimOrganizationid).HasColumnName("dim_organizationid");

                entity.Property(e => e.DimCallProgrammeid).HasColumnName("dim_call_programmeid");

                entity.HasOne(d => d.DimCallProgramme)
                    .WithMany(p => p.BrOrganizationsFundCallProgrammes)
                    .HasForeignKey(d => d.DimCallProgrammeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_organiz165034");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.BrOrganizationsFundCallProgrammes)
                    .HasForeignKey(d => d.DimOrganizationid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_organiz621686");
            });

            modelBuilder.Entity<BrParticipatesInFundingGroup>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionid, e.DimNameId })
                    .HasName("PK__br_parti__5EC9BC64243B857E");

                entity.ToTable("br_participates_in_funding_group");

                entity.Property(e => e.DimFundingDecisionid).HasColumnName("dim_funding_decisionid");

                entity.Property(e => e.DimNameId).HasColumnName("dim_name_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.EndOfParticipation).HasColumnName("end_of_participation");

                entity.Property(e => e.RoleInFundingGroup)
                    .HasMaxLength(255)
                    .HasColumnName("role_in_funding_group");

                entity.Property(e => e.ShareOfFundingInEur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("share_of_funding_in_EUR");

                entity.Property(e => e.SourceId)
                    .HasMaxLength(30)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrParticipatesInFundingGroups)
                    .HasForeignKey(d => d.DimFundingDecisionid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_partici137682");

                entity.HasOne(d => d.DimName)
                    .WithMany(p => p.BrParticipatesInFundingGroups)
                    .HasForeignKey(d => d.DimNameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_partici869162");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.BrParticipatesInFundingGroups)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("get_funding_in_name_of_org");
            });

            modelBuilder.Entity<BrPredecessorOrganization>(entity =>
            {
                entity.HasKey(e => new { e.DimOrganizationid, e.DimOrganizationid2 })
                    .HasName("PK__br_prede__A7CAD2F488E9A5D7");

                entity.ToTable("br_predecessor_organization");

                entity.Property(e => e.DimOrganizationid).HasColumnName("dim_organizationid");

                entity.Property(e => e.DimOrganizationid2).HasColumnName("dim_organizationid2");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.BrPredecessorOrganizationDimOrganizations)
                    .HasForeignKey(d => d.DimOrganizationid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_predece849307");

                entity.HasOne(d => d.DimOrganizationid2Navigation)
                    .WithMany(p => p.BrPredecessorOrganizationDimOrganizationid2Navigations)
                    .HasForeignKey(d => d.DimOrganizationid2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_predece505451");
            });

            modelBuilder.Entity<BrPreviousFundingDecision>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionFromId, e.DimFundingDecisionToId })
                    .HasName("PK__br_previ__90966491B197976E");

                entity.ToTable("br_previous_funding_decision");

                entity.Property(e => e.DimFundingDecisionFromId).HasColumnName("dim_funding_decision_from_id");

                entity.Property(e => e.DimFundingDecisionToId).HasColumnName("dim_funding_decision_to_id");

                entity.HasOne(d => d.DimFundingDecisionFrom)
                    .WithMany(p => p.BrPreviousFundingDecisionDimFundingDecisionFroms)
                    .HasForeignKey(d => d.DimFundingDecisionFromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_previou481541");

                entity.HasOne(d => d.DimFundingDecisionTo)
                    .WithMany(p => p.BrPreviousFundingDecisionDimFundingDecisionTos)
                    .HasForeignKey(d => d.DimFundingDecisionToId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_previou440746");
            });

            modelBuilder.Entity<BrRelatedFundingDecision>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionFromId, e.DimFundingDecisionToId })
                    .HasName("PK__br_relat__909664916F5CA1D0");

                entity.ToTable("br_related_funding_decision");

                entity.Property(e => e.DimFundingDecisionFromId).HasColumnName("dim_funding_decision_from_id");

                entity.Property(e => e.DimFundingDecisionToId).HasColumnName("dim_funding_decision_to_id");

                entity.HasOne(d => d.DimFundingDecisionFrom)
                    .WithMany(p => p.BrRelatedFundingDecisionDimFundingDecisionFroms)
                    .HasForeignKey(d => d.DimFundingDecisionFromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_related232364");

                entity.HasOne(d => d.DimFundingDecisionTo)
                    .WithMany(p => p.BrRelatedFundingDecisionDimFundingDecisionTos)
                    .HasForeignKey(d => d.DimFundingDecisionToId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_related689923");
            });

            modelBuilder.Entity<BrResearchDatasetDimFieldOfScience>(entity =>
            {
                entity.HasKey(e => new { e.DimResearchDatasetid, e.DimFieldOfScienceid })
                    .HasName("PK__br_resea__ADD3384B91A34EE9");

                entity.ToTable("br_research_dataset_dim_field_of_science");

                entity.Property(e => e.DimResearchDatasetid).HasColumnName("dim_research_datasetid");

                entity.Property(e => e.DimFieldOfScienceid).HasColumnName("dim_field_of_scienceid");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.BrResearchDatasetDimFieldOfSciences)
                    .HasForeignKey(d => d.DimFieldOfScienceid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_researc385994");

                entity.HasOne(d => d.DimResearchDataset)
                    .WithMany(p => p.BrResearchDatasetDimFieldOfSciences)
                    .HasForeignKey(d => d.DimResearchDatasetid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_researc927326");
            });

            modelBuilder.Entity<BrResearchDatasetDimKeyword>(entity =>
            {
                entity.HasKey(e => new { e.DimResearchDatasetId, e.DimKeywordId })
                    .HasName("PK__br_resea__4D226DF2BE09C5C5");

                entity.ToTable("br_research_dataset_dim_keyword");

                entity.Property(e => e.DimResearchDatasetId).HasColumnName("dim_research_dataset_id");

                entity.Property(e => e.DimKeywordId).HasColumnName("dim_keyword_id");

                entity.HasOne(d => d.DimKeyword)
                    .WithMany(p => p.BrResearchDatasetDimKeywords)
                    .HasForeignKey(d => d.DimKeywordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbt_researc329080");

                entity.HasOne(d => d.DimResearchDataset)
                    .WithMany(p => p.BrResearchDatasetDimKeywords)
                    .HasForeignKey(d => d.DimResearchDatasetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("dataset-keywords");
            });

            modelBuilder.Entity<BrServiceSubscription>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("br_service_subscription");

                entity.Property(e => e.DimExternalServiceId).HasColumnName("dim_external_service_id");

                entity.Property(e => e.DimUserProfileId).HasColumnName("dim_user_profile_id");

                entity.HasOne(d => d.DimExternalService)
                    .WithMany()
                    .HasForeignKey(d => d.DimExternalServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_service763943");

                entity.HasOne(d => d.DimUserProfile)
                    .WithMany()
                    .HasForeignKey(d => d.DimUserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("permitted services");
            });

            modelBuilder.Entity<BrSuccessorOrganization>(entity =>
            {
                entity.HasKey(e => new { e.DimOrganizationid, e.DimOrganizationid2 })
                    .HasName("PK__br_succe__A7CAD2F46F847B1F");

                entity.ToTable("br_successor organization");

                entity.Property(e => e.DimOrganizationid).HasColumnName("dim_organizationid");

                entity.Property(e => e.DimOrganizationid2).HasColumnName("dim_organizationid2");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.BrSuccessorOrganizationDimOrganizations)
                    .HasForeignKey(d => d.DimOrganizationid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_success452227");

                entity.HasOne(d => d.DimOrganizationid2Navigation)
                    .WithMany(p => p.BrSuccessorOrganizationDimOrganizationid2Navigations)
                    .HasForeignKey(d => d.DimOrganizationid2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_success902531");
            });

            modelBuilder.Entity<DimAffiliation>(entity =>
            {
                entity.ToTable("dim_affiliation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AffiliationType).HasColumnName("affiliation_type");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.PositionCode).HasColumnName("position_code");

                entity.Property(e => e.PositionNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("position_name_en");

                entity.Property(e => e.PositionNameFi)
                    .HasMaxLength(255)
                    .HasColumnName("position_name_fi");

                entity.Property(e => e.PositionNameSv)
                    .HasMaxLength(255)
                    .HasColumnName("position_name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.HasOne(d => d.AffiliationTypeNavigation)
                    .WithMany(p => p.DimAffiliationAffiliationTypeNavigations)
                    .HasForeignKey(d => d.AffiliationType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_affili651984");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimAffiliations)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_affili162369");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimAffiliations)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_affili510828");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimAffiliations)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_affili501573");

                entity.HasOne(d => d.EndDateNavigation)
                    .WithMany(p => p.DimAffiliationEndDateNavigations)
                    .HasForeignKey(d => d.EndDate)
                    .HasConstraintName("FKdim_affili435050");

                entity.HasOne(d => d.PositionCodeNavigation)
                    .WithMany(p => p.DimAffiliationPositionCodeNavigations)
                    .HasForeignKey(d => d.PositionCode)
                    .HasConstraintName("FKdim_affili562212");

                entity.HasOne(d => d.StartDateNavigation)
                    .WithMany(p => p.DimAffiliationStartDateNavigations)
                    .HasForeignKey(d => d.StartDate)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_affili706343");
            });

            modelBuilder.Entity<DimCallProgramme>(entity =>
            {
                entity.ToTable("dim_call_programme");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Abbreviation)
                    .HasMaxLength(511)
                    .HasColumnName("abbreviation");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimCallProgrammeId).HasColumnName("dim_call_programme_id");

                entity.Property(e => e.DimDateIdDue).HasColumnName("dim_date_id_due");

                entity.Property(e => e.DimDateIdOpen).HasColumnName("dim_date_id_open");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.EuCallId)
                    .HasMaxLength(511)
                    .HasColumnName("eu_call_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(511)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(511)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(511)
                    .HasColumnName("name_sv");

                entity.Property(e => e.NameUnd)
                    .HasMaxLength(511)
                    .HasColumnName("name_und");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.SourceProgrammeId)
                    .HasMaxLength(55)
                    .HasColumnName("source_programme_id");

                entity.HasOne(d => d.DimDateIdDueNavigation)
                    .WithMany(p => p.DimCallProgrammeDimDateIdDueNavigations)
                    .HasForeignKey(d => d.DimDateIdDue)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("start");

                entity.HasOne(d => d.DimDateIdOpenNavigation)
                    .WithMany(p => p.DimCallProgrammeDimDateIdOpenNavigations)
                    .HasForeignKey(d => d.DimDateIdOpen)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("end");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimCallProgrammes)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_call_p102028");
            });

            modelBuilder.Entity<DimCompetence>(entity =>
            {
                entity.ToTable("dim_competence");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompetenceType).HasColumnName("competence_type");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionEn)
                    .HasMaxLength(4000)
                    .HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi)
                    .HasMaxLength(4000)
                    .HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv)
                    .HasMaxLength(4000)
                    .HasColumnName("description_sv");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.LocalIdentifier).HasColumnName("local_identifier");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(4000)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(4000)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(4000)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimCompetences)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("competence");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimCompetences)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_compet151101");
            });

            modelBuilder.Entity<DimDate>(entity =>
            {
                entity.ToTable("dim_date");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Day).HasColumnName("day");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.Month).HasColumnName("month");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<DimEducation>(entity =>
            {
                entity.ToTable("dim_education");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Credits).HasColumnName("credits");

                entity.Property(e => e.DegreeGrantingInstitutionName)
                    .HasMaxLength(511)
                    .HasColumnName("degree_granting_institution_name");

                entity.Property(e => e.DegreeNameEn)
                    .HasMaxLength(4000)
                    .HasColumnName("degree_name_en");

                entity.Property(e => e.DegreeNameFi)
                    .HasMaxLength(4000)
                    .HasColumnName("degree_name_fi");

                entity.Property(e => e.DegreeNameSv)
                    .HasMaxLength(4000)
                    .HasColumnName("degree_name_sv");

                entity.Property(e => e.DescriptionEn)
                    .HasMaxLength(4000)
                    .HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi)
                    .HasMaxLength(4000)
                    .HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv)
                    .HasMaxLength(4000)
                    .HasColumnName("description_sv");

                entity.Property(e => e.DimEndDate).HasColumnName("dim_end_date");

                entity.Property(e => e.DimInstructionLanguage).HasColumnName("dim_instruction_language");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.DimStartDate).HasColumnName("dim_start_date");

                entity.Property(e => e.LocalIdentifier).HasColumnName("local_identifier");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(4000)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(4000)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(4000)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimEndDateNavigation)
                    .WithMany(p => p.DimEducationDimEndDateNavigations)
                    .HasForeignKey(d => d.DimEndDate)
                    .HasConstraintName("education end");

                entity.HasOne(d => d.DimInstructionLanguageNavigation)
                    .WithMany(p => p.DimEducations)
                    .HasForeignKey(d => d.DimInstructionLanguage)
                    .HasConstraintName("FKdim_educat732488");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimEducations)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("education");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimEducations)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_educat81864");

                entity.HasOne(d => d.DimStartDateNavigation)
                    .WithMany(p => p.DimEducationDimStartDateNavigations)
                    .HasForeignKey(d => d.DimStartDate)
                    .HasConstraintName("education start");
            });

            modelBuilder.Entity<DimEmailAddrress>(entity =>
            {
                entity.ToTable("dim_email_addrress");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimEmailAddrresses)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_email_322546");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimEmailAddrresses)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_email_341396");
            });

            modelBuilder.Entity<DimEsfri>(entity =>
            {
                entity.ToTable("dim_esfri");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");
            });

            modelBuilder.Entity<DimEvent>(entity =>
            {
                entity.ToTable("dim_event");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimDateIdEndDate).HasColumnName("dim_date_id_end_date");

                entity.Property(e => e.DimDateIdStartDate).HasColumnName("dim_date_id_start_date");

                entity.Property(e => e.DimGeoIdEventCountry).HasColumnName("dim_geo_id_event_country");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.EventLocationText).HasColumnName("event_location_text");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.NameUnd)
                    .HasMaxLength(255)
                    .HasColumnName("name_und");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimDateIdEndDateNavigation)
                    .WithMany(p => p.DimEventDimDateIdEndDateNavigations)
                    .HasForeignKey(d => d.DimDateIdEndDate)
                    .HasConstraintName("end_date");

                entity.HasOne(d => d.DimDateIdStartDateNavigation)
                    .WithMany(p => p.DimEventDimDateIdStartDateNavigations)
                    .HasForeignKey(d => d.DimDateIdStartDate)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("start_date");

                entity.HasOne(d => d.DimGeoIdEventCountryNavigation)
                    .WithMany(p => p.DimEvents)
                    .HasForeignKey(d => d.DimGeoIdEventCountry)
                    .HasConstraintName("event_location");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimEvents)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_event492051");
            });

            modelBuilder.Entity<DimExternalService>(entity =>
            {
                entity.ToTable("dim_external_service");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimExternalServices)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_extern413099");
            });

            modelBuilder.Entity<DimFieldDisplaySetting>(entity =>
            {
                entity.ToTable("dim_field_display_settings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimUserProfileId).HasColumnName("dim_user_profile_id");

                entity.Property(e => e.FieldIdentifier).HasColumnName("field_identifier");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.Show).HasColumnName("show");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimUserProfile)
                    .WithMany(p => p.DimFieldDisplaySettings)
                    .HasForeignKey(d => d.DimUserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_field_653425");
            });

            modelBuilder.Entity<DimFieldOfArt>(entity =>
            {
                entity.ToTable("dim_field_of_art");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.FieldId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("field_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");
            });

            modelBuilder.Entity<DimFieldOfEducation>(entity =>
            {
                entity.ToTable("dim_field_of_education");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.FieldId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("field_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");
            });

            modelBuilder.Entity<DimFieldOfScience>(entity =>
            {
                entity.ToTable("dim_field_of_science");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.FieldId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("field_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");
            });

            modelBuilder.Entity<DimFieldOfScienceDimResearchActivity>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfScienceId, e.DimResearchActivityId })
                    .HasName("PK__dim_fiel__D251A58212473C45");

                entity.ToTable("dim_field_of_science_dim_research_activity");

                entity.Property(e => e.DimFieldOfScienceId).HasColumnName("dim_field_of_science_id");

                entity.Property(e => e.DimResearchActivityId).HasColumnName("dim_research_activity_id");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.DimFieldOfScienceDimResearchActivities)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_field_235508");

                entity.HasOne(d => d.DimResearchActivity)
                    .WithMany(p => p.DimFieldOfScienceDimResearchActivities)
                    .HasForeignKey(d => d.DimResearchActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_field_982235");
            });

            modelBuilder.Entity<DimFundingDecision>(entity =>
            {
                entity.ToTable("dim_funding_decision");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Acronym)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("acronym");

                entity.Property(e => e.AmountInEur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount_in_EUR");

                entity.Property(e => e.AmountInFundingDecisionCurrency)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount_in_funding_decision_currency");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionEn).HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi).HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv).HasColumnName("description_sv");

                entity.Property(e => e.DimCallProgrammeId).HasColumnName("dim_call_programme_id");

                entity.Property(e => e.DimDateIdApproval).HasColumnName("dim_date_id_approval");

                entity.Property(e => e.DimDateIdEnd).HasColumnName("dim_date_id_end");

                entity.Property(e => e.DimDateIdStart).HasColumnName("dim_date_id_start");

                entity.Property(e => e.DimFundingDecisionIdParentDecision).HasColumnName("dim_funding_decision_id_parent_decision");

                entity.Property(e => e.DimGeoId).HasColumnName("dim_geo_id");

                entity.Property(e => e.DimNameIdContactPerson).HasColumnName("dim_name_id_contact_person");

                entity.Property(e => e.DimOrganizationIdFunder).HasColumnName("dim_organization_id_funder");

                entity.Property(e => e.DimPidPidContent)
                    .HasMaxLength(255)
                    .HasColumnName("dim_pid_pid_content");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.DimTypeOfFundingId).HasColumnName("dim_type_of_funding_id");

                entity.Property(e => e.FunderProjectNumber)
                    .HasMaxLength(255)
                    .HasColumnName("funder_project_number");

                entity.Property(e => e.FundingDecisionCurrencyAbbreviation)
                    .HasMaxLength(255)
                    .HasColumnName("funding_decision_currency_abbreviation");

                entity.Property(e => e.HasBusinessCollaboration).HasColumnName("has_business_collaboration");

                entity.Property(e => e.HasInternationalCollaboration).HasColumnName("has_international_collaboration");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.NameUnd)
                    .HasMaxLength(255)
                    .HasColumnName("name_und");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimCallProgramme)
                    .WithMany(p => p.DimFundingDecisions)
                    .HasForeignKey(d => d.DimCallProgrammeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("programme");

                entity.HasOne(d => d.DimDateIdApprovalNavigation)
                    .WithMany(p => p.DimFundingDecisionDimDateIdApprovalNavigations)
                    .HasForeignKey(d => d.DimDateIdApproval)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("project start");

                entity.HasOne(d => d.DimDateIdEndNavigation)
                    .WithMany(p => p.DimFundingDecisionDimDateIdEndNavigations)
                    .HasForeignKey(d => d.DimDateIdEnd)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("approval");

                entity.HasOne(d => d.DimDateIdStartNavigation)
                    .WithMany(p => p.DimFundingDecisionDimDateIdStartNavigations)
                    .HasForeignKey(d => d.DimDateIdStart)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("project end");

                entity.HasOne(d => d.DimFundingDecisionIdParentDecisionNavigation)
                    .WithMany(p => p.InverseDimFundingDecisionIdParentDecisionNavigation)
                    .HasForeignKey(d => d.DimFundingDecisionIdParentDecision)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Parent decision");

                entity.HasOne(d => d.DimGeo)
                    .WithMany(p => p.DimFundingDecisions)
                    .HasForeignKey(d => d.DimGeoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Where work is to be carried out");

                entity.HasOne(d => d.DimNameIdContactPersonNavigation)
                    .WithMany(p => p.DimFundingDecisions)
                    .HasForeignKey(d => d.DimNameIdContactPerson)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("contact_person");

                entity.HasOne(d => d.DimOrganizationIdFunderNavigation)
                    .WithMany(p => p.DimFundingDecisions)
                    .HasForeignKey(d => d.DimOrganizationIdFunder)
                    .HasConstraintName("funder");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimFundingDecisions)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_fundin282559");

                entity.HasOne(d => d.DimTypeOfFunding)
                    .WithMany(p => p.DimFundingDecisions)
                    .HasForeignKey(d => d.DimTypeOfFundingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_fundin974924");
            });

            modelBuilder.Entity<DimGeo>(entity =>
            {
                entity.ToTable("dim_geo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(255)
                    .HasColumnName("country_code");

                entity.Property(e => e.CountryEn)
                    .HasMaxLength(255)
                    .HasColumnName("country_en");

                entity.Property(e => e.CountryFi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("country_fi");

                entity.Property(e => e.CountryId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("country_id");

                entity.Property(e => e.CountrySv)
                    .HasMaxLength(255)
                    .HasColumnName("country_sv");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.MunicipalityFi)
                    .HasMaxLength(255)
                    .HasColumnName("municipality_fi");

                entity.Property(e => e.MunicipalityId)
                    .HasMaxLength(255)
                    .HasColumnName("municipality_id");

                entity.Property(e => e.MunicipalitySv)
                    .HasMaxLength(255)
                    .HasColumnName("municipality_sv");

                entity.Property(e => e.RegionFi)
                    .HasMaxLength(255)
                    .HasColumnName("region_fi");

                entity.Property(e => e.RegionId)
                    .HasMaxLength(255)
                    .HasColumnName("region_id");

                entity.Property(e => e.RegionSv)
                    .HasMaxLength(255)
                    .HasColumnName("region_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");
            });

            modelBuilder.Entity<DimIdentifierlessDatum>(entity =>
            {
                entity.ToTable("dim_identifierless_data");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimIdentifierlessDataId).HasColumnName("dim_identifierless_data_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.Value)
                    .HasMaxLength(4000)
                    .HasColumnName("value");

                entity.HasOne(d => d.DimIdentifierlessData)
                    .WithMany(p => p.InverseDimIdentifierlessData)
                    .HasForeignKey(d => d.DimIdentifierlessDataId)
                    .HasConstraintName("parent_data");
            });

            modelBuilder.Entity<DimInfrastructure>(entity =>
            {
                entity.ToTable("dim_infrastructure");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Acronym)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("acronym");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionEn)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("description_sv");

                entity.Property(e => e.EndYear).HasColumnName("end_year");

                entity.Property(e => e.FinlandRoadmap).HasColumnName("finland_roadmap");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("name_sv");

                entity.Property(e => e.NextInfastructureId).HasColumnName("next_infastructure_id");

                entity.Property(e => e.ScientificDescriptionEn)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("scientific_description_en");

                entity.Property(e => e.ScientificDescriptionFi)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("scientific_description_fi");

                entity.Property(e => e.ScientificDescriptionSv)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("scientific_description_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.StartYear).HasColumnName("start_year");

                entity.Property(e => e.Urn)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("urn");

                entity.HasOne(d => d.NextInfastructure)
                    .WithMany(p => p.InverseNextInfastructure)
                    .HasForeignKey(d => d.NextInfastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_infras462742");
            });

            modelBuilder.Entity<DimKeyword>(entity =>
            {
                entity.ToTable("dim_keyword");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConceptUri)
                    .HasMaxLength(255)
                    .HasColumnName("concept_uri");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimKeywordCloseMatch).HasColumnName("dim_keyword_close_match");

                entity.Property(e => e.DimKeywordLanguageVariant).HasColumnName("dim_keyword_language_variant");

                entity.Property(e => e.DimKeywordRelated).HasColumnName("dim_keyword_related");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.Keyword)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("keyword");

                entity.Property(e => e.Language)
                    .HasMaxLength(255)
                    .HasColumnName("language");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.Scheme)
                    .HasMaxLength(255)
                    .HasColumnName("scheme");

                entity.Property(e => e.SchemeUri)
                    .HasMaxLength(255)
                    .HasColumnName("scheme_uri");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimKeywordCloseMatchNavigation)
                    .WithMany(p => p.InverseDimKeywordCloseMatchNavigation)
                    .HasForeignKey(d => d.DimKeywordCloseMatch)
                    .HasConstraintName("related");

                entity.HasOne(d => d.DimKeywordLanguageVariantNavigation)
                    .WithMany(p => p.InverseDimKeywordLanguageVariantNavigation)
                    .HasForeignKey(d => d.DimKeywordLanguageVariant)
                    .HasConstraintName("language_variant");

                entity.HasOne(d => d.DimKeywordRelatedNavigation)
                    .WithMany(p => p.InverseDimKeywordRelatedNavigation)
                    .HasForeignKey(d => d.DimKeywordRelated)
                    .HasConstraintName("close_match");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimKeywords)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_keywor723976");
            });

            modelBuilder.Entity<DimKnownPerson>(entity =>
            {
                entity.ToTable("dim_known_person");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.SourceProjectId)
                    .HasMaxLength(100)
                    .HasColumnName("source_project_id");
            });

            modelBuilder.Entity<DimKnownPersonDimFieldOfScience>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfScienceId, e.DimKnownPersonId })
                    .HasName("PK__dim_know__493EE076B9D35CA2");

                entity.ToTable("dim_known_person_dim_field_of_science");

                entity.Property(e => e.DimFieldOfScienceId).HasColumnName("dim_field_of_science_id");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.DimKnownPersonDimFieldOfSciences)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_known_609663");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimKnownPersonDimFieldOfSciences)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_known_232019");
            });

            modelBuilder.Entity<DimLocallyReportedPubInfo>(entity =>
            {
                entity.ToTable("dim_locally_reported_pub_info");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimPublicationid).HasColumnName("dim_publicationid");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SelfArchivedEmbargoDate)
                    .HasColumnType("datetime")
                    .HasColumnName("self_archived_embargo_date");

                entity.Property(e => e.SelfArchivedLicenseCode)
                    .HasMaxLength(1)
                    .HasColumnName("self_archived_license_code");

                entity.Property(e => e.SelfArchivedType)
                    .HasMaxLength(50)
                    .HasColumnName("self_archived_type");

                entity.Property(e => e.SelfArchivedUrl)
                    .HasMaxLength(400)
                    .HasColumnName("self_archived_url");

                entity.Property(e => e.SelfArchivedVersionCode)
                    .HasMaxLength(1)
                    .HasColumnName("self_archived_version_code");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.DimLocallyReportedPubInfos)
                    .HasForeignKey(d => d.DimPublicationid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_locall183416");
            });

            modelBuilder.Entity<DimMeril>(entity =>
            {
                entity.ToTable("dim_meril");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");
            });

            modelBuilder.Entity<DimName>(entity =>
            {
                entity.ToTable("dim_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimKnownPersonIdConfirmedIdentity).HasColumnName("dim_known_person_id_confirmed_identity");

                entity.Property(e => e.DimKnownPersonidFormerNames)
                    .HasColumnName("dim_known_personid_former_names")
                    .HasDefaultValueSql("((-1))");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.FirstNames)
                    .HasMaxLength(255)
                    .HasColumnName("first_names");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .HasColumnName("last_name");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.SourceProjectId)
                    .HasMaxLength(100)
                    .HasColumnName("source_project_id");

                entity.HasOne(d => d.DimKnownPersonIdConfirmedIdentityNavigation)
                    .WithMany(p => p.DimNameDimKnownPersonIdConfirmedIdentityNavigations)
                    .HasForeignKey(d => d.DimKnownPersonIdConfirmedIdentity)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("confirmed identity");

                entity.HasOne(d => d.DimKnownPersonidFormerNamesNavigation)
                    .WithMany(p => p.DimNameDimKnownPersonidFormerNamesNavigations)
                    .HasForeignKey(d => d.DimKnownPersonidFormerNames)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("former names");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimNames)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_name798039");
            });

            modelBuilder.Entity<DimNewsFeed>(entity =>
            {
                entity.ToTable("dim_news_feed");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.FeedUrl)
                    .HasMaxLength(4000)
                    .HasColumnName("feed_url");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.Title)
                    .HasMaxLength(511)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<DimNewsItem>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.DimNewsFeedid })
                    .HasName("PK__dim_news__B87E6703099306E5");

                entity.ToTable("dim_news_item");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.DimNewsFeedid).HasColumnName("dim_news_feedid");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NewsContent).HasColumnName("news_content");

                entity.Property(e => e.NewsHeadline).HasColumnName("news_headline");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("timestamp");

                entity.Property(e => e.Url)
                    .HasMaxLength(4000)
                    .HasColumnName("url");

                entity.HasOne(d => d.DimNewsFeed)
                    .WithMany(p => p.DimNewsItems)
                    .HasForeignKey(d => d.DimNewsFeedid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_news_i691810");
            });

            modelBuilder.Entity<DimOrcidPublication>(entity =>
            {
                entity.ToTable("dim_orcid_publication");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ArticleNumberText)
                    .HasMaxLength(255)
                    .HasColumnName("article_number_text");

                entity.Property(e => e.ArticleTypeCode).HasColumnName("article_type_code");

                entity.Property(e => e.AuthorsText)
                    .IsRequired()
                    .HasColumnName("authors_text");

                entity.Property(e => e.ConferenceName)
                    .HasMaxLength(4000)
                    .HasColumnName("conference_name");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimParentOrcidPublicationId).HasColumnName("dim_parent_orcid_publication id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.DoiHandle)
                    .HasMaxLength(4000)
                    .HasColumnName("doi_handle");

                entity.Property(e => e.IssueNumber)
                    .HasMaxLength(255)
                    .HasColumnName("issue_number");

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(255)
                    .HasColumnName("language_code");

                entity.Property(e => e.LicenseCode).HasColumnName("license_code");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NumberOfAuthors).HasColumnName("number_of_authors");

                entity.Property(e => e.OpenAccessCode)
                    .HasMaxLength(255)
                    .HasColumnName("open_access_code");

                entity.Property(e => e.OrcidPersonDataSource).HasColumnName("orcid_person_data_source");

                entity.Property(e => e.OrcidWorkType)
                    .HasMaxLength(255)
                    .HasColumnName("orcid_work_type");

                entity.Property(e => e.OriginalPublicationId)
                    .HasMaxLength(255)
                    .HasColumnName("original_publication_id");

                entity.Property(e => e.PageNumberText)
                    .HasMaxLength(255)
                    .HasColumnName("page_number_text");

                entity.Property(e => e.ParentPublicationEditors)
                    .HasMaxLength(4000)
                    .HasColumnName("parent_publication_editors");

                entity.Property(e => e.ParentPublicationName)
                    .HasMaxLength(4000)
                    .HasColumnName("parent_publication_name");

                entity.Property(e => e.ParentPublicationTypeCode).HasColumnName("parent_publication_type_code");

                entity.Property(e => e.PeerReviewed).HasColumnName("peer_reviewed");

                entity.Property(e => e.PublicationCountryCode)
                    .HasMaxLength(255)
                    .HasColumnName("publication_country_code");

                entity.Property(e => e.PublicationId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("publication_id");

                entity.Property(e => e.PublicationName)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasColumnName("publication_name");

                entity.Property(e => e.PublicationTypeCode).HasColumnName("publication_type_code");

                entity.Property(e => e.PublicationTypeCode2).HasColumnName("publication_type_code2");

                entity.Property(e => e.PublicationYear).HasColumnName("publication_year");

                entity.Property(e => e.PublisherLocation)
                    .HasMaxLength(255)
                    .HasColumnName("publisher_location");

                entity.Property(e => e.PublisherName)
                    .HasMaxLength(4000)
                    .HasColumnName("publisher_name");

                entity.Property(e => e.Report).HasColumnName("report");

                entity.Property(e => e.ShortDescription).HasColumnName("short_description");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.TargetAudienceCode).HasColumnName("target_audience_code");

                entity.Property(e => e.ThesisTypeCode).HasColumnName("thesis_type_code");

                entity.Property(e => e.Volume)
                    .HasMaxLength(255)
                    .HasColumnName("volume");

                entity.HasOne(d => d.ArticleTypeCodeNavigation)
                    .WithMany(p => p.DimOrcidPublicationArticleTypeCodeNavigations)
                    .HasForeignKey(d => d.ArticleTypeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Or_article_type_code");

                entity.HasOne(d => d.DimParentOrcidPublication)
                    .WithMany(p => p.InverseDimParentOrcidPublication)
                    .HasForeignKey(d => d.DimParentOrcidPublicationId)
                    .HasConstraintName("parent orcid publication");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimOrcidPublications)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_orcid_729203");

                entity.HasOne(d => d.OrcidPersonDataSourceNavigation)
                    .WithMany(p => p.DimOrcidPublications)
                    .HasForeignKey(d => d.OrcidPersonDataSource)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_orcid_438595");

                entity.HasOne(d => d.ParentPublicationTypeCodeNavigation)
                    .WithMany(p => p.DimOrcidPublicationParentPublicationTypeCodeNavigations)
                    .HasForeignKey(d => d.ParentPublicationTypeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Or_parent_publication_type_code");

                entity.HasOne(d => d.PublicationTypeCodeNavigation)
                    .WithMany(p => p.DimOrcidPublicationPublicationTypeCodeNavigations)
                    .HasForeignKey(d => d.PublicationTypeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("publication_type_code");

                entity.HasOne(d => d.PublicationTypeCode2Navigation)
                    .WithMany(p => p.DimOrcidPublicationPublicationTypeCode2Navigations)
                    .HasForeignKey(d => d.PublicationTypeCode2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Or_publication_type_code2");

                entity.HasOne(d => d.TargetAudienceCodeNavigation)
                    .WithMany(p => p.DimOrcidPublicationTargetAudienceCodeNavigations)
                    .HasForeignKey(d => d.TargetAudienceCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Or_target_audience_code");
            });

            modelBuilder.Entity<DimOrganisationMedium>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("dim_organisation_media");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.LanguageVariant)
                    .HasMaxLength(255)
                    .HasColumnName("language_variant");

                entity.Property(e => e.MediaUri)
                    .HasMaxLength(511)
                    .HasColumnName("media_uri");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany()
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_organi623202");
            });

            modelBuilder.Entity<DimOrganization>(entity =>
            {
                entity.ToTable("dim_organization");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(2)
                    .HasColumnName("country_code");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DegreeCountBsc).HasColumnName("degree_count_bsc");

                entity.Property(e => e.DegreeCountLic).HasColumnName("degree_count_lic");

                entity.Property(e => e.DegreeCountMsc).HasColumnName("degree_count_msc");

                entity.Property(e => e.DegreeCountPhd).HasColumnName("degree_count_phd");

                entity.Property(e => e.DimOrganizationBroader).HasColumnName("dim_organization_broader");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.DimSectorid).HasColumnName("dim_sectorid");

                entity.Property(e => e.Established)
                    .HasColumnType("datetime")
                    .HasColumnName("established");

                entity.Property(e => e.LocalOrganizationSector)
                    .HasMaxLength(255)
                    .HasColumnName("local_organization_sector");

                entity.Property(e => e.LocalOrganizationUnitId)
                    .HasMaxLength(255)
                    .HasColumnName("local_organization_unit_Id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.NameUnd)
                    .HasMaxLength(255)
                    .HasColumnName("name_und");

                entity.Property(e => e.NameVariants)
                    .HasMaxLength(1023)
                    .HasColumnName("name_variants");

                entity.Property(e => e.OrganizationActive).HasColumnName("organization_active");

                entity.Property(e => e.OrganizationBackground)
                    .HasMaxLength(4000)
                    .HasColumnName("organization_background");

                entity.Property(e => e.OrganizationId)
                    .HasMaxLength(255)
                    .HasColumnName("organization_id");

                entity.Property(e => e.OrganizationType)
                    .HasMaxLength(255)
                    .HasColumnName("organization_type");

                entity.Property(e => e.PostalAddress)
                    .HasMaxLength(511)
                    .HasColumnName("postal_address");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.StaffCountAsFte).HasColumnName("staff_count_as_fte");

                entity.Property(e => e.VisitingAddress)
                    .HasMaxLength(4000)
                    .HasColumnName("visiting_address");

                entity.HasOne(d => d.DimOrganizationBroaderNavigation)
                    .WithMany(p => p.InverseDimOrganizationBroaderNavigation)
                    .HasForeignKey(d => d.DimOrganizationBroader)
                    .HasConstraintName("is_part_of");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimOrganizations)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_organi972074");

                entity.HasOne(d => d.DimSector)
                    .WithMany(p => p.DimOrganizations)
                    .HasForeignKey(d => d.DimSectorid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_organi330513");
            });

            modelBuilder.Entity<DimPid>(entity =>
            {
                entity.ToTable("dim_pid");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimEventId).HasColumnName("dim_event_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimOrcidPublicationId).HasColumnName("dim_orcid_publication_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimPublicationChannelId).HasColumnName("dim_publication_channel_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.Property(e => e.DimResearchActivityId).HasColumnName("dim_research_activity_id");

                entity.Property(e => e.DimResearchDataCatalogId).HasColumnName("dim_research_data_catalog_id");

                entity.Property(e => e.DimResearchDatasetId).HasColumnName("dim_research_dataset_id");

                entity.Property(e => e.DimServiceId).HasColumnName("dim_service_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.PidContent)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("pid_content");

                entity.Property(e => e.PidType)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("pid_type");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimEvent)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("event_identifier");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ROI/RAID");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("URN/infrastructure");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Orcid/ISNI");

                entity.HasOne(d => d.DimOrcidPublication)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimOrcidPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ll");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ISNI/GRID/ROR/Business-ID\\PIC");

                entity.HasOne(d => d.DimPublicationChannel)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimPublicationChannelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("mostly ISSN");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DOI/ISBN1-2");

                entity.HasOne(d => d.DimResearchActivity)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimResearchActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_pid725718");

                entity.HasOne(d => d.DimResearchDataCatalog)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimResearchDataCatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_pid400266");

                entity.HasOne(d => d.DimService)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("URN/service");
            });

            modelBuilder.Entity<DimPublication>(entity =>
            {
                entity.ToTable("dim_publication");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApcFeeEur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("apc_fee_EUR");

                entity.Property(e => e.ApcPaymentYear).HasColumnName("apc_payment_year");

                entity.Property(e => e.ArticleNumberText)
                    .HasMaxLength(255)
                    .HasColumnName("article_number_text");

                entity.Property(e => e.ArticleTypeCode).HasColumnName("article_type_code");

                entity.Property(e => e.AuthorsText)
                    .IsRequired()
                    .HasColumnName("authors_text");

                entity.Property(e => e.BusinessCollaboration).HasColumnName("business_collaboration");

                entity.Property(e => e.ConferenceName)
                    .HasMaxLength(4000)
                    .HasColumnName("conference_name");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.Doi)
                    .HasMaxLength(4000)
                    .HasColumnName("doi");

                entity.Property(e => e.DoiHandle)
                    .HasMaxLength(4000)
                    .HasColumnName("doi_handle");

                entity.Property(e => e.GovermentCollaboration).HasColumnName("goverment_collaboration");

                entity.Property(e => e.HospitalDistrictCollaboration).HasColumnName("hospital_district_collaboration");

                entity.Property(e => e.InternationalCollaboration).HasColumnName("international_collaboration");

                entity.Property(e => e.InternationalPublication).HasColumnName("international_publication");

                entity.Property(e => e.Isbn)
                    .HasMaxLength(255)
                    .HasColumnName("isbn");

                entity.Property(e => e.Isbn2)
                    .HasMaxLength(255)
                    .HasColumnName("isbn2");

                entity.Property(e => e.Issn)
                    .HasMaxLength(255)
                    .HasColumnName("issn");

                entity.Property(e => e.Issn2)
                    .HasMaxLength(255)
                    .HasColumnName("issn2");

                entity.Property(e => e.IssueNumber)
                    .HasMaxLength(255)
                    .HasColumnName("issue_number");

                entity.Property(e => e.JournalName)
                    .HasMaxLength(4000)
                    .HasColumnName("journal_name");

                entity.Property(e => e.JufoClassCode)
                    .HasMaxLength(255)
                    .HasColumnName("jufo_class_code");

                entity.Property(e => e.JufoCode)
                    .HasMaxLength(255)
                    .HasColumnName("jufo_code");

                entity.Property(e => e.JuuliAddress)
                    .HasMaxLength(4000)
                    .HasColumnName("juuli_address");

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(255)
                    .HasColumnName("language_code");

                entity.Property(e => e.LicenseCode).HasColumnName("license_code");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NumberOfAuthors).HasColumnName("number_of_authors");

                entity.Property(e => e.OpenAccessCode)
                    .HasMaxLength(255)
                    .HasColumnName("open_access_code");

                entity.Property(e => e.OriginalPublicationId)
                    .HasMaxLength(255)
                    .HasColumnName("original_publication_id");

                entity.Property(e => e.OtherCollaboration).HasColumnName("other_collaboration");

                entity.Property(e => e.PageNumberText)
                    .HasMaxLength(255)
                    .HasColumnName("page_number_text");

                entity.Property(e => e.ParentPublicationName)
                    .HasMaxLength(4000)
                    .HasColumnName("parent_publication_name");

                entity.Property(e => e.ParentPublicationPublisher)
                    .HasMaxLength(4000)
                    .HasColumnName("parent_publication_publisher");

                entity.Property(e => e.ParentPublicationTypeCode).HasColumnName("parent_publication_type_code");

                entity.Property(e => e.PeerReviewed).HasColumnName("peer_reviewed");

                entity.Property(e => e.PublicationCountryCode)
                    .HasMaxLength(255)
                    .HasColumnName("publication_country_code");

                entity.Property(e => e.PublicationId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("publication_id");

                entity.Property(e => e.PublicationName)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasColumnName("publication_name");

                entity.Property(e => e.PublicationOrgId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("publication_org_id");

                entity.Property(e => e.PublicationStatusCode)
                    .HasMaxLength(255)
                    .HasColumnName("publication_status_code");

                entity.Property(e => e.PublicationTypeCode)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("publication_type_code");

                entity.Property(e => e.PublicationTypeCode2).HasColumnName("publication_type_code2");

                entity.Property(e => e.PublicationYear).HasColumnName("publication_year");

                entity.Property(e => e.PublisherLocation)
                    .HasMaxLength(255)
                    .HasColumnName("publisher_location");

                entity.Property(e => e.PublisherName)
                    .HasMaxLength(4000)
                    .HasColumnName("publisher_name");

                entity.Property(e => e.Report).HasColumnName("report");

                entity.Property(e => e.ReportingYear).HasColumnName("reporting_year");

                entity.Property(e => e.SelfArchivedCode)
                    .HasMaxLength(255)
                    .HasColumnName("self_archived_code");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.SpecialStateSubsidy).HasColumnName("special_state_subsidy");

                entity.Property(e => e.TargetAudienceCode).HasColumnName("target_audience_code");

                entity.Property(e => e.ThesisTypeCode).HasColumnName("thesis_type_code");

                entity.Property(e => e.Volume)
                    .HasMaxLength(255)
                    .HasColumnName("volume");

                entity.HasOne(d => d.ArticleTypeCodeNavigation)
                    .WithMany(p => p.DimPublicationArticleTypeCodeNavigations)
                    .HasForeignKey(d => d.ArticleTypeCode)
                    .HasConstraintName("article_type_code");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimPublications)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_public896887");

                entity.HasOne(d => d.ParentPublicationTypeCodeNavigation)
                    .WithMany(p => p.DimPublicationParentPublicationTypeCodeNavigations)
                    .HasForeignKey(d => d.ParentPublicationTypeCode)
                    .HasConstraintName("parent_publication_type_code");

                entity.HasOne(d => d.PublicationTypeCode2Navigation)
                    .WithMany(p => p.DimPublicationPublicationTypeCode2Navigations)
                    .HasForeignKey(d => d.PublicationTypeCode2)
                    .HasConstraintName("publication_type_code2");

                entity.HasOne(d => d.TargetAudienceCodeNavigation)
                    .WithMany(p => p.DimPublicationTargetAudienceCodeNavigations)
                    .HasForeignKey(d => d.TargetAudienceCode)
                    .HasConstraintName("target_audience_code");
            });

            modelBuilder.Entity<DimPublicationChannel>(entity =>
            {
                entity.ToTable("dim_publication_channel");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChannelNameAnylang)
                    .HasMaxLength(4000)
                    .HasColumnName("channel_name_anylang");

                entity.Property(e => e.JufoCode)
                    .HasMaxLength(255)
                    .HasColumnName("jufo_code");

                entity.Property(e => e.PublisherNameText)
                    .HasMaxLength(4000)
                    .HasColumnName("publisher_name_text");
            });

            modelBuilder.Entity<DimReferencedatum>(entity =>
            {
                entity.ToTable("dim_referencedata");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CodeScheme)
                    .IsRequired()
                    .HasMaxLength(511)
                    .HasColumnName("code_scheme");

                entity.Property(e => e.CodeValue)
                    .IsRequired()
                    .HasMaxLength(511)
                    .HasColumnName("code_value");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.State)
                    .HasMaxLength(255)
                    .HasColumnName("state");
            });

            modelBuilder.Entity<DimRegisteredDataSource>(entity =>
            {
                entity.ToTable("dim_registered_data_source");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimRegisteredDataSources)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("data source of organisation");
            });

            modelBuilder.Entity<DimResearchActivity>(entity =>
            {
                entity.ToTable("dim_research_activity");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionEn).HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi).HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv).HasColumnName("description_sv");

                entity.Property(e => e.DimCountryCode).HasColumnName("dim_country_code");

                entity.Property(e => e.DimEndDate).HasColumnName("dim_end_date");

                entity.Property(e => e.DimEventId).HasColumnName("dim_event_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimPublicationChannelId).HasColumnName("dim_publication_channel_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.DimStartDate).HasColumnName("dim_start_date");

                entity.Property(e => e.IndentifierlessTargetOrg)
                    .HasMaxLength(511)
                    .HasColumnName("indentifierless_target_org");

                entity.Property(e => e.InternationalCollaboration).HasColumnName("international_collaboration");

                entity.Property(e => e.LocalIdentifier)
                    .HasMaxLength(255)
                    .HasColumnName("local_identifier");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.NameUnd)
                    .HasMaxLength(255)
                    .HasColumnName("name_und");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimCountryCodeNavigation)
                    .WithMany(p => p.DimResearchActivities)
                    .HasForeignKey(d => d.DimCountryCode)
                    .HasConstraintName("FKdim_resear758241");

                entity.HasOne(d => d.DimEndDateNavigation)
                    .WithMany(p => p.DimResearchActivityDimEndDateNavigations)
                    .HasForeignKey(d => d.DimEndDate)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear682769");

                entity.HasOne(d => d.DimEvent)
                    .WithMany(p => p.DimResearchActivities)
                    .HasForeignKey(d => d.DimEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Activity Context ");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimResearchActivities)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear921843");

                entity.HasOne(d => d.DimPublicationChannel)
                    .WithMany(p => p.DimResearchActivities)
                    .HasForeignKey(d => d.DimPublicationChannelId)
                    .HasConstraintName("FKdim_resear832055");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimResearchActivities)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear37345");

                entity.HasOne(d => d.DimStartDateNavigation)
                    .WithMany(p => p.DimResearchActivityDimStartDateNavigations)
                    .HasForeignKey(d => d.DimStartDate)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear797546");
            });

            modelBuilder.Entity<DimResearchActivityDimKeyword>(entity =>
            {
                entity.HasKey(e => new { e.DimResearchActivityId, e.DimKeywordId })
                    .HasName("PK__dim_rese__F7B536BC194A32D5");

                entity.ToTable("dim_research_activity_dim_keyword");

                entity.Property(e => e.DimResearchActivityId).HasColumnName("dim_research_activity_id");

                entity.Property(e => e.DimKeywordId).HasColumnName("dim_keyword_id");

                entity.HasOne(d => d.DimResearchActivity)
                    .WithMany(p => p.DimResearchActivityDimKeywords)
                    .HasForeignKey(d => d.DimResearchActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear970214");
            });

            modelBuilder.Entity<DimResearchCommunity>(entity =>
            {
                entity.ToTable("dim_research_community");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Acronym)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("acronym");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionEn).HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi).HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv).HasColumnName("description_sv");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.NameUnd)
                    .HasMaxLength(255)
                    .HasColumnName("name_und");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimResearchCommunities)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear59027");
            });

            modelBuilder.Entity<DimResearchDataCatalog>(entity =>
            {
                entity.ToTable("dim_research_data_catalog");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionEn).HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi).HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv).HasColumnName("description_sv");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(4000)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(4000)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(4000)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");
            });

            modelBuilder.Entity<DimResearchDataset>(entity =>
            {
                entity.ToTable("dim_research_dataset");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DatasetCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("dataset_created");

                entity.Property(e => e.DatasetModified)
                    .HasColumnType("datetime")
                    .HasColumnName("dataset_modified");

                entity.Property(e => e.DescriptionEn).HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi).HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv).HasColumnName("description_sv");

                entity.Property(e => e.DescriptionUnd).HasColumnName("description_und");

                entity.Property(e => e.DimReferencedataAvailability).HasColumnName("dim_referencedata_availability");

                entity.Property(e => e.DimReferencedataLicense).HasColumnName("dim_referencedata_license");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.DimResearchDataCatalogId).HasColumnName("dim_research_data_catalog_id");

                entity.Property(e => e.DimResearchDatasetId).HasColumnName("dim_research_dataset_id");

                entity.Property(e => e.GeographicCoverage)
                    .HasMaxLength(4000)
                    .HasColumnName("geographic_coverage");

                entity.Property(e => e.InternationalCollaboration).HasColumnName("international_collaboration");

                entity.Property(e => e.LocalIdentifier)
                    .HasMaxLength(255)
                    .HasColumnName("local_identifier");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(4000)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(4000)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(4000)
                    .HasColumnName("name_sv");

                entity.Property(e => e.NameUnd)
                    .HasMaxLength(4000)
                    .HasColumnName("name_und");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.TemporalCoverageEnd)
                    .HasColumnType("datetime")
                    .HasColumnName("temporal_coverage_end");

                entity.Property(e => e.TemporalCoverageStart)
                    .HasColumnType("datetime")
                    .HasColumnName("temporal_coverage_start");

                entity.HasOne(d => d.DimReferencedataAvailabilityNavigation)
                    .WithMany(p => p.DimResearchDatasetDimReferencedataAvailabilityNavigations)
                    .HasForeignKey(d => d.DimReferencedataAvailability)
                    .HasConstraintName("Availibiity classes");

                entity.HasOne(d => d.DimReferencedataLicenseNavigation)
                    .WithMany(p => p.DimResearchDatasetDimReferencedataLicenseNavigations)
                    .HasForeignKey(d => d.DimReferencedataLicense)
                    .HasConstraintName("License");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimResearchDatasets)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear76083");

                entity.HasOne(d => d.DimResearchDataCatalog)
                    .WithMany(p => p.DimResearchDatasets)
                    .HasForeignKey(d => d.DimResearchDataCatalogId)
                    .HasConstraintName("FKdim_resear753411");

                entity.HasOne(d => d.DimResearchDatasetNavigation)
                    .WithMany(p => p.InverseDimResearchDatasetNavigation)
                    .HasForeignKey(d => d.DimResearchDatasetId)
                    .HasConstraintName("part of dataset");
            });

            modelBuilder.Entity<DimResearcherDescription>(entity =>
            {
                entity.ToTable("dim_researcher_description");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionType).HasColumnName("description_type");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.ResearchDescriptionEn).HasColumnName("research_description_en");

                entity.Property(e => e.ResearchDescriptionFi).HasColumnName("research_description_fi");

                entity.Property(e => e.ResearchDescriptionSv).HasColumnName("research_description_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimResearcherDescriptions)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear662094");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimResearcherDescriptions)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear1848");
            });

            modelBuilder.Entity<DimResearcherToResearchCommunity>(entity =>
            {
                entity.ToTable("dim_researcher_to_research_community");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionEn)
                    .HasMaxLength(511)
                    .HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi)
                    .HasMaxLength(511)
                    .HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv)
                    .HasMaxLength(511)
                    .HasColumnName("description_sv");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.DimResearchCommunityId).HasColumnName("dim_research_community_id");

                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimResearcherToResearchCommunities)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear255903");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimResearcherToResearchCommunities)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear408039");

                entity.HasOne(d => d.DimResearchCommunity)
                    .WithMany(p => p.DimResearcherToResearchCommunities)
                    .HasForeignKey(d => d.DimResearchCommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear3824");

                entity.HasOne(d => d.EndDateNavigation)
                    .WithMany(p => p.DimResearcherToResearchCommunityEndDateNavigations)
                    .HasForeignKey(d => d.EndDate)
                    .HasConstraintName("FKdim_resear658483");

                entity.HasOne(d => d.StartDateNavigation)
                    .WithMany(p => p.DimResearcherToResearchCommunityStartDateNavigations)
                    .HasForeignKey(d => d.StartDate)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_resear612809");
            });

            modelBuilder.Entity<DimSector>(entity =>
            {
                entity.ToTable("dim_sector");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SectorId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("sector_id");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");
            });

            modelBuilder.Entity<DimService>(entity =>
            {
                entity.ToTable("dim_service");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Acronym)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("acronym");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionEn)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("description_sv");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("name_sv");

                entity.Property(e => e.ScientificDescriptionEn)
                    .HasMaxLength(4000)
                    .HasColumnName("scientific_description_en");

                entity.Property(e => e.ScientificDescriptionFi)
                    .HasMaxLength(4000)
                    .HasColumnName("scientific_description_fi");

                entity.Property(e => e.ScientificDescriptionSv)
                    .HasMaxLength(4000)
                    .HasColumnName("scientific_description_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<DimServicePoint>(entity =>
            {
                entity.ToTable("dim_service_point");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DescriptionEn)
                    .HasMaxLength(4000)
                    .HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi)
                    .HasMaxLength(4000)
                    .HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv)
                    .HasMaxLength(4000)
                    .HasColumnName("description_sv");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.LinkAccessPolicy)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("link_access_policy");

                entity.Property(e => e.LinkAdditionalInfo)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("link_additional_info");

                entity.Property(e => e.LinkInternationalInfra)
                    .HasMaxLength(4000)
                    .HasColumnName("link_international_infra");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(4000)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .HasMaxLength(4000)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(4000)
                    .HasColumnName("name_sv");

                entity.Property(e => e.Phone)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.VisitingAddress)
                    .HasMaxLength(4000)
                    .HasColumnName("visiting_address");
            });

            modelBuilder.Entity<DimTelephoneNumber>(entity =>
            {
                entity.ToTable("dim_telephone_number");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.TelephoneNumber)
                    .HasMaxLength(255)
                    .HasColumnName("telephone_number");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimTelephoneNumbers)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_teleph963809");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.DimTelephoneNumbers)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_teleph299867");
            });

            modelBuilder.Entity<DimTypeOfFunding>(entity =>
            {
                entity.ToTable("dim_type_of_funding");

                entity.HasIndex(e => e.TypeId, "UQ__dim_type__2C000599B555DD80")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimTypeOfFundingId).HasColumnName("dim_type_of_funding_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name_fi");

                entity.Property(e => e.NameSv)
                    .HasMaxLength(255)
                    .HasColumnName("name_sv");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.TypeId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("type_id");

                entity.HasOne(d => d.DimTypeOfFundingNavigation)
                    .WithMany(p => p.InverseDimTypeOfFundingNavigation)
                    .HasForeignKey(d => d.DimTypeOfFundingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("part of ");
            });

            modelBuilder.Entity<DimUserProfile>(entity =>
            {
                entity.ToTable("dim_user_profile");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AllowAllSubscriptions).HasColumnName("allow_all_subscriptions");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimUserProfiles)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_user_p611467");
            });

            modelBuilder.Entity<DimWebLink>(entity =>
            {
                entity.ToTable("dim_web_link");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimCallProgrammeId).HasColumnName("dim_call_programme_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimResearchCommunityId).HasColumnName("dim_research_community_id");

                entity.Property(e => e.DimResearchDataCatalogId).HasColumnName("dim_research_data_catalog_id");

                entity.Property(e => e.DimResearchDatasetId).HasColumnName("dim_research_dataset_id");

                entity.Property(e => e.LanguageVariant)
                    .HasMaxLength(255)
                    .HasColumnName("language_variant");

                entity.Property(e => e.LinkLabel)
                    .HasMaxLength(255)
                    .HasColumnName("link_label");

                entity.Property(e => e.LinkType)
                    .HasMaxLength(255)
                    .HasColumnName("link_type");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.Property(e => e.Url)
                    .HasMaxLength(511)
                    .HasColumnName("url");

                entity.HasOne(d => d.DimCallProgramme)
                    .WithMany(p => p.DimWebLinks)
                    .HasForeignKey(d => d.DimCallProgrammeId)
                    .HasConstraintName("call homepage");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.DimWebLinks)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .HasConstraintName("FKdim_web_li388345");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimWebLinks)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .HasConstraintName("web presence");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimWebLinks)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .HasConstraintName("language specific homepage");

                entity.HasOne(d => d.DimResearchCommunity)
                    .WithMany(p => p.DimWebLinks)
                    .HasForeignKey(d => d.DimResearchCommunityId)
                    .HasConstraintName("FKdim_web_li827668");

                entity.HasOne(d => d.DimResearchDataCatalog)
                    .WithMany(p => p.DimWebLinks)
                    .HasForeignKey(d => d.DimResearchDataCatalogId)
                    .HasConstraintName("FKdim_web_li597610");

                entity.HasOne(d => d.DimResearchDataset)
                    .WithMany(p => p.DimWebLinks)
                    .HasForeignKey(d => d.DimResearchDatasetId)
                    .HasConstraintName("fairdata_weblink");
            });

            modelBuilder.Entity<FactContribution>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionId, e.DimOrganizationId, e.DimDateId, e.DimNameId, e.DimPublicationId, e.DimGeoId, e.DimInfrastructureId, e.DimNewsFeedId, e.DimResearchDatasetId, e.DimResearchDataCatalogId, e.DimIdentifierlessDataId, e.DimResearchActivityId, e.DimResearchCommunityId, e.DimReferencedataActorRoleId })
                    .HasName("PK__fact_con__B7D7E1B5CCE1D2AA");

                entity.ToTable("fact_contribution");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimDateId).HasColumnName("dim_date_id");

                entity.Property(e => e.DimNameId).HasColumnName("dim_name_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.Property(e => e.DimGeoId).HasColumnName("dim_geo_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.DimNewsFeedId).HasColumnName("dim_news_feed_id");

                entity.Property(e => e.DimResearchDatasetId).HasColumnName("dim_research_dataset_id");

                entity.Property(e => e.DimResearchDataCatalogId).HasColumnName("dim_research_data_catalog_id");

                entity.Property(e => e.DimIdentifierlessDataId).HasColumnName("dim_identifierless_data_id");

                entity.Property(e => e.DimResearchActivityId).HasColumnName("dim_research_activity_id");

                entity.Property(e => e.DimResearchCommunityId).HasColumnName("dim_research_community_id");

                entity.Property(e => e.DimReferencedataActorRoleId).HasColumnName("dim_referencedata_actor_role_id");

                entity.Property(e => e.ContributionType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("contribution_type");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimDate)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimDateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr224183");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr321314");

                entity.HasOne(d => d.DimGeo)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimGeoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr711408");

                entity.HasOne(d => d.DimIdentifierlessData)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimIdentifierlessDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr276794");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr565219");

                entity.HasOne(d => d.DimName)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimNameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr288640");

                entity.HasOne(d => d.DimNewsFeed)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimNewsFeedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr632925");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr42449");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr481795");

                entity.HasOne(d => d.DimReferencedataActorRole)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimReferencedataActorRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr466649");

                entity.HasOne(d => d.DimResearchActivity)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimResearchActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activity - person -affiliation AND activity - funding decision ");

                entity.HasOne(d => d.DimResearchCommunity)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimResearchCommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr434262");

                entity.HasOne(d => d.DimResearchDataCatalog)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimResearchDataCatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr859541");

                entity.HasOne(d => d.DimResearchDataset)
                    .WithMany(p => p.FactContributions)
                    .HasForeignKey(d => d.DimResearchDatasetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("authors, infras, publications, funding decisions");
            });

            modelBuilder.Entity<FactFieldValue>(entity =>
            {
                entity.HasKey(e => new { e.DimUserProfileId, e.DimFieldDisplaySettingsId, e.DimNameId, e.DimWebLinkId, e.DimFundingDecisionId, e.DimPublicationId, e.DimPidId, e.DimPidIdOrcidPutCode, e.DimResearchActivityId, e.DimEventId, e.DimEducationId, e.DimCompetenceId, e.DimResearchCommunityId, e.DimTelephoneNumberId, e.DimEmailAddrressId, e.DimResearcherDescriptionId, e.DimIdentifierlessDataId, e.DimOrcidPublicationId, e.DimKeywordId, e.DimAffiliationId, e.DimResearcherToResearchCommunityId, e.DimFieldOfScienceId });

                entity.ToTable("fact_field_values");

                entity.Property(e => e.DimUserProfileId).HasColumnName("dim_user_profile_id");

                entity.Property(e => e.DimFieldDisplaySettingsId).HasColumnName("dim_field_display_settings_id");

                entity.Property(e => e.DimNameId).HasColumnName("dim_name_id");

                entity.Property(e => e.DimWebLinkId).HasColumnName("dim_web_link_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.Property(e => e.DimPidId).HasColumnName("dim_pid_id");

                entity.Property(e => e.DimPidIdOrcidPutCode).HasColumnName("dim_pid_id_orcid_put_code");

                entity.Property(e => e.DimResearchActivityId).HasColumnName("dim_research_activity_id");

                entity.Property(e => e.DimEventId).HasColumnName("dim_event_id");

                entity.Property(e => e.DimEducationId).HasColumnName("dim_education_id");

                entity.Property(e => e.DimCompetenceId).HasColumnName("dim_competence_id");

                entity.Property(e => e.DimResearchCommunityId).HasColumnName("dim_research_community_id");

                entity.Property(e => e.DimTelephoneNumberId).HasColumnName("dim_telephone_number_id");

                entity.Property(e => e.DimEmailAddrressId).HasColumnName("dim_email_addrress_id");

                entity.Property(e => e.DimResearcherDescriptionId).HasColumnName("dim_researcher_description_id");

                entity.Property(e => e.DimIdentifierlessDataId).HasColumnName("dim_identifierless_data_id");

                entity.Property(e => e.DimOrcidPublicationId).HasColumnName("dim_orcid_publication_id");

                entity.Property(e => e.DimKeywordId).HasColumnName("dim_keyword_id");

                entity.Property(e => e.DimAffiliationId).HasColumnName("dim_affiliation_id");

                entity.Property(e => e.DimResearcherToResearchCommunityId).HasColumnName("dim_researcher_to_research_community_id");

                entity.Property(e => e.DimFieldOfScienceId)
                    .HasColumnName("dim_field_of_science_id")
                    .HasDefaultValueSql("((-1))");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.PrimaryValue).HasColumnName("primary_value");

                entity.Property(e => e.Show).HasColumnName("show");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimAffiliation)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimAffiliationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field236121");

                entity.HasOne(d => d.DimCompetence)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimCompetenceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field746305");

                entity.HasOne(d => d.DimEducation)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimEducationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field445913");

                entity.HasOne(d => d.DimEmailAddrress)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimEmailAddrressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field507571");

                entity.HasOne(d => d.DimEvent)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field645906");

                entity.HasOne(d => d.DimFieldDisplaySettings)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimFieldDisplaySettingsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("field content settings");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field1234567");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field5141");

                entity.HasOne(d => d.DimIdentifierlessData)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimIdentifierlessDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field39379");

                entity.HasOne(d => d.DimKeyword)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimKeywordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field786713");

                entity.HasOne(d => d.DimName)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimNameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field604813");

                entity.HasOne(d => d.DimOrcidPublication)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimOrcidPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field692932");

                entity.HasOne(d => d.DimPid)
                    .WithMany(p => p.FactFieldValueDimPids)
                    .HasForeignKey(d => d.DimPidId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field989816");

                entity.HasOne(d => d.DimPidIdOrcidPutCodeNavigation)
                    .WithMany(p => p.FactFieldValueDimPidIdOrcidPutCodeNavigations)
                    .HasForeignKey(d => d.DimPidIdOrcidPutCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("orcid_put_code");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("displayed publications");

                entity.HasOne(d => d.DimResearchActivity)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimResearchActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field878671");

                entity.HasOne(d => d.DimResearchCommunity)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimResearchCommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field750435");

                entity.HasOne(d => d.DimResearcherDescription)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimResearcherDescriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field76121");

                entity.HasOne(d => d.DimResearcherToResearchCommunity)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimResearcherToResearchCommunityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field377489");

                entity.HasOne(d => d.DimTelephoneNumber)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimTelephoneNumberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field995052");

                entity.HasOne(d => d.DimUserProfile)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimUserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field936263");

                entity.HasOne(d => d.DimWebLink)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimWebLinkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field475402");
            });

            modelBuilder.Entity<FactInfraKeyword>(entity =>
            {
                entity.HasKey(e => new { e.DimKeywordId, e.DimServiceId, e.DimServicePointId, e.DimInfrastructureId })
                    .HasName("PK__fact_inf__3C29B680CB1233FF");

                entity.ToTable("fact_infra_keywords");

                entity.Property(e => e.DimKeywordId).HasColumnName("dim_keyword_id");

                entity.Property(e => e.DimServiceId).HasColumnName("dim_service_id");

                entity.Property(e => e.DimServicePointId).HasColumnName("dim_service_point_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.FactInfraKeywords)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_infra800296");

                entity.HasOne(d => d.DimKeyword)
                    .WithMany(p => p.FactInfraKeywords)
                    .HasForeignKey(d => d.DimKeywordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_infra76615");

                entity.HasOne(d => d.DimService)
                    .WithMany(p => p.FactInfraKeywords)
                    .HasForeignKey(d => d.DimServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_infra505599");

                entity.HasOne(d => d.DimServicePoint)
                    .WithMany(p => p.FactInfraKeywords)
                    .HasForeignKey(d => d.DimServicePointId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_infra619117");
            });

            modelBuilder.Entity<FactJufoClassCodesForPubChannel>(entity =>
            {
                entity.HasKey(e => new { e.DimPublicationChannelId, e.DimReferencedataId, e.Year })
                    .HasName("PK__fact_juf__0E099E4BEF49D978");

                entity.ToTable("fact_jufo_class_codes_for_pub_channels");

                entity.Property(e => e.DimPublicationChannelId).HasColumnName("dim_publication_channel_id");

                entity.Property(e => e.DimReferencedataId).HasColumnName("dim_referencedata_id");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.DimPublicationChannel)
                    .WithMany(p => p.FactJufoClassCodesForPubChannels)
                    .HasForeignKey(d => d.DimPublicationChannelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("jufo_classes");

                entity.HasOne(d => d.DimReferencedata)
                    .WithMany(p => p.FactJufoClassCodesForPubChannels)
                    .HasForeignKey(d => d.DimReferencedataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_jufo_876058");
            });

            modelBuilder.Entity<FactUpkeep>(entity =>
            {
                entity.HasKey(e => new { e.DimOrganizationId, e.DimGeoId, e.DimInfrastructureId, e.DimServiceId, e.DimServicePointId, e.DimDateIdStart, e.DimDateIdEnd })
                    .HasName("PK__fact_upk__850A8E304254F728");

                entity.ToTable("fact_upkeep");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimGeoId).HasColumnName("dim_geo_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.DimServiceId).HasColumnName("dim_service_id");

                entity.Property(e => e.DimServicePointId).HasColumnName("dim_service_point_id");

                entity.Property(e => e.DimDateIdStart).HasColumnName("dim_date_id_start");

                entity.Property(e => e.DimDateIdEnd).HasColumnName("dim_date_id_end");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.SourceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("source_description");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_id");

                entity.HasOne(d => d.DimDateIdEndNavigation)
                    .WithMany(p => p.FactUpkeepDimDateIdEndNavigations)
                    .HasForeignKey(d => d.DimDateIdEnd)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee332759");

                entity.HasOne(d => d.DimDateIdStartNavigation)
                    .WithMany(p => p.FactUpkeepDimDateIdStartNavigations)
                    .HasForeignKey(d => d.DimDateIdStart)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee283512");

                entity.HasOne(d => d.DimGeo)
                    .WithMany(p => p.FactUpkeeps)
                    .HasForeignKey(d => d.DimGeoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee520794");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.FactUpkeeps)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee374605");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.FactUpkeeps)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee851834");

                entity.HasOne(d => d.DimService)
                    .WithMany(p => p.FactUpkeeps)
                    .HasForeignKey(d => d.DimServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee302288");

                entity.HasOne(d => d.DimServicePoint)
                    .WithMany(p => p.FactUpkeeps)
                    .HasForeignKey(d => d.DimServicePointId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee415806");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

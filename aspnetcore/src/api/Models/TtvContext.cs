using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace api.Models
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

        public virtual DbSet<BrCallProgrammeDimCallProgramme> BrCallProgrammeDimCallProgramme { get; set; }
        public virtual DbSet<BrDimAuroraHakualatDimCallProgramme> BrDimAuroraHakualatDimCallProgramme { get; set; }
        public virtual DbSet<BrEsfriDimInfrastructure> BrEsfriDimInfrastructure { get; set; }
        public virtual DbSet<BrFieldOfArtDimPublication> BrFieldOfArtDimPublication { get; set; }
        public virtual DbSet<BrFieldOfEducationDimPublication> BrFieldOfEducationDimPublication { get; set; }
        public virtual DbSet<BrFieldOfScienceDimFundingDecision> BrFieldOfScienceDimFundingDecision { get; set; }
        public virtual DbSet<BrFieldOfScienceDimPublication> BrFieldOfScienceDimPublication { get; set; }
        public virtual DbSet<BrFundingConsortiumParticipation> BrFundingConsortiumParticipation { get; set; }
        public virtual DbSet<BrFundingDecisionDimFieldOfArt> BrFundingDecisionDimFieldOfArt { get; set; }
        public virtual DbSet<BrInfrastructureDimFieldOfScience> BrInfrastructureDimFieldOfScience { get; set; }
        public virtual DbSet<BrKeywordDimFundingDecision> BrKeywordDimFundingDecision { get; set; }
        public virtual DbSet<BrKeywordDimPublication> BrKeywordDimPublication { get; set; }
        public virtual DbSet<BrLanguageCodesForDatasets> BrLanguageCodesForDatasets { get; set; }
        public virtual DbSet<BrMerilDimInfrastructure> BrMerilDimInfrastructure { get; set; }
        public virtual DbSet<BrOrganizationsFundCallProgrammes> BrOrganizationsFundCallProgrammes { get; set; }
        public virtual DbSet<BrParticipatesInFundingGroup> BrParticipatesInFundingGroup { get; set; }
        public virtual DbSet<BrPredecessorOrganization> BrPredecessorOrganization { get; set; }
        public virtual DbSet<BrPreviousFundingDecision> BrPreviousFundingDecision { get; set; }
        public virtual DbSet<BrRelatedFundingDecision> BrRelatedFundingDecision { get; set; }
        public virtual DbSet<BrResearchDatasetDimFieldOfScience> BrResearchDatasetDimFieldOfScience { get; set; }
        public virtual DbSet<BrServiceSubscription> BrServiceSubscription { get; set; }
        public virtual DbSet<BrSuccessorOrganization> BrSuccessorOrganization { get; set; }
        public virtual DbSet<DimAuroraDisciplines> DimAuroraDisciplines { get; set; }
        public virtual DbSet<DimCallProgramme> DimCallProgramme { get; set; }
        public virtual DbSet<DimDate> DimDate { get; set; }
        public virtual DbSet<DimEsfri> DimEsfri { get; set; }
        public virtual DbSet<DimExternalService> DimExternalService { get; set; }
        public virtual DbSet<DimFieldDisplaySettings> DimFieldDisplaySettings { get; set; }
        public virtual DbSet<DimFieldOfArt> DimFieldOfArt { get; set; }
        public virtual DbSet<DimFieldOfEducation> DimFieldOfEducation { get; set; }
        public virtual DbSet<DimFieldOfScience> DimFieldOfScience { get; set; }
        public virtual DbSet<DimFundingDecision> DimFundingDecision { get; set; }
        public virtual DbSet<DimGeo> DimGeo { get; set; }
        public virtual DbSet<DimInfrastructure> DimInfrastructure { get; set; }
        public virtual DbSet<DimKeyword> DimKeyword { get; set; }
        public virtual DbSet<DimKnownPerson> DimKnownPerson { get; set; }
        public virtual DbSet<DimKnownPersonDimFieldOfScience> DimKnownPersonDimFieldOfScience { get; set; }
        public virtual DbSet<DimLocallyReportedPubInfo> DimLocallyReportedPubInfo { get; set; }
        public virtual DbSet<DimMeril> DimMeril { get; set; }
        public virtual DbSet<DimName> DimName { get; set; }
        public virtual DbSet<DimNewsFeed> DimNewsFeed { get; set; }
        public virtual DbSet<DimNewsItem> DimNewsItem { get; set; }
        public virtual DbSet<DimOrganisationMedia> DimOrganisationMedia { get; set; }
        public virtual DbSet<DimOrganization> DimOrganization { get; set; }
        public virtual DbSet<DimPid> DimPid { get; set; }
        public virtual DbSet<DimPublication> DimPublication { get; set; }
        public virtual DbSet<DimPublicationChannel> DimPublicationChannel { get; set; }
        public virtual DbSet<DimReferencedata> DimReferencedata { get; set; }
        public virtual DbSet<DimRegisteredDataSource> DimRegisteredDataSource { get; set; }
        public virtual DbSet<DimResearchDataset> DimResearchDataset { get; set; }
        public virtual DbSet<DimSector> DimSector { get; set; }
        public virtual DbSet<DimService> DimService { get; set; }
        public virtual DbSet<DimServicePoint> DimServicePoint { get; set; }
        public virtual DbSet<DimTypeOfFunding> DimTypeOfFunding { get; set; }
        public virtual DbSet<DimUserProfile> DimUserProfile { get; set; }
        public virtual DbSet<DimWebLink> DimWebLink { get; set; }
        public virtual DbSet<FactContribution> FactContribution { get; set; }
        public virtual DbSet<FactFieldDisplayContent> FactFieldDisplayContent { get; set; }
        public virtual DbSet<FactInfraKeywords> FactInfraKeywords { get; set; }
        public virtual DbSet<FactJufoClassCodesForPubChannels> FactJufoClassCodesForPubChannels { get; set; }
        public virtual DbSet<FactUpkeep> FactUpkeep { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost;User Id=sa;Password=Test1234;database=Ttv;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrCallProgrammeDimCallProgramme>(entity =>
            {
                entity.HasKey(e => new { e.DimCallProgrammeId, e.DimCallProgrammeId2 })
                    .HasName("PK__br_call___6F0CEDFB0EB0F253");

                entity.ToTable("br_call_programme_dim_call_programme");

                entity.Property(e => e.DimCallProgrammeId).HasColumnName("dim_call_programme_id");

                entity.Property(e => e.DimCallProgrammeId2).HasColumnName("dim_call_programme_id2");

                entity.HasOne(d => d.DimCallProgramme)
                    .WithMany(p => p.BrCallProgrammeDimCallProgrammeDimCallProgramme)
                    .HasForeignKey(d => d.DimCallProgrammeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("belongs to / a part of ");

                entity.HasOne(d => d.DimCallProgrammeId2Navigation)
                    .WithMany(p => p.BrCallProgrammeDimCallProgrammeDimCallProgrammeId2Navigation)
                    .HasForeignKey(d => d.DimCallProgrammeId2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_call_pr785575");
            });

            modelBuilder.Entity<BrDimAuroraHakualatDimCallProgramme>(entity =>
            {
                entity.HasKey(e => new { e.DimAuroraHakualatid, e.DimCallProgrammeid })
                    .HasName("PK__br_dim_a__758276F95B090FD7");

                entity.ToTable("br_dim_aurora_hakualat_dim_call_programme");

                entity.Property(e => e.DimAuroraHakualatid).HasColumnName("dim_aurora_hakualatid");

                entity.Property(e => e.DimCallProgrammeid).HasColumnName("dim_call_programmeid");

                entity.HasOne(d => d.DimAuroraHakualat)
                    .WithMany(p => p.BrDimAuroraHakualatDimCallProgramme)
                    .HasForeignKey(d => d.DimAuroraHakualatid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_dim_aur835602");

                entity.HasOne(d => d.DimCallProgramme)
                    .WithMany(p => p.BrDimAuroraHakualatDimCallProgramme)
                    .HasForeignKey(d => d.DimCallProgrammeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("has disciplines");
            });

            modelBuilder.Entity<BrEsfriDimInfrastructure>(entity =>
            {
                entity.HasKey(e => new { e.DimEsfriId, e.DimInfrastructureId })
                    .HasName("PK__br_esfri__A4A0FE1096BDB0CE");

                entity.ToTable("br_esfri_dim_infrastructure");

                entity.Property(e => e.DimEsfriId).HasColumnName("dim_esfri_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.HasOne(d => d.DimEsfri)
                    .WithMany(p => p.BrEsfriDimInfrastructure)
                    .HasForeignKey(d => d.DimEsfriId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_esfri_d559740");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.BrEsfriDimInfrastructure)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_esfri_d490989");
            });

            modelBuilder.Entity<BrFieldOfArtDimPublication>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfArtId, e.DimPublicationId })
                    .HasName("PK__br_field__809A87CD56820415");

                entity.ToTable("br_field_of_art_dim_publication");

                entity.Property(e => e.DimFieldOfArtId).HasColumnName("dim_field_of_art_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.HasOne(d => d.DimFieldOfArt)
                    .WithMany(p => p.BrFieldOfArtDimPublication)
                    .HasForeignKey(d => d.DimFieldOfArtId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o978876");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.BrFieldOfArtDimPublication)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o937704");
            });

            modelBuilder.Entity<BrFieldOfEducationDimPublication>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfEducationId, e.DimPublicationId })
                    .HasName("PK__br_field__6E377B2C4E03BCAC");

                entity.ToTable("br_field_of_education_dim_publication");

                entity.Property(e => e.DimFieldOfEducationId).HasColumnName("dim_field_of_education_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.HasOne(d => d.DimFieldOfEducation)
                    .WithMany(p => p.BrFieldOfEducationDimPublication)
                    .HasForeignKey(d => d.DimFieldOfEducationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o983513");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.BrFieldOfEducationDimPublication)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o881968");
            });

            modelBuilder.Entity<BrFieldOfScienceDimFundingDecision>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfScienceId, e.DimFundingDecisionId })
                    .HasName("PK__br_field__1A103AF7095158EE");

                entity.ToTable("br_field_of_science_dim_funding_decision");

                entity.Property(e => e.DimFieldOfScienceId).HasColumnName("dim_field_of_science_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.BrFieldOfScienceDimFundingDecision)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o643706");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrFieldOfScienceDimFundingDecision)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o993952");
            });

            modelBuilder.Entity<BrFieldOfScienceDimPublication>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfScienceId, e.DimPublicationId })
                    .HasName("PK__br_field__5088B776BF6758A1");

                entity.ToTable("br_field_of_science_dim_publication");

                entity.Property(e => e.DimFieldOfScienceId).HasColumnName("dim_field_of_science_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.BrFieldOfScienceDimPublication)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o934749");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.BrFieldOfScienceDimPublication)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_o769299");
            });

            modelBuilder.Entity<BrFundingConsortiumParticipation>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionId, e.DimOrganizationid })
                    .HasName("PK__br_fundi__3DB567F80637931C");

                entity.ToTable("br_funding_consortium_participation");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimOrganizationid).HasColumnName("dim_organizationid");

                entity.Property(e => e.RoleInConsortium)
                    .HasColumnName("role_in_consortium")
                    .HasMaxLength(255);

                entity.Property(e => e.ShareOfFundingInEur)
                    .HasColumnName("share_of_funding_in_EUR")
                    .HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrFundingConsortiumParticipation)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_funding504308");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.BrFundingConsortiumParticipation)
                    .HasForeignKey(d => d.DimOrganizationid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_funding503907");
            });

            modelBuilder.Entity<BrFundingDecisionDimFieldOfArt>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionId, e.DimFieldOfArtId })
                    .HasName("PK__br_fundi__07CB586D85E2D3E0");

                entity.ToTable("br_funding_decision_dim_field_of_art");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimFieldOfArtId).HasColumnName("dim_field_of_art_id");

                entity.HasOne(d => d.DimFieldOfArt)
                    .WithMany(p => p.BrFundingDecisionDimFieldOfArt)
                    .HasForeignKey(d => d.DimFieldOfArtId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_funding154428");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrFundingDecisionDimFieldOfArt)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_funding281737");
            });

            modelBuilder.Entity<BrInfrastructureDimFieldOfScience>(entity =>
            {
                entity.HasKey(e => new { e.DimInfrastructureId, e.DimFieldOfScienceId })
                    .HasName("PK__br_infra__17B77C1693970E26");

                entity.ToTable("br_infrastructure_dim_field_of_science");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.DimFieldOfScienceId).HasColumnName("dim_field_of_science_id");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.BrInfrastructureDimFieldOfScience)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_infrast565984");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.BrInfrastructureDimFieldOfScience)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_infrast156732");
            });

            modelBuilder.Entity<BrKeywordDimFundingDecision>(entity =>
            {
                entity.HasKey(e => new { e.DimKeywordId, e.DimFundingDecisionId })
                    .HasName("PK__br_keywo__8C7B929B09F0A0A9");

                entity.ToTable("br_keyword_dim_funding_decision");

                entity.Property(e => e.DimKeywordId).HasColumnName("dim_keyword_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrKeywordDimFundingDecision)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_keyword955130");

                entity.HasOne(d => d.DimKeyword)
                    .WithMany(p => p.BrKeywordDimFundingDecision)
                    .HasForeignKey(d => d.DimKeywordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_keyword224605");
            });

            modelBuilder.Entity<BrKeywordDimPublication>(entity =>
            {
                entity.HasKey(e => new { e.DimKeywordId, e.DimPublicationId })
                    .HasName("PK__br_keywo__C6E31F1A97B5C24C");

                entity.ToTable("br_keyword_dim_publication");

                entity.Property(e => e.DimKeywordId).HasColumnName("dim_keyword_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.HasOne(d => d.DimKeyword)
                    .WithMany(p => p.BrKeywordDimPublication)
                    .HasForeignKey(d => d.DimKeywordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_keyword944303");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.BrKeywordDimPublication)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_keyword66951");
            });

            modelBuilder.Entity<BrLanguageCodesForDatasets>(entity =>
            {
                entity.HasKey(e => new { e.DimResearchDatasetId, e.DimReferencedataId })
                    .HasName("PK__br_langu__576647BFF8E03FB8");

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
                    .HasName("PK__br_meril__A30C54DA7A2FA46A");

                entity.ToTable("br_meril_dim_infrastructure");

                entity.Property(e => e.DimMerilId).HasColumnName("dim_meril_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.BrMerilDimInfrastructure)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_meril_d901766");

                entity.HasOne(d => d.DimMeril)
                    .WithMany(p => p.BrMerilDimInfrastructure)
                    .HasForeignKey(d => d.DimMerilId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_meril_d209645");
            });

            modelBuilder.Entity<BrOrganizationsFundCallProgrammes>(entity =>
            {
                entity.HasKey(e => new { e.DimOrganizationid, e.DimCallProgrammeid })
                    .HasName("PK__br_organ__10F219BCBC99046E");

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
                    .HasName("PK__br_parti__5EC9BC6496B63505");

                entity.ToTable("br_participates_in_funding_group");

                entity.Property(e => e.DimFundingDecisionid).HasColumnName("dim_funding_decisionid");

                entity.Property(e => e.DimNameId).HasColumnName("dim_name_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.RoleInFundingGroup)
                    .HasColumnName("role_in_funding_group")
                    .HasMaxLength(255);

                entity.Property(e => e.ShareOfFundingInEur)
                    .HasColumnName("share_of_funding_in_EUR")
                    .HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.BrParticipatesInFundingGroup)
                    .HasForeignKey(d => d.DimFundingDecisionid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_partici137682");

                entity.HasOne(d => d.DimName)
                    .WithMany(p => p.BrParticipatesInFundingGroup)
                    .HasForeignKey(d => d.DimNameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_partici869162");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.BrParticipatesInFundingGroup)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("get_funding_in_name_of_org");
            });

            modelBuilder.Entity<BrPredecessorOrganization>(entity =>
            {
                entity.HasKey(e => new { e.DimOrganizationid, e.DimOrganizationid2 })
                    .HasName("PK__br_prede__A7CAD2F421167FCD");

                entity.ToTable("br_predecessor_organization");

                entity.Property(e => e.DimOrganizationid).HasColumnName("dim_organizationid");

                entity.Property(e => e.DimOrganizationid2).HasColumnName("dim_organizationid2");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.BrPredecessorOrganizationDimOrganization)
                    .HasForeignKey(d => d.DimOrganizationid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_predece849307");

                entity.HasOne(d => d.DimOrganizationid2Navigation)
                    .WithMany(p => p.BrPredecessorOrganizationDimOrganizationid2Navigation)
                    .HasForeignKey(d => d.DimOrganizationid2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_predece505451");
            });

            modelBuilder.Entity<BrPreviousFundingDecision>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionFromId, e.DimFundingDecisionToId })
                    .HasName("PK__br_previ__90966491E1E97D99");

                entity.ToTable("br_previous_funding_decision");

                entity.Property(e => e.DimFundingDecisionFromId).HasColumnName("dim_funding_decision_from_id");

                entity.Property(e => e.DimFundingDecisionToId).HasColumnName("dim_funding_decision_to_id");

                entity.HasOne(d => d.DimFundingDecisionFrom)
                    .WithMany(p => p.BrPreviousFundingDecisionDimFundingDecisionFrom)
                    .HasForeignKey(d => d.DimFundingDecisionFromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_previou481541");

                entity.HasOne(d => d.DimFundingDecisionTo)
                    .WithMany(p => p.BrPreviousFundingDecisionDimFundingDecisionTo)
                    .HasForeignKey(d => d.DimFundingDecisionToId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_previou440746");
            });

            modelBuilder.Entity<BrRelatedFundingDecision>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionFromId, e.DimFundingDecisionToId })
                    .HasName("PK__br_relat__909664913E7F50EA");

                entity.ToTable("br_related_funding_decision");

                entity.Property(e => e.DimFundingDecisionFromId).HasColumnName("dim_funding_decision_from_id");

                entity.Property(e => e.DimFundingDecisionToId).HasColumnName("dim_funding_decision_to_id");

                entity.HasOne(d => d.DimFundingDecisionFrom)
                    .WithMany(p => p.BrRelatedFundingDecisionDimFundingDecisionFrom)
                    .HasForeignKey(d => d.DimFundingDecisionFromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_related232364");

                entity.HasOne(d => d.DimFundingDecisionTo)
                    .WithMany(p => p.BrRelatedFundingDecisionDimFundingDecisionTo)
                    .HasForeignKey(d => d.DimFundingDecisionToId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_related689923");
            });

            modelBuilder.Entity<BrResearchDatasetDimFieldOfScience>(entity =>
            {
                entity.HasKey(e => new { e.DimResearchDatasetid, e.DimFieldOfScienceid })
                    .HasName("PK__br_resea__ADD3384BE6AAD2A2");

                entity.ToTable("br_research_dataset_dim_field_of_science");

                entity.Property(e => e.DimResearchDatasetid).HasColumnName("dim_research_datasetid");

                entity.Property(e => e.DimFieldOfScienceid).HasColumnName("dim_field_of_scienceid");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.BrResearchDatasetDimFieldOfScience)
                    .HasForeignKey(d => d.DimFieldOfScienceid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_researc385994");

                entity.HasOne(d => d.DimResearchDataset)
                    .WithMany(p => p.BrResearchDatasetDimFieldOfScience)
                    .HasForeignKey(d => d.DimResearchDatasetid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_researc927326");
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
                    .HasName("PK__br_succe__A7CAD2F45E8D7D05");

                entity.ToTable("br_successor organization");

                entity.Property(e => e.DimOrganizationid).HasColumnName("dim_organizationid");

                entity.Property(e => e.DimOrganizationid2).HasColumnName("dim_organizationid2");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.BrSuccessorOrganizationDimOrganization)
                    .HasForeignKey(d => d.DimOrganizationid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_success452227");

                entity.HasOne(d => d.DimOrganizationid2Navigation)
                    .WithMany(p => p.BrSuccessorOrganizationDimOrganizationid2Navigation)
                    .HasForeignKey(d => d.DimOrganizationid2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_success902531");
            });

            modelBuilder.Entity<DimAuroraDisciplines>(entity =>
            {
                entity.ToTable("dim_aurora_disciplines");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(511);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(511);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimCallProgramme>(entity =>
            {
                entity.ToTable("dim_call_programme");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Abbreviation)
                    .HasColumnName("abbreviation")
                    .HasMaxLength(511);

                entity.Property(e => e.ApplicationTermsEn).HasColumnName("application_terms_en");

                entity.Property(e => e.ApplicationTermsFi).HasColumnName("application_terms_fi");

                entity.Property(e => e.ApplicationTermsSv).HasColumnName("application_terms_sv");

                entity.Property(e => e.ContactInformation).HasColumnName("contact_information");

                entity.Property(e => e.ContinuosApplicationPeriod).HasColumnName("continuos_application _period");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DescriptionEn).HasColumnName("description_en");

                entity.Property(e => e.DescriptionFi).HasColumnName("description_fi");

                entity.Property(e => e.DescriptionSv).HasColumnName("description_sv");

                entity.Property(e => e.DimDateIdDue).HasColumnName("dim_date_id_due");

                entity.Property(e => e.DimDateIdOpen).HasColumnName("dim_date_id_open");

                entity.Property(e => e.EuCallId)
                    .HasColumnName("eu_call_id")
                    .HasMaxLength(511);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(511);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(511);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(511);

                entity.Property(e => e.NameUnd)
                    .HasColumnName("name_und")
                    .HasMaxLength(511);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimDateIdDueNavigation)
                    .WithMany(p => p.DimCallProgrammeDimDateIdDueNavigation)
                    .HasForeignKey(d => d.DimDateIdDue)
                    .HasConstraintName("open	");

                entity.HasOne(d => d.DimDateIdOpenNavigation)
                    .WithMany(p => p.DimCallProgrammeDimDateIdOpenNavigation)
                    .HasForeignKey(d => d.DimDateIdOpen)
                    .HasConstraintName("close");
            });

            modelBuilder.Entity<DimDate>(entity =>
            {
                entity.ToTable("dim_date");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Day).HasColumnName("day");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Month).HasColumnName("month");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<DimEsfri>(entity =>
            {
                entity.ToTable("dim_esfri");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimExternalService>(entity =>
            {
                entity.ToTable("dim_external_service");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimExternalService)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_extern413099");
            });

            modelBuilder.Entity<DimFieldDisplaySettings>(entity =>
            {
                entity.ToTable("dim_field_display_settings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimUserProfileId).HasColumnName("dim_user_profile_id");

                entity.Property(e => e.FieldIdentifier).HasColumnName("field_identifier");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Show).HasColumnName("show");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

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
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.FieldId)
                    .IsRequired()
                    .HasColumnName("field_id")
                    .HasMaxLength(255);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255);

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasColumnName("name_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimFieldOfEducation>(entity =>
            {
                entity.ToTable("dim_field_of_education");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.FieldId)
                    .IsRequired()
                    .HasColumnName("field_id")
                    .HasMaxLength(255);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255);

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasColumnName("name_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimFieldOfScience>(entity =>
            {
                entity.ToTable("dim_field_of_science");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.FieldId)
                    .IsRequired()
                    .HasColumnName("field_id")
                    .HasMaxLength(255);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255);

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasColumnName("name_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimFundingDecision>(entity =>
            {
                entity.ToTable("dim_funding_decision");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Acronym)
                    .HasColumnName("acronym")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.AmountInEur)
                    .HasColumnName("amount_in_EUR")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AmountInFundingDecisionCurrency)
                    .HasColumnName("amount_in_funding_decision_currency")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

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

                entity.Property(e => e.DimTypeOfFundingId).HasColumnName("dim_type_of_funding_id");

                entity.Property(e => e.FunderProjectNumber)
                    .HasColumnName("funder_project_number")
                    .HasMaxLength(255);

                entity.Property(e => e.FundingDecisionCurrencyAbbreviation)
                    .HasColumnName("funding_decision_currency_abbreviation")
                    .HasMaxLength(255);

                entity.Property(e => e.HasBusinessCollaboration).HasColumnName("has_business_collaboration");

                entity.Property(e => e.HasInternationalCollaboration).HasColumnName("has_international_collaboration");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.NameUnd)
                    .HasColumnName("name_und")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimCallProgramme)
                    .WithMany(p => p.DimFundingDecision)
                    .HasForeignKey(d => d.DimCallProgrammeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("programme");

                entity.HasOne(d => d.DimDateIdApprovalNavigation)
                    .WithMany(p => p.DimFundingDecisionDimDateIdApprovalNavigation)
                    .HasForeignKey(d => d.DimDateIdApproval)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("project start");

                entity.HasOne(d => d.DimDateIdEndNavigation)
                    .WithMany(p => p.DimFundingDecisionDimDateIdEndNavigation)
                    .HasForeignKey(d => d.DimDateIdEnd)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("approval");

                entity.HasOne(d => d.DimDateIdStartNavigation)
                    .WithMany(p => p.DimFundingDecisionDimDateIdStartNavigation)
                    .HasForeignKey(d => d.DimDateIdStart)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("project end");

                entity.HasOne(d => d.DimFundingDecisionIdParentDecisionNavigation)
                    .WithMany(p => p.InverseDimFundingDecisionIdParentDecisionNavigation)
                    .HasForeignKey(d => d.DimFundingDecisionIdParentDecision)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Parent decision");

                entity.HasOne(d => d.DimGeo)
                    .WithMany(p => p.DimFundingDecision)
                    .HasForeignKey(d => d.DimGeoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Where work is to be carried out");

                entity.HasOne(d => d.DimNameIdContactPersonNavigation)
                    .WithMany(p => p.DimFundingDecision)
                    .HasForeignKey(d => d.DimNameIdContactPerson)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("contact_person");

                entity.HasOne(d => d.DimOrganizationIdFunderNavigation)
                    .WithMany(p => p.DimFundingDecision)
                    .HasForeignKey(d => d.DimOrganizationIdFunder)
                    .HasConstraintName("funder");

                entity.HasOne(d => d.DimTypeOfFunding)
                    .WithMany(p => p.DimFundingDecision)
                    .HasForeignKey(d => d.DimTypeOfFundingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_fundin974924");
            });

            modelBuilder.Entity<DimGeo>(entity =>
            {
                entity.ToTable("dim_geo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountryCode)
                    .HasColumnName("country_code")
                    .HasMaxLength(255);

                entity.Property(e => e.CountryEn)
                    .HasColumnName("country_en")
                    .HasMaxLength(255);

                entity.Property(e => e.CountryFi)
                    .IsRequired()
                    .HasColumnName("country_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.CountryId)
                    .IsRequired()
                    .HasColumnName("country_id")
                    .HasMaxLength(255);

                entity.Property(e => e.CountrySv)
                    .HasColumnName("country_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.MunicipalityFi)
                    .HasColumnName("municipality_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.MunicipalityId)
                    .HasColumnName("municipality_id")
                    .HasMaxLength(255);

                entity.Property(e => e.MunicipalitySv)
                    .HasColumnName("municipality_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.RegionFi)
                    .HasColumnName("region_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.RegionId)
                    .HasColumnName("region_id")
                    .HasMaxLength(255);

                entity.Property(e => e.RegionSv)
                    .HasColumnName("region_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimInfrastructure>(entity =>
            {
                entity.ToTable("dim_infrastructure");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Acronym)
                    .HasColumnName("acronym")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DescriptionEn)
                    .HasColumnName("description_en")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.DescriptionFi)
                    .HasColumnName("description_fi")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.DescriptionSv)
                    .HasColumnName("description_sv")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.EndYear).HasColumnName("end_year");

                entity.Property(e => e.FinlandRoadmap).HasColumnName("finland_roadmap");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.NextInfastructureId).HasColumnName("next_infastructure_id");

                entity.Property(e => e.ScientificDescriptionEn)
                    .HasColumnName("scientific_description_en")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.ScientificDescriptionFi)
                    .HasColumnName("scientific_description_fi")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.ScientificDescriptionSv)
                    .HasColumnName("scientific_description_sv")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.StartYear).HasColumnName("start_year");

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
                    .HasColumnName("concept_uri")
                    .HasMaxLength(255);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimKeywordCloseMatch).HasColumnName("dim_keyword_close_match");

                entity.Property(e => e.DimKeywordLanguageVariant).HasColumnName("dim_keyword_language_variant");

                entity.Property(e => e.DimKeywordRelated).HasColumnName("dim_keyword_related");

                entity.Property(e => e.Keyword)
                    .IsRequired()
                    .HasColumnName("keyword")
                    .HasMaxLength(255);

                entity.Property(e => e.Language)
                    .HasColumnName("language")
                    .HasMaxLength(255);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Scheme)
                    .HasColumnName("scheme")
                    .HasMaxLength(255);

                entity.Property(e => e.SchemeUri)
                    .HasColumnName("scheme_uri")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

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
            });

            modelBuilder.Entity<DimKnownPerson>(entity =>
            {
                entity.ToTable("dim_known_person");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.ResearchDescription).HasColumnName("research_description");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimKnownPersonDimFieldOfScience>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldOfScienceId, e.DimKnownPersonId })
                    .HasName("PK__dim_know__493EE07692F82401");

                entity.ToTable("dim_known_person_dim_field_of_science");

                entity.Property(e => e.DimFieldOfScienceId).HasColumnName("dim_field_of_science_id");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.HasOne(d => d.DimFieldOfScience)
                    .WithMany(p => p.DimKnownPersonDimFieldOfScience)
                    .HasForeignKey(d => d.DimFieldOfScienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_known_609663");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimKnownPersonDimFieldOfScience)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_known_232019");
            });

            modelBuilder.Entity<DimLocallyReportedPubInfo>(entity =>
            {
                entity.ToTable("dim_locally_reported_pub_info");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimPublicationid).HasColumnName("dim_publicationid");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SelfArchivedEmbargoDate)
                    .HasColumnName("self_archived_embargo_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.SelfArchivedLicenseCode)
                    .HasColumnName("self_archived_license_code")
                    .HasMaxLength(1);

                entity.Property(e => e.SelfArchivedType)
                    .HasColumnName("self_archived_type")
                    .HasMaxLength(50);

                entity.Property(e => e.SelfArchivedUrl)
                    .HasColumnName("self_archived_url")
                    .HasMaxLength(400);

                entity.Property(e => e.SelfArchivedVersionCode)
                    .HasColumnName("self_archived_version_code")
                    .HasMaxLength(1);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.DimLocallyReportedPubInfo)
                    .HasForeignKey(d => d.DimPublicationid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_locall615726");
            });

            modelBuilder.Entity<DimMeril>(entity =>
            {
                entity.ToTable("dim_meril");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimName>(entity =>
            {
                entity.ToTable("dim_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimKnownPersonIdConfirmedIdentity).HasColumnName("dim_known_person_id_confirmed_identity");

                entity.Property(e => e.DimKnownPersonidFormerNames).HasColumnName("dim_known_personid_former_names");

                entity.Property(e => e.FirstNames)
                    .HasColumnName("first_names")
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(255);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimKnownPersonIdConfirmedIdentityNavigation)
                    .WithMany(p => p.DimNameDimKnownPersonIdConfirmedIdentityNavigation)
                    .HasForeignKey(d => d.DimKnownPersonIdConfirmedIdentity)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("confirmed identity");

                entity.HasOne(d => d.DimKnownPersonidFormerNamesNavigation)
                    .WithMany(p => p.DimNameDimKnownPersonidFormerNamesNavigation)
                    .HasForeignKey(d => d.DimKnownPersonidFormerNames)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("former names");
            });

            modelBuilder.Entity<DimNewsFeed>(entity =>
            {
                entity.ToTable("dim_news_feed");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.FeedUrl)
                    .HasColumnName("feed_url")
                    .HasMaxLength(4000);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(511);
            });

            modelBuilder.Entity<DimNewsItem>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.DimNewsFeedid })
                    .HasName("PK__dim_news__B87E6703396CD97C");

                entity.ToTable("dim_news_item");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DimNewsFeedid).HasColumnName("dim_news_feedid");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NewsContent).HasColumnName("news_content");

                entity.Property(e => e.NewsHeadline).HasColumnName("news_headline");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(4000);

                entity.HasOne(d => d.DimNewsFeed)
                    .WithMany(p => p.DimNewsItem)
                    .HasForeignKey(d => d.DimNewsFeedid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_news_i691810");
            });

            modelBuilder.Entity<DimOrganisationMedia>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("dim_organisation_media");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.LanguageVariant)
                    .HasColumnName("language_variant")
                    .HasMaxLength(255);

                entity.Property(e => e.MediaUri)
                    .HasColumnName("media_uri")
                    .HasMaxLength(511);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

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
                    .HasColumnName("country_code")
                    .HasMaxLength(2);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DegreeCountBsc).HasColumnName("degree_count_bsc");

                entity.Property(e => e.DegreeCountLic).HasColumnName("degree_count_lic");

                entity.Property(e => e.DegreeCountMsc).HasColumnName("degree_count_msc");

                entity.Property(e => e.DegreeCountPhd).HasColumnName("degree_count_phd");

                entity.Property(e => e.DimOrganizationBroader).HasColumnName("dim_organization_broader");

                entity.Property(e => e.DimSectorid).HasColumnName("dim_sectorid");

                entity.Property(e => e.Established)
                    .HasColumnName("established")
                    .HasColumnType("datetime");

                entity.Property(e => e.LocalOrganizationSector)
                    .HasColumnName("local_organization_sector")
                    .HasMaxLength(255);

                entity.Property(e => e.LocalOrganizationUnitId)
                    .HasColumnName("local_organization_unit_Id")
                    .HasMaxLength(255);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.NameUnd)
                    .HasColumnName("name_und")
                    .HasMaxLength(255);

                entity.Property(e => e.NameVariants)
                    .HasColumnName("name_variants")
                    .HasMaxLength(1023);

                entity.Property(e => e.OrganizationActive).HasColumnName("organization_active");

                entity.Property(e => e.OrganizationBackground)
                    .HasColumnName("organization_background")
                    .HasMaxLength(4000);

                entity.Property(e => e.OrganizationId)
                    .HasColumnName("organization_id")
                    .HasMaxLength(255);

                entity.Property(e => e.OrganizationType)
                    .HasColumnName("organization_type")
                    .HasMaxLength(255);

                entity.Property(e => e.PostalAddress)
                    .HasColumnName("postal_address")
                    .HasMaxLength(511);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.StaffCountAsFte).HasColumnName("staff_count_as_fte");

                entity.Property(e => e.VisitingAddress)
                    .HasColumnName("visiting_address")
                    .HasMaxLength(4000);

                entity.HasOne(d => d.DimOrganizationBroaderNavigation)
                    .WithMany(p => p.InverseDimOrganizationBroaderNavigation)
                    .HasForeignKey(d => d.DimOrganizationBroader)
                    .HasConstraintName("is_part_of");

                entity.HasOne(d => d.DimSector)
                    .WithMany(p => p.DimOrganization)
                    .HasForeignKey(d => d.DimSectorid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_organi330513");
            });

            modelBuilder.Entity<DimPid>(entity =>
            {
                entity.ToTable("dim_pid");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimPublicationChannelId).HasColumnName("dim_publication_channel_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.Property(e => e.DimResearchDatasetId).HasColumnName("dim_research_dataset_id");

                entity.Property(e => e.DimServiceId).HasColumnName("dim_service_id");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.PidContent)
                    .IsRequired()
                    .HasColumnName("pid_content")
                    .HasMaxLength(255);

                entity.Property(e => e.PidType)
                    .IsRequired()
                    .HasColumnName("pid_type")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .HasConstraintName("ROI/RAID");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .HasConstraintName("URN/infrastructure");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .HasConstraintName("Orcid/ISNI");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .HasConstraintName("ISNI/GRID/ROR/Business-ID\\PIC");

                entity.HasOne(d => d.DimPublicationChannel)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimPublicationChannelId)
                    .HasConstraintName("mostly ISSN");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimPublicationId)
                    .HasConstraintName("DOI/ISBN1-2");

                entity.HasOne(d => d.DimResearchDataset)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimResearchDatasetId)
                    .HasConstraintName("URN/DOI");

                entity.HasOne(d => d.DimService)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimServiceId)
                    .HasConstraintName("URN/service");
            });

            modelBuilder.Entity<DimPublication>(entity =>
            {
                entity.ToTable("dim_publication ");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApcFeeEur)
                    .HasColumnName("apc_fee_EUR")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ApcPaymentYear).HasColumnName("apc_payment_year");

                entity.Property(e => e.ArticleNumberText)
                    .HasColumnName("article_number_text")
                    .HasMaxLength(255);

                entity.Property(e => e.ArticleTypeCode).HasColumnName("article_type_code");

                entity.Property(e => e.AuthorsText)
                    .IsRequired()
                    .HasColumnName("authors_text");

                entity.Property(e => e.BusinessCollaboration).HasColumnName("business_collaboration");

                entity.Property(e => e.ConferenceName)
                    .HasColumnName("conference_name")
                    .HasMaxLength(4000);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.Property(e => e.DoiHandle)
                    .HasColumnName("doi_handle")
                    .HasMaxLength(4000);

                entity.Property(e => e.InternationalCollaboration).HasColumnName("international_collaboration");

                entity.Property(e => e.InternationalPublication).HasColumnName("international_publication");

                entity.Property(e => e.IssueNumber)
                    .HasColumnName("issue_number")
                    .HasMaxLength(255);

                entity.Property(e => e.JuuliAddress)
                    .HasColumnName("juuli_address")
                    .HasMaxLength(4000);

                entity.Property(e => e.LanguageCode)
                    .HasColumnName("language_code")
                    .HasMaxLength(255);

                entity.Property(e => e.LicenseCode).HasColumnName("license_code");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NumberOfAuthors).HasColumnName("number_of_authors");

                entity.Property(e => e.OpenAccessCode)
                    .HasColumnName("open_access_code")
                    .HasMaxLength(255);

                entity.Property(e => e.OriginalPublicationId)
                    .HasColumnName("original_publication_id")
                    .HasMaxLength(255);

                entity.Property(e => e.PageNumberText)
                    .HasColumnName("page_number_text")
                    .HasMaxLength(255);

                entity.Property(e => e.ParentPublicationEditors)
                    .HasColumnName("parent_publication_editors")
                    .HasMaxLength(4000);

                entity.Property(e => e.ParentPublicationName)
                    .HasColumnName("parent_publication_name")
                    .HasMaxLength(4000);

                entity.Property(e => e.ParentPublicationTypeCode).HasColumnName("parent_publication_type_code");

                entity.Property(e => e.PeerReviewed).HasColumnName("peer_reviewed");

                entity.Property(e => e.PublicationCountryCode)
                    .HasColumnName("publication_country_code")
                    .HasMaxLength(255);

                entity.Property(e => e.PublicationId)
                    .IsRequired()
                    .HasColumnName("publication_id")
                    .HasMaxLength(255);

                entity.Property(e => e.PublicationName)
                    .IsRequired()
                    .HasColumnName("publication_name")
                    .HasMaxLength(4000);

                entity.Property(e => e.PublicationOrgId)
                    .IsRequired()
                    .HasColumnName("publication_org_id")
                    .HasMaxLength(255);

                entity.Property(e => e.PublicationStatusCode)
                    .HasColumnName("publication_status_code")
                    .HasMaxLength(255);

                entity.Property(e => e.PublicationTypeCode)
                    .IsRequired()
                    .HasColumnName("publication_type_code")
                    .HasMaxLength(255);

                entity.Property(e => e.PublicationTypeCode2).HasColumnName("publication_type_code2");

                entity.Property(e => e.PublicationYear).HasColumnName("publication_year");

                entity.Property(e => e.PublisherLocation)
                    .HasColumnName("publisher_location")
                    .HasMaxLength(255);

                entity.Property(e => e.PublisherName)
                    .HasColumnName("publisher_name")
                    .HasMaxLength(4000);

                entity.Property(e => e.Report).HasColumnName("report");

                entity.Property(e => e.ReportingYear).HasColumnName("reporting_year");

                entity.Property(e => e.SelfArchivedCode)
                    .HasColumnName("self_archived_code")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.TargetAudienceCode).HasColumnName("target_audience_code");

                entity.Property(e => e.ThesisTypeCode).HasColumnName("thesis_type_code");

                entity.Property(e => e.Volume)
                    .HasColumnName("volume")
                    .HasMaxLength(255);

                entity.HasOne(d => d.ArticleTypeCodeNavigation)
                    .WithMany(p => p.DimPublicationArticleTypeCodeNavigation)
                    .HasForeignKey(d => d.ArticleTypeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("article_type_code");

                entity.HasOne(d => d.DimPublicationNavigation)
                    .WithMany(p => p.InverseDimPublicationNavigation)
                    .HasForeignKey(d => d.DimPublicationId)
                    .HasConstraintName("parent_publication");

                entity.HasOne(d => d.ParentPublicationTypeCodeNavigation)
                    .WithMany(p => p.DimPublicationParentPublicationTypeCodeNavigation)
                    .HasForeignKey(d => d.ParentPublicationTypeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("parent_publication_type_code");

                entity.HasOne(d => d.PublicationTypeCode2Navigation)
                    .WithMany(p => p.DimPublicationPublicationTypeCode2Navigation)
                    .HasForeignKey(d => d.PublicationTypeCode2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("publication_type_code2");

                entity.HasOne(d => d.TargetAudienceCodeNavigation)
                    .WithMany(p => p.DimPublicationTargetAudienceCodeNavigation)
                    .HasForeignKey(d => d.TargetAudienceCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("target_audience_code");
            });

            modelBuilder.Entity<DimPublicationChannel>(entity =>
            {
                entity.ToTable("dim_publication_channel");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChannelNameAnylang)
                    .HasColumnName("channel_name_anylang")
                    .HasMaxLength(4000);

                entity.Property(e => e.JufoCode)
                    .HasColumnName("jufo_code")
                    .HasMaxLength(255);

                entity.Property(e => e.PublisherNameText)
                    .HasColumnName("publisher_name_text")
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<DimReferencedata>(entity =>
            {
                entity.ToTable("dim_referencedata");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CodeScheme)
                    .IsRequired()
                    .HasColumnName("code_scheme")
                    .HasMaxLength(511);

                entity.Property(e => e.CodeValue)
                    .IsRequired()
                    .HasColumnName("code_value")
                    .HasMaxLength(511);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .IsRequired()
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimRegisteredDataSource>(entity =>
            {
                entity.ToTable("dim_registered_data_source");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimRegisteredDataSource)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("data source of organisation");
            });

            modelBuilder.Entity<DimResearchDataset>(entity =>
            {
                entity.ToTable("dim_research_dataset");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DatasetCreated)
                    .HasColumnName("dataset_created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DatasetModified)
                    .HasColumnName("dataset_modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.DescriptionEn)
                    .HasColumnName("description_en")
                    .HasMaxLength(4000);

                entity.Property(e => e.DescriptionFi)
                    .HasColumnName("description_fi")
                    .HasMaxLength(4000);

                entity.Property(e => e.DescriptionSv)
                    .HasColumnName("description_sv")
                    .HasMaxLength(4000);

                entity.Property(e => e.DimPublicationChannelId).HasColumnName("dim_publication_channel_id");

                entity.Property(e => e.GeographicCoverage)
                    .HasColumnName("geographic_coverage")
                    .HasMaxLength(4000);

                entity.Property(e => e.InternationalCollaboration).HasColumnName("international_collaboration");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(4000);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(4000);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(4000);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.TemporalCoverageEnd)
                    .HasColumnName("temporal_coverage_end")
                    .HasColumnType("datetime");

                entity.Property(e => e.TemporalCoverageStart)
                    .HasColumnName("temporal_coverage_start")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.DimPublicationChannel)
                    .WithMany(p => p.DimResearchDataset)
                    .HasForeignKey(d => d.DimPublicationChannelId)
                    .HasConstraintName("FKdim_resear690218");
            });

            modelBuilder.Entity<DimSector>(entity =>
            {
                entity.ToTable("dim_sector");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255);

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasColumnName("name_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.SectorId)
                    .IsRequired()
                    .HasColumnName("sector_id")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimService>(entity =>
            {
                entity.ToTable("dim_service");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Acronym)
                    .HasColumnName("acronym")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DescriptionEn)
                    .HasColumnName("description_en")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.DescriptionFi)
                    .HasColumnName("description_fi")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.DescriptionSv)
                    .HasColumnName("description_sv")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.ScientificDescriptionEn)
                    .HasColumnName("scientific_description_en")
                    .HasMaxLength(4000);

                entity.Property(e => e.ScientificDescriptionFi)
                    .HasColumnName("scientific_description_fi")
                    .HasMaxLength(4000);

                entity.Property(e => e.ScientificDescriptionSv)
                    .HasColumnName("scientific_description_sv")
                    .HasMaxLength(4000);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DimServicePoint>(entity =>
            {
                entity.ToTable("dim_service_point");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DescriptionEn)
                    .HasColumnName("description_en")
                    .HasMaxLength(4000);

                entity.Property(e => e.DescriptionFi)
                    .HasColumnName("description_fi")
                    .HasMaxLength(4000);

                entity.Property(e => e.DescriptionSv)
                    .HasColumnName("description_sv")
                    .HasMaxLength(4000);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LinkAccessPolicy)
                    .HasColumnName("link_access_policy")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.LinkAdditionalInfo)
                    .HasColumnName("link_additional_info")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.LinkInternationalInfra)
                    .HasColumnName("link_international_infra")
                    .HasMaxLength(4000);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(4000);

                entity.Property(e => e.NameFi)
                    .HasColumnName("name_fi")
                    .HasMaxLength(4000);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(4000);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.VisitingAddress)
                    .HasColumnName("visiting_address")
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<DimTypeOfFunding>(entity =>
            {
                entity.ToTable("dim_type_of_funding");

                entity.HasIndex(e => e.TypeId)
                    .HasName("UQ__dim_type__2C0005993BF2B7AD")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimTypeOfFundingId).HasColumnName("dim_type_of_funding_id");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(255);

                entity.Property(e => e.NameFi)
                    .IsRequired()
                    .HasColumnName("name_fi")
                    .HasMaxLength(255);

                entity.Property(e => e.NameSv)
                    .HasColumnName("name_sv")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.TypeId)
                    .IsRequired()
                    .HasColumnName("type_id")
                    .HasMaxLength(255);

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
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimUserProfile)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_user_p611467");
            });

            modelBuilder.Entity<DimWebLink>(entity =>
            {
                entity.ToTable("dim_web_link");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimCallProgrammeId).HasColumnName("dim_call_programme_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.LanguageVariant)
                    .HasColumnName("language_variant")
                    .HasMaxLength(255);

                entity.Property(e => e.LinkLabel)
                    .HasColumnName("link_label")
                    .HasMaxLength(255);

                entity.Property(e => e.LinkType)
                    .HasColumnName("link_type")
                    .HasMaxLength(255);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(511);

                entity.HasOne(d => d.DimCallProgramme)
                    .WithMany(p => p.DimWebLink)
                    .HasForeignKey(d => d.DimCallProgrammeId)
                    .HasConstraintName("call homepage");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.DimWebLink)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .HasConstraintName("FKdim_web_li388345");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimWebLink)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .HasConstraintName("web presence");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimWebLink)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .HasConstraintName("language specific homepage");
            });

            modelBuilder.Entity<FactContribution>(entity =>
            {
                entity.HasKey(e => new { e.DimFundingDecisionId, e.DimOrganizationId, e.DimDateId, e.DimNameId, e.DimPublicationId, e.DimGeoId, e.DimInfrastructureId, e.DimNewsFeedId })
                    .HasName("PK__fact_con__73A03EA3FAA778E8");

                entity.ToTable("fact_contribution");

                entity.HasIndex(e => e.DimFundingDecisionId)
                    .HasName("fact_contribution_dim_funding_decision_id");

                entity.HasIndex(e => e.DimNewsFeedId)
                    .HasName("fact_contribution_dim_news_feed_id");

                entity.HasIndex(e => new { e.DimPublicationId, e.DimNameId, e.DimOrganizationId, e.ContributionType, e.DimFundingDecisionId, e.DimInfrastructureId, e.DimGeoId, e.DimDateId, e.SourceId })
                    .HasName("fact_contribution");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimDateId).HasColumnName("dim_date_id");

                entity.Property(e => e.DimNameId).HasColumnName("dim_name_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.Property(e => e.DimGeoId).HasColumnName("dim_geo_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.DimNewsFeedId).HasColumnName("dim_news_feed_id");

                entity.Property(e => e.ActorRole).HasColumnName("actor_role");

                entity.Property(e => e.ContributionType)
                    .IsRequired()
                    .HasColumnName("contribution_type")
                    .HasMaxLength(50);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimResearchDatasetId).HasColumnName("dim_research_dataset_id");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimDate)
                    .WithMany(p => p.FactContribution)
                    .HasForeignKey(d => d.DimDateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr224183");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.FactContribution)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr321314");

                entity.HasOne(d => d.DimGeo)
                    .WithMany(p => p.FactContribution)
                    .HasForeignKey(d => d.DimGeoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr711408");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.FactContribution)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr565219");

                entity.HasOne(d => d.DimName)
                    .WithMany(p => p.FactContribution)
                    .HasForeignKey(d => d.DimNameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr288640");

                entity.HasOne(d => d.DimNewsFeed)
                    .WithMany(p => p.FactContribution)
                    .HasForeignKey(d => d.DimNewsFeedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr632925");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.FactContribution)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr42449");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.FactContribution)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_contr481795");

                entity.HasOne(d => d.DimResearchDataset)
                    .WithMany(p => p.FactContribution)
                    .HasForeignKey(d => d.DimResearchDatasetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("authors, infras, publications, funding decisions");
            });

            modelBuilder.Entity<FactFieldDisplayContent>(entity =>
            {
                entity.HasKey(e => new { e.DimUserProfileId, e.DimFieldDisplaySettingsId, e.DimNameId, e.DimWebLinkId, e.DimFundingDecisionId, e.DimPublicationId, e.DimPidId })
                    .HasName("PK__fact_fie__E763B0951AC5B9D9");

                entity.ToTable("fact_field_display_content");

                entity.Property(e => e.DimUserProfileId).HasColumnName("dim_user_profile_id");

                entity.Property(e => e.DimFieldDisplaySettingsId).HasColumnName("dim_field_display_settings_id");

                entity.Property(e => e.DimNameId).HasColumnName("dim_name_id");

                entity.Property(e => e.DimWebLinkId).HasColumnName("dim_web_link_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.Property(e => e.DimPidId).HasColumnName("dim_pid_id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimFieldDisplaySettings)
                    .WithMany(p => p.FactFieldDisplayContent)
                    .HasForeignKey(d => d.DimFieldDisplaySettingsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("field content settings");

                entity.HasOne(d => d.DimFundingDecision)
                    .WithMany(p => p.FactFieldDisplayContent)
                    .HasForeignKey(d => d.DimFundingDecisionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field644622");

                entity.HasOne(d => d.DimName)
                    .WithMany(p => p.FactFieldDisplayContent)
                    .HasForeignKey(d => d.DimNameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field717013");

                entity.HasOne(d => d.DimPid)
                    .WithMany(p => p.FactFieldDisplayContent)
                    .HasForeignKey(d => d.DimPidId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field667989");

                entity.HasOne(d => d.DimPublication)
                    .WithMany(p => p.FactFieldDisplayContent)
                    .HasForeignKey(d => d.DimPublicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("displayed publications");

                entity.HasOne(d => d.DimUserProfile)
                    .WithMany(p => p.FactFieldDisplayContent)
                    .HasForeignKey(d => d.DimUserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field713499");

                entity.HasOne(d => d.DimWebLink)
                    .WithMany(p => p.FactFieldDisplayContent)
                    .HasForeignKey(d => d.DimWebLinkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field174361");
            });

            modelBuilder.Entity<FactInfraKeywords>(entity =>
            {
                entity.HasKey(e => new { e.DimKeywordId, e.DimServiceId, e.DimServicePointId, e.DimInfrastructureId })
                    .HasName("PK__fact_inf__3C29B68066744A78");

                entity.ToTable("fact_infra_keywords");

                entity.Property(e => e.DimKeywordId).HasColumnName("dim_keyword_id");

                entity.Property(e => e.DimServiceId).HasColumnName("dim_service_id");

                entity.Property(e => e.DimServicePointId).HasColumnName("dim_service_point_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

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

            modelBuilder.Entity<FactJufoClassCodesForPubChannels>(entity =>
            {
                entity.HasKey(e => new { e.DimPublicationChannelId, e.DimReferencedataId, e.Year })
                    .HasName("PK__fact_juf__0E099E4BA249ABCF");

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
                    .HasName("PK__fact_upk__850A8E30F4CAAD32");

                entity.ToTable("fact_upkeep");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimGeoId).HasColumnName("dim_geo_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.DimServiceId).HasColumnName("dim_service_id");

                entity.Property(e => e.DimServicePointId).HasColumnName("dim_service_point_id");

                entity.Property(e => e.DimDateIdStart).HasColumnName("dim_date_id_start");

                entity.Property(e => e.DimDateIdEnd).HasColumnName("dim_date_id_end");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimDateIdEndNavigation)
                    .WithMany(p => p.FactUpkeepDimDateIdEndNavigation)
                    .HasForeignKey(d => d.DimDateIdEnd)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee332759");

                entity.HasOne(d => d.DimDateIdStartNavigation)
                    .WithMany(p => p.FactUpkeepDimDateIdStartNavigation)
                    .HasForeignKey(d => d.DimDateIdStart)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee283512");

                entity.HasOne(d => d.DimGeo)
                    .WithMany(p => p.FactUpkeep)
                    .HasForeignKey(d => d.DimGeoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee520794");

                entity.HasOne(d => d.DimInfrastructure)
                    .WithMany(p => p.FactUpkeep)
                    .HasForeignKey(d => d.DimInfrastructureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee374605");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.FactUpkeep)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee851834");

                entity.HasOne(d => d.DimService)
                    .WithMany(p => p.FactUpkeep)
                    .HasForeignKey(d => d.DimServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee302288");

                entity.HasOne(d => d.DimServicePoint)
                    .WithMany(p => p.FactUpkeep)
                    .HasForeignKey(d => d.DimServicePointId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_upkee415806");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

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

        public virtual DbSet<BrFieldDisplaySettingsDimRegisteredDataSource> BrFieldDisplaySettingsDimRegisteredDataSources { get; set; }
        public virtual DbSet<DimDate> DimDates { get; set; }
        public virtual DbSet<DimFieldDisplaySetting> DimFieldDisplaySettings { get; set; }
        public virtual DbSet<DimKeyword> DimKeywords { get; set; }
        public virtual DbSet<DimKnownPerson> DimKnownPeople { get; set; }
        public virtual DbSet<DimName> DimNames { get; set; }
        public virtual DbSet<DimOrganization> DimOrganizations { get; set; }
        public virtual DbSet<DimPid> DimPids { get; set; }
        public virtual DbSet<DimRegisteredDataSource> DimRegisteredDataSources { get; set; }
        public virtual DbSet<DimUserProfile> DimUserProfiles { get; set; }
        public virtual DbSet<DimWebLink> DimWebLinks { get; set; }
        public virtual DbSet<FactFieldValue> FactFieldValues { get; set; }

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

            modelBuilder.Entity<BrFieldDisplaySettingsDimRegisteredDataSource>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldDisplaySettingsId, e.DimRegisteredDataSourceId })
                    .HasName("PK__br_field__6148A77222DE2D0B");

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

                entity.Property(e => e.Year).HasColumnName("year");
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

                entity.HasOne(d => d.DimUserProfile)
                    .WithMany(p => p.DimFieldDisplaySettings)
                    .HasForeignKey(d => d.DimUserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKdim_field_653425");
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
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.ResearchDescription).HasColumnName("research_description");
            });

            modelBuilder.Entity<DimName>(entity =>
            {
                entity.ToTable("dim_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimKnownPersonIdConfirmedIdentity).HasColumnName("dim_known_person_id_confirmed_identity");

                entity.Property(e => e.DimKnownPersonidFormerNames).HasColumnName("dim_known_personid_former_names");

                entity.Property(e => e.FirstNames)
                    .HasMaxLength(255)
                    .HasColumnName("first_names");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name")
                    .HasComment("Only to be used, when first name + last name not known (i.e. Metax).");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .HasColumnName("last_name");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

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

                entity.Property(e => e.StaffCountAsFte).HasColumnName("staff_count_as_fte");

                entity.Property(e => e.VisitingAddress)
                    .HasMaxLength(4000)
                    .HasColumnName("visiting_address");
            });

            modelBuilder.Entity<DimPid>(entity =>
            {
                entity.ToTable("dim_pid");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimInfrastructureId).HasColumnName("dim_infrastructure_id");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimPublicationChannelId).HasColumnName("dim_publication_channel_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

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

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimPids)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .HasConstraintName("Orcid/ISNI");
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
                    .HasMaxLength(255)
                    .HasColumnName("name");
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

                entity.Property(e => e.Url)
                    .HasMaxLength(511)
                    .HasColumnName("url");

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimWebLinks)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .HasConstraintName("web presence");
            });

            modelBuilder.Entity<FactFieldValue>(entity =>
            {
                entity.HasKey(e => new { e.DimUserProfileId, e.DimFieldDisplaySettingsId, e.DimNameId, e.DimWebLinkId, e.DimFundingDecisionId, e.DimPublicationId, e.DimPidId, e.DimPidIdOrcidPutCode })
                    .HasName("PK__fact_fie__0B4803F61D0D935A");

                entity.ToTable("fact_field_values");

                entity.Property(e => e.DimUserProfileId).HasColumnName("dim_user_profile_id");

                entity.Property(e => e.DimFieldDisplaySettingsId).HasColumnName("dim_field_display_settings_id");

                entity.Property(e => e.DimNameId).HasColumnName("dim_name_id");

                entity.Property(e => e.DimWebLinkId).HasColumnName("dim_web_link_id");

                entity.Property(e => e.DimFundingDecisionId).HasColumnName("dim_funding_decision_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.Property(e => e.DimPidId).HasColumnName("dim_pid_id");

                entity.Property(e => e.DimPidIdOrcidPutCode).HasColumnName("dim_pid_id_orcid_put_code");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.Modified)
                    .HasColumnType("datetime")
                    .HasColumnName("modified");

                entity.Property(e => e.Show).HasColumnName("show");

                entity.HasOne(d => d.DimFieldDisplaySettings)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimFieldDisplaySettingsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("field content settings");

                entity.HasOne(d => d.DimName)
                    .WithMany(p => p.FactFieldValues)
                    .HasForeignKey(d => d.DimNameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field604813");

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

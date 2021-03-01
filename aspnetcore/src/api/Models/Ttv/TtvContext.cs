using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<BrFieldDisplaySettingsDimRegisteredDataSource> BrFieldDisplaySettingsDimRegisteredDataSource { get; set; }
        public virtual DbSet<DimDate> DimDate { get; set; }
        public virtual DbSet<DimFieldDisplaySettings> DimFieldDisplaySettings { get; set; }
        public virtual DbSet<DimKeyword> DimKeyword { get; set; }
        public virtual DbSet<DimKnownPerson> DimKnownPerson { get; set; }
        public virtual DbSet<DimName> DimName { get; set; }
        public virtual DbSet<DimOrganization> DimOrganization { get; set; }
        public virtual DbSet<DimPid> DimPid { get; set; }
        public virtual DbSet<DimRegisteredDataSource> DimRegisteredDataSource { get; set; }
        public virtual DbSet<DimUserProfile> DimUserProfile { get; set; }
        public virtual DbSet<DimWebLink> DimWebLink { get; set; }
        public virtual DbSet<FactFieldValues> FactFieldValues { get; set; }

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
            modelBuilder.Entity<BrFieldDisplaySettingsDimRegisteredDataSource>(entity =>
            {
                entity.HasKey(e => new { e.DimFieldDisplaySettingsId, e.DimRegisteredDataSourceId })
                    .HasName("PK__br_field__6148A772CD3C97C2");

                entity.ToTable("br_field_display_settings_dim_registered_data_source");

                entity.Property(e => e.DimFieldDisplaySettingsId).HasColumnName("dim_field_display_settings_id");

                entity.Property(e => e.DimRegisteredDataSourceId).HasColumnName("dim_registered_data_source_id");

                entity.HasOne(d => d.DimFieldDisplaySettings)
                    .WithMany(p => p.BrFieldDisplaySettingsDimRegisteredDataSource)
                    .HasForeignKey(d => d.DimFieldDisplaySettingsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_d783303");

                entity.HasOne(d => d.DimRegisteredDataSource)
                    .WithMany(p => p.BrFieldDisplaySettingsDimRegisteredDataSource)
                    .HasForeignKey(d => d.DimRegisteredDataSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbr_field_d115264");
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

                entity.Property(e => e.Year).HasColumnName("year");
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

                entity.Property(e => e.FullName)
                    .HasColumnName("full_name")
                    .HasMaxLength(255)
                    .HasComment("Only to be used, when first name + last name not known (i.e. Metax).");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(255);

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

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

                entity.Property(e => e.StaffCountAsFte).HasColumnName("staff_count_as_fte");

                entity.Property(e => e.VisitingAddress)
                    .HasColumnName("visiting_address")
                    .HasMaxLength(4000);
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

                entity.Property(e => e.DimResearchDataCatalogId).HasColumnName("dim_research_data_catalog_id");

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

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .HasConstraintName("Orcid/ISNI");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .HasConstraintName("ISNI/GRID/ROR/Business-ID\\PIC");
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

                entity.Property(e => e.DimResearchCommunityId).HasColumnName("dim_research_community_id");

                entity.Property(e => e.DimResearchDataCatalogId).HasColumnName("dim_research_data_catalog_id");

                entity.Property(e => e.DimResearchDatasetId).HasColumnName("dim_research_dataset_id");

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

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(511);

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimWebLink)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .HasConstraintName("web presence");

                entity.HasOne(d => d.DimOrganization)
                    .WithMany(p => p.DimWebLink)
                    .HasForeignKey(d => d.DimOrganizationId)
                    .HasConstraintName("language specific homepage");
            });

            modelBuilder.Entity<FactFieldValues>(entity =>
            {
                entity.HasKey(e => new { e.DimUserProfileId, e.DimFieldDisplaySettingsId, e.DimNameId, e.DimWebLinkId, e.DimFundingDecisionId, e.DimPublicationId, e.DimPidId, e.DimPidIdOrcidPutCode })
                    .HasName("PK__fact_fie__0B4803F684D29165");

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
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

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
                    .WithMany(p => p.FactFieldValuesDimPid)
                    .HasForeignKey(d => d.DimPidId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfact_field989816");

                entity.HasOne(d => d.DimPidIdOrcidPutCodeNavigation)
                    .WithMany(p => p.FactFieldValuesDimPidIdOrcidPutCodeNavigation)
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

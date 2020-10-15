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

        public virtual DbSet<DimKnownPerson> DimKnownPerson { get; set; }
        public virtual DbSet<DimPid> DimPid { get; set; }

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

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DimPid>(entity =>
            {
                entity.HasKey(e => e.PidContent)
                    .HasName("PK__dim_pid__8519075AAAD83AB5");

                entity.ToTable("dim_pid");

                entity.Property(e => e.PidContent)
                    .HasColumnName("pid_content")
                    .HasMaxLength(255);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DimInfrastructureid).HasColumnName("dim_infrastructureid");

                entity.Property(e => e.DimKnownPersonId).HasColumnName("dim_known_person_id");

                entity.Property(e => e.DimOrganizationId).HasColumnName("dim_organization_id");

                entity.Property(e => e.DimPublicationId).HasColumnName("dim_publication_id");

                entity.Property(e => e.DimServiceId).HasColumnName("dim_service_id");

                entity.Property(e => e.Modified)
                    .HasColumnName("modified")
                    .HasColumnType("datetime");

                entity.Property(e => e.PidType)
                    .HasColumnName("pid_type")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceDescription)
                    .HasColumnName("source_description")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id")
                    .HasMaxLength(255);

                entity.HasOne(d => d.DimKnownPerson)
                    .WithMany(p => p.DimPid)
                    .HasForeignKey(d => d.DimKnownPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Orcid/ISNI");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

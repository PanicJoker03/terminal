using Microsoft.EntityFrameworkCore;
using TerminalApi.Domain.Entities;

namespace TerminalApi.Data;

public class TerminalDbContext(DbContextOptions<TerminalDbContext> options) : DbContext(options)
{
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<Interview> Interviews => Set<Interview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobApplication>(entity =>
        {
            // Configuración de la entidad JobApplication
            // Configuración de la tabla y la clave primaria
            entity.ToTable("JobApplications");
            entity.HasKey(application => application.Id);

            entity.Property(application => application.Id)
                .ValueGeneratedOnAdd();
            entity.Property(application => application.Timestamp)
                .IsRequired();
            entity.Property(application => application.Company)
                .IsRequired();
            entity.Property(application => application.Role)
                .IsRequired();
            entity.Property(application => application.Status)
                .IsRequired();
            entity.Property(application => application.Payment)
                .IsRequired();
            entity.Property(application => application.Origin)
                .IsRequired();
            entity.Property(application => application.Modality)
                .IsRequired()
                .HasDefaultValue("on-site");
            entity.Property(application => application.Priority)
                .IsRequired()
                .HasDefaultValue("C");

            entity.HasIndex(application => application.Timestamp);
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.ToTable("Interviews");
            entity.HasKey(interview => interview.Id);

            entity.Property(interview => interview.Id)
                .ValueGeneratedOnAdd();
            entity.Property(interview => interview.Attendants)
                .IsRequired();
            entity.Property(interview => interview.Organizer)
                .IsRequired();
            entity.Property(interview => interview.Month)
                .IsRequired();
            entity.Property(interview => interview.Day)
                .IsRequired();
            entity.Property(interview => interview.Hour)
                .IsRequired();
            entity.Property(interview => interview.Status)
                .IsRequired();
            entity.Property(interview => interview.CompanyClient)
                .IsRequired();
            entity.Property(interview => interview.Role)
                .IsRequired();

            entity.HasIndex(interview => new { interview.Month, interview.Day });
        });
    }
}

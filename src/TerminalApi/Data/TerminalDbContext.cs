using Microsoft.EntityFrameworkCore;
using TerminalApi.Domain.Entities;

namespace TerminalApi.Data;

public class TerminalDbContext(DbContextOptions<TerminalDbContext> options) : DbContext(options)
{
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

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

            entity.HasIndex(application => application.Timestamp);
        });
    }
}
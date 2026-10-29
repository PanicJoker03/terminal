using Microsoft.EntityFrameworkCore;

namespace TerminalApi.Services;

public class TerminalDbContext(DbContextOptions<TerminalDbContext> options) : DbContext(options)
{
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobApplication>(entity =>
        {
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
using Microsoft.EntityFrameworkCore;
using TerminalApi.Domain.Entities;

namespace TerminalApi.Data;

public class TerminalDbContext(DbContextOptions<TerminalDbContext> options) : DbContext(options)
{
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Client> Clients => Set<Client>();

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
            entity.Property(application => application.Contact);
            entity.Property(application => application.RhName);
            entity.Property(application => application.Channel);

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

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasKey(course => course.Id);
            entity.Property(course => course.Id).ValueGeneratedOnAdd();
            entity.Property(course => course.Name).IsRequired();
            entity.Property(course => course.Progress).IsRequired();
            entity.Property(course => course.Status).IsRequired();
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(book => book.Id);
            entity.Property(book => book.Id).ValueGeneratedOnAdd();
            entity.Property(book => book.Name).IsRequired();
            entity.Property(book => book.Status).IsRequired();
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(client => client.Id);
            entity.Property(client => client.Id).ValueGeneratedOnAdd();
            entity.Property(client => client.Name).IsRequired();
            entity.Property(client => client.Projects).IsRequired();
            entity.Property(client => client.Status).IsRequired();
        });
    }
}

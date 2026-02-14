using Microsoft.EntityFrameworkCore;
using HospitalTriageAI.Models;

namespace HospitalTriageAI.Data;

/// <summary>
/// SQLite database context for hospital triage system
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<TriageAssessment> Assessments => Set<TriageAssessment>();
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            entity.HasMany(p => p.Assessments)
                  .WithOne(a => a.Patient)
                  .HasForeignKey(a => a.PatientId);
        });
        
        // TriageAssessment configuration
        modelBuilder.Entity<TriageAssessment>(entity =>
        {
            entity.HasKey(a => a.Id);
        });
    }
    
    /// <summary>
    /// Gets the SQLite database path for MAUI
    /// </summary>
    public static string GetDatabasePath()
    {
        return Path.Combine(FileSystem.AppDataDirectory, "hospital_triage.db");
    }
}

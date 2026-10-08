using HealthcarePortalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcarePortalApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<AccessRequest> AccessRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. User Constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(p => p.Email)
            .IsRequired();
            
        modelBuilder.Entity<User>()
            .Property(p => p.PasswordHash)
            .IsRequired();

        // 2. Patient -> Doctor (User) Relationship
        modelBuilder.Entity<Patient>()
            .HasOne(p => p.Doctor)
            .WithMany(u => u.Patients)
            .HasForeignKey(p => p.DoctorId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a doctor if they have patients

        // 3. AccessRequest -> Users (Owning & Requesting Doctors)
        modelBuilder.Entity<AccessRequest>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.RequestingDoctorId)
            .OnDelete(DeleteBehavior.Restrict); // Fixes the multiple cascade paths issue

        modelBuilder.Entity<AccessRequest>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.OwningDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // 4. AccessRequest -> Patient
        modelBuilder.Entity<AccessRequest>()
            .HasOne<Patient>()
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
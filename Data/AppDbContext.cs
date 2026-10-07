using HealthcarePortalApi.Models;
using Microsoft.EntityFrameworkCore;


namespace HealthcarePortal.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensure no two users can register with the same email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(p=>p.Email)
            .IsRequired();
        modelBuilder.Entity<User>()
            .Property(p=>p.PasswordHash)
            .HasMaxLength(20)
            .IsRequired();
    }
}
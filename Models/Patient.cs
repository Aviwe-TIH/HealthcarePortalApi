namespace HealthcarePortalApi.Models;

public class Patient
{
    public Guid Id { get; set; }
    
    // Foreign Key linking this patient to their primary Doctor
    public Guid DoctorId { get; set; }
    public User? Doctor { get; set; }

    public string NationalId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    
    // This is the private data we will protect later
    public string MedicalHistory { get; set; } = string.Empty; 
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
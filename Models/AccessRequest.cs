namespace HealthcarePortalApi.Models;

public class AccessRequest
{
    public Guid Id { get; set; }
    public Guid RequestingDoctorId { get; set; }
    public Guid PatientId { get; set; }
    public Guid OwningDoctorId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
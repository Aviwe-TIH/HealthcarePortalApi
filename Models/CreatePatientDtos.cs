namespace HealthcarePortalApi.Models;

public class CreatePatientDto
{
    public string NationalId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string MedicalHistory { get; set; } = string.Empty;
}
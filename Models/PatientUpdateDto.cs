namespace HealthcarePortalApi.Models;

public class UpdatePatientDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string MedicalHistory { get; set; } = string.Empty;
}
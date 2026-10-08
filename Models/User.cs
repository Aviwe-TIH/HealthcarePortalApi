namespace HealthcarePortalApi.Models;

public enum Role{doctor}
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.doctor;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
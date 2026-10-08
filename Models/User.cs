using System;
using System.ComponentModel.DataAnnotations;

namespace HealthcarePortalApi.Models;


public enum Role
{
    doctor,patient
}
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [MinLength(8)]
    public string PasswordHash { get; set; } = string.Empty;
    
    public Role Role { get; set; } = Role.doctor; // Defaulting to Doctor for practice
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
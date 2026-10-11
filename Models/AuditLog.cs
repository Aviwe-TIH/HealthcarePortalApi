namespace HealthcarePortalApi.Models;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "Insert", "Update", "Delete"
    public string? OldValues { get; set; } 
    public string? NewValues { get; set; } 
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; } // Who made the change
}
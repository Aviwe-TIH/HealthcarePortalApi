using System.Security.Claims;
using HealthcarePortalApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HealthcarePortalApi.Services;

public class AuditInterceptor(IHttpContextAccessor httpContext): SaveChangesInterceptor{
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData is null)
        {
            return base.SavingChangesAsync(eventData!, result, cancellationToken);
        }

        var auditLogs = new List<AuditLog>();
        var context = eventData.Context;
        if (context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entries = context.ChangeTracker.Entries<IAuditable>().
            Where(e => e.State == EntityState.Added ||
                       e.State == EntityState.Deleted ||
                       e.State == EntityState.Modified
            ).ToList();

        foreach (var entry in entries)
        {
            var audit = new AuditLog();

            audit.TableName = entry.Entity.GetType().Name;
            audit.Action = entry.State.ToString();
            audit.Timestamp = DateTime.UtcNow;
            audit.UserId = httpContext.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContext.HttpContext?.User?.Identity?.Name
                ?? "anonymous";

            var changedProperties = new List<string>();

            foreach (var p in entry.Properties)
            {
                if (entry.State == EntityState.Added)
                    changedProperties.Add($"{p.Metadata.Name}: Added -> {p.CurrentValue}");
                else if (entry.State == EntityState.Deleted)
                    changedProperties.Add($"{p.Metadata.Name}: {p.OriginalValue} -> Deleted");
                else if (entry.State == EntityState.Modified && p.IsModified)
                    changedProperties.Add($"{p.Metadata.Name}: {p.OriginalValue} -> {p.CurrentValue}");
            }

            audit.OldValues = changedProperties.Count > 0
                ? string.Join("; ", changedProperties)
                : "No changes detected";

            auditLogs.Add(audit);
        }
        
        context.Set<AuditLog>().AddRange(auditLogs);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}


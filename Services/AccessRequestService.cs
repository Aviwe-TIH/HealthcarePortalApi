using Microsoft.EntityFrameworkCore;
using HealthcarePortalApi.Data;
using HealthcarePortalApi.Models;

namespace HealthcarePortalApi.Services;

public interface IAccessRequestService
{
    Task<(bool Success, string Message)> RequestAccessAsync(Guid patientId, Guid requestingDoctorId);
    Task<List<AccessRequest>> GetPendingRequestsAsync(Guid doctorId);
    Task<(bool Success, string Message)> ApproveRequestAsync(Guid requestId, Guid doctorId);
}

public class AccessRequestService : IAccessRequestService
{
    private readonly ApplicationDbContext _context;

    public AccessRequestService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> RequestAccessAsync(Guid patientId, Guid requestingDoctorId)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        
        if (patient == null) 
            return (false, "Patient not found.");
        
        if (patient.DoctorId == requestingDoctorId) 
            return (false, "You already own this patient's record.");

        var existingRequest = await _context.AccessRequests
            .FirstOrDefaultAsync(r => r.PatientId == patientId && r.RequestingDoctorId == requestingDoctorId);

        if (existingRequest != null) 
            return (false, $"Request already exists with status: {existingRequest.Status}");

        var request = new AccessRequest
        {
            RequestingDoctorId = requestingDoctorId,
            PatientId = patient.Id,
            OwningDoctorId = patient.DoctorId
        };

        _context.AccessRequests.Add(request);
        await _context.SaveChangesAsync();

        return (true, "Access request sent successfully.");
    }

    public async Task<List<AccessRequest>> GetPendingRequestsAsync(Guid doctorId)
    {
        return await _context.AccessRequests
            .Where(r => r.OwningDoctorId == doctorId && r.Status == "Pending")
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> ApproveRequestAsync(Guid requestId, Guid doctorId)
    {
        var request = await _context.AccessRequests.FindAsync(requestId);

        if (request == null) 
            return (false, "Request not found.");
        
        if (request.OwningDoctorId != doctorId) 
            return (false, "Forbidden");

        request.Status = "Approved";
        await _context.SaveChangesAsync();

        return (true, "Request approved.");
    }
}
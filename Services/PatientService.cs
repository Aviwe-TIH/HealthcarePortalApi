using Microsoft.EntityFrameworkCore;
using HealthcarePortalApi.Data;
using HealthcarePortalApi.Models;

namespace HealthcarePortalApi.Services;

public interface IPatientService
{
    Task<Patient> CreatePatientAsync(CreatePatientDto dto, Guid doctorId);
    Task<List<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId);
    Task<(Patient? Patient, bool IsForbidden)> GetPatientByIdAsync(Guid id, Guid doctorId);
    Task<object?> SearchPatientByNationalIdAsync(string nationalId, Guid doctorId);
}

public class PatientService(ApplicationDbContext _context) : IPatientService
{

    public async Task<Patient> CreatePatientAsync(CreatePatientDto dto, Guid doctorId)
    {
        var patient = new Patient
        {
            DoctorId = doctorId,
            NationalId = dto.NationalId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            MedicalHistory = dto.MedicalHistory
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return patient;
    }

    public async Task<List<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId)
    {
        return await _context.Patients
            .Where(p => p.DoctorId == doctorId)
            .ToListAsync();
    }

    public async Task<(Patient? Patient, bool IsForbidden)> GetPatientByIdAsync(Guid id, Guid doctorId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null) 
            return (null, false); // Not found

        if (patient.DoctorId != doctorId) 
            return (patient, true); // Found, but not authorized

        return (patient, false); // Found and authorized
    }

    public async Task<object?> SearchPatientByNationalIdAsync(string nationalId, Guid doctorId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.NationalId == nationalId);

        if (patient == null) return null;

        // Full access if they own the record
        if (patient.DoctorId == doctorId) return patient;

        // Check if they have an approved request
        var hasApprovedAccess = await _context.AccessRequests
            .AnyAsync(r => r.PatientId == patient.Id 
                        && r.RequestingDoctorId == doctorId 
                        && r.Status == "Approved");

        if (hasApprovedAccess) return patient;

        return new 
        {
            patient.Id,
            patient.DoctorId,
            patient.NationalId,
            patient.FirstName,
            patient.LastName,
            patient.DateOfBirth,
            MedicalHistory = "HIDDEN - Request permission from the assigned doctor to view.",
            patient.CreatedAt
        };
    }
}
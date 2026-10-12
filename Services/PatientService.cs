using Microsoft.EntityFrameworkCore;
using HealthcarePortalApi.Data;
using HealthcarePortalApi.Models;

namespace HealthcarePortalApi.Services;

public interface IPatientService
{
    Task<Patient> CreatePatientAsync(CreatePatientDto dto, Guid doctorId);
    Task<PagedResponse<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId, int pageNumber, int pageSize);
    Task<(Patient? Patient, bool IsForbidden)> GetPatientByIdAsync(Guid id, Guid doctorId);
    Task<object?> SearchPatientByNationalIdAsync(string nationalId, Guid doctorId);

    Task<bool> UpdatePatientAsync(Guid patientId, Guid doctorId, UpdatePatientDto dto);
    Task<bool> DeletePatientAsync(Guid id, Guid doctorId);
    Task<List<Patient>> CreateMultiplePatientsAsync(IEnumerable<CreatePatientDto> dtos, Guid doctorId);
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

    public async Task<PagedResponse<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId, int pageNumber, int pageSize)
{
    // 1. Build the base query (does not hit the DB yet)
    var query = _context.Patients.Where(p => p.DoctorId == doctorId);

    // 2. Count the total records for this specific doctor
    var totalRecords = await query.CountAsync();

    // 3. Fetch only the specific page of data
    var patients = await query
        .OrderByDescending(p => p.CreatedAt) // Newest patients first
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // 4. Calculate total pages and return
    var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

    return new PagedResponse<Patient>
    {
        Data = patients,
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalRecords = totalRecords,
        TotalPages = totalPages
    };
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

    public async Task<bool> UpdatePatientAsync(Guid patientId, Guid doctorId, UpdatePatientDto dto)
{
    // 1. Get the patient (EF Core starts tracking it)
    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == patientId);
    
    if (patient is null) return false;

    // 2. Strict Security: Only the assigned doctor can edit this record
    if (patient.DoctorId != doctorId) throw new UnauthorizedAccessException("You do not own this patient record.");

    // 3. Update the existing tracked object using the DTO
    patient.FirstName = dto.FirstName;
    patient.LastName = dto.LastName;
    patient.MedicalHistory = dto.MedicalHistory;
    patient.DateOfBirth = dto.DateOfBirth;

    // 4. Save changes (No need to call _context.Update, EF Core already knows it changed)
    await _context.SaveChangesAsync();

    return true;
}

    public async Task<bool> DeletePatientAsync(Guid id, Guid doctorId)
    {
        var patientToDelete = await _context.Patients.FirstOrDefaultAsync(p=> p.Id == id);
        if(patientToDelete is null) return false;

        if (patientToDelete.DoctorId != doctorId) throw new UnauthorizedAccessException("You do not own this patient record.");


        _context.Remove(patientToDelete);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Patient>> CreateMultiplePatientsAsync(IEnumerable<CreatePatientDto> dtos, Guid doctorId)
    {
        var patients = dtos.Select(dto => new Patient
        {
            DoctorId = doctorId,
            NationalId = dto.NationalId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            MedicalHistory = dto.MedicalHistory
        }).ToList();

        // AddRange tracks the entire list at once
        _context.Patients.AddRange(patients);
        await _context.SaveChangesAsync();

        return patients;
    }
}
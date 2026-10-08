using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthcarePortalApi.Models;
using HealthcarePortalApi.Services;

namespace HealthcarePortalApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    private Guid GetCurrentDoctorId()
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                       ?? User.FindFirst("sub")?.Value;
        
        return Guid.Parse(doctorId!);
    }

    [HttpPost]
    public async Task<IActionResult> AddPatient([FromBody] CreatePatientDto dto)
    {
        var doctorId = GetCurrentDoctorId();
        var patient = await _patientService.CreatePatientAsync(dto, doctorId);

        return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyPatients()
    {
        var doctorId = GetCurrentDoctorId();
        var patients = await _patientService.GetPatientsByDoctorIdAsync(doctorId);

        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientById(Guid id)
    {
        var doctorId = GetCurrentDoctorId();
        var (patient, isForbidden) = await _patientService.GetPatientByIdAsync(id, doctorId);

        if (patient == null) return NotFound();
        if (isForbidden) return Forbid();

        return Ok(patient);
    }

    [HttpGet("search/{nationalId}")]
    public async Task<IActionResult> SearchByNationalId(string nationalId)
    {
        var doctorId = GetCurrentDoctorId();
        var result = await _patientService.SearchPatientByNationalIdAsync(nationalId, doctorId);

        if (result == null) 
            return NotFound(new { Message = "Patient not found." });

        return Ok(result);
    }
}
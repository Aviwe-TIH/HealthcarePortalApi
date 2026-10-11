using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthcarePortalApi.Services;

namespace HealthcarePortalApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccessRequestsController : ControllerBase
{
    private readonly IAccessRequestService _accessRequestService;

    public AccessRequestsController(IAccessRequestService accessRequestService)
    {
        _accessRequestService = accessRequestService;
    }

    private Guid GetCurrentDoctorId()
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                       ?? User.FindFirst("sub")?.Value;
        
        return Guid.Parse(doctorId!);
    }

    [HttpPost("{patientId}")]
    public async Task<IActionResult> RequestAccess(Guid patientId)
    {
        var requestingDoctorId = GetCurrentDoctorId();
        var (success, message) = await _accessRequestService.RequestAccessAsync(patientId, requestingDoctorId);

        if (!success)
        {
            if (message == "Patient not found.") return NotFound(new { Message = message });
            return BadRequest(new { Message = message });
        }

        return Ok(new { Message = message });
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var doctorId = GetCurrentDoctorId();
        var requests = await _accessRequestService.GetPendingRequestsAsync(doctorId);
        
        return Ok(requests);
    }

    [HttpPut("{requestId}/approve")]
    public async Task<IActionResult> ApproveRequest(Guid requestId)
    {
        var doctorId = GetCurrentDoctorId();
        var (success, message) = await _accessRequestService.ApproveRequestAsync(requestId, doctorId);

        if (!success)
        {
            if (message == "Request not found.") return NotFound(new { Message = message });
            if (message == "Forbidden") return Forbid();
            return BadRequest(new { Message = message });
        }

        return Ok(new { Message = message });
    }
}
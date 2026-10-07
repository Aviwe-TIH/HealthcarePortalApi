using Microsoft.AspNetCore.Mvc;

namespace HealthcarePortal.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class PatientController: ControllerBase
{
    
    [HttpGet]
    public string Test()
    {
        return "The enpoint TEST is doing well.";
    }

}
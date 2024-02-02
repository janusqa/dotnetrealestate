using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


namespace RealEstate.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/errorhandler")]
    [ApiVersionNeutral]
    [ApiExplorerSettings(IgnoreApi = true)] // ignore this controller in swagger
    public class ErrorHandlerController : ControllerBase
    {


        [HttpGet("processerror")]
        public async Task<IActionResult> ProcessError([FromServices] IHostEnvironment he)
        {
            await Task.CompletedTask;

            if (he.IsDevelopment())
            {
                // custom logic to get more detailed errors we 
                // would not want to show in production to end users
                var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
                return Problem(
                    detail: feature?.Error.InnerException?.StackTrace ?? feature?.Error.StackTrace,
                    title: feature?.Error.Message,
                    instance: he.EnvironmentName
                );
            }
            else
            {
                return Problem();
            }
        }

    }
}
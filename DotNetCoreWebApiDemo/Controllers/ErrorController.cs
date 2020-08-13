using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApiDemo.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error() => Problem();

        [Route("/error-local")]
        public IActionResult ErrorLocal([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if(webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException("Local error handler should not be invoked in Non-Development environment.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return Problem(detail: context.Error.StackTrace, title: context.Error.Message);
        }
    }
}

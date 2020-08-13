using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApiDemo.Controllers
{
    [Route("/static-secure")]
    [ApiController]
    [Authorize]
    public class FilesController : Controller
    {
        private readonly IWebHostEnvironment environment;

        public FilesController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        /// <summary>
        /// Serves the secure files using file name. It would download the file on browser.
        /// Eg: https://localhost:5001/static-secure/Index.html
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet("{filename}")]
        public IActionResult GetFile(string filename)
        {
            var filePath = Path.Combine(environment.ContentRootPath, "wwwroot-secure", filename);
            return  PhysicalFile(filePath, "html/text");
        }

    }
}

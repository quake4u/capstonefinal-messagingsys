using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CourseraCapstone.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested can not be found.";
                    logger.LogWarning($"404 Error occured. the path is {statusCodeResult.OriginalPath} " +
                        $",and the query string is {statusCodeResult.OriginalQueryString}");
                    break;
            }
            return View("NotFound");
        }

        [Route("Error")]
        public IActionResult Error()
        {
            var errorResult = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            logger.LogError($"The path = {errorResult.Path} threw an Exception {errorResult.Error}");

            return View("Error");
        }
    }
}

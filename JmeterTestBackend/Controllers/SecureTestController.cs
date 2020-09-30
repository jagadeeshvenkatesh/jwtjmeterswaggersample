using JmeterTestBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JmeterTestBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SecureTestController : Controller
    {
        private ILogger<SecureTestController> _logger;
        private TestService _testService;

        public SecureTestController(ILogger<SecureTestController> logger, TestService testService)
        {
            _logger = logger;
            _testService = testService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_testService.GetTestResult(HttpContext.User));
        }
    }
}

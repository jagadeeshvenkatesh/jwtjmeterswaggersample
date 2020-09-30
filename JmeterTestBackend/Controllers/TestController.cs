using JmeterTestBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JmeterTestBackend.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private ILogger<TestController> _logger;
        private TestService _testService;

        public TestController(ILogger<TestController> logger, TestService testService)
        {
            _testService = testService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_testService.GetTestResult(HttpContext.User));
        }
    }
}

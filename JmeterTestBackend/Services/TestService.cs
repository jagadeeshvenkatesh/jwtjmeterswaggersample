using JmeterTestBackend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JmeterTestBackend.Services
{
    public class TestService
    {
        private ILogger<TestService> _logger;

        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }

        public TestModel GetTestResult(ClaimsPrincipal currentPrincipal)
        {
            _logger.LogInformation("Client calling GetTestResults()...");
            
            var response = new TestModel
            {
                ResponseId = Guid.NewGuid(),
                ResponseTime = DateTime.UtcNow
            };

            if((currentPrincipal == null) || !(currentPrincipal.Identity.IsAuthenticated))
            {
                _logger.LogWarning("--> GetTestResult called without identity, request not authenticated.");
            } 
            else
            {
                _logger.LogInformation("--> Called with an authenticated user.");
                
                response.CallingUser = currentPrincipal.Identity.Name;
                response.CallingUserClaims = new List<KeyValuePair<string, string>>();
                foreach(var c in currentPrincipal.Claims)
                {
                    response.CallingUserClaims.Add(
                        new KeyValuePair<string, string>(c.Type, c.Value)
                    );
                }
            }

            return response;
        }
    }
}

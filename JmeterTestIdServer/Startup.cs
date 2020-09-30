using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace JmeterTestIdServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Note: we don't need any end users on this STS for now
            services.AddIdentityServer()
                    .AddInMemoryClients(GetTestClients())
                    .AddInMemoryIdentityResources(GetIdentityResources())
                    .AddInMemoryApiResources(GetApiResources())
                    .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }

        private IEnumerable<ApiResource> GetApiResources()
        {
            yield return new ApiResource()
            {
                Name = "jmeter-test-api-audience",
                DisplayName = "Test API for demonstrating, how-to use tokens with JMeter tests",
                
                Scopes = new List<Scope> ()
                {
                    new Scope
                    {
                        Name = "api://marioszp-jmeter-test-application-backend/testbackend",
                        UserClaims = new string [] { "automated-test" }
                    }
                }
            };
        }

        private IEnumerable<IdentityResource> GetIdentityResources()
        {
            yield return new IdentityResources.OpenId();
            yield return new IdentityResources.Profile();
        }

        private IEnumerable<Client> GetTestClients()
        {
            yield return new Client()
            {
                ClientId = "jmeter-test-driver",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = new List<string>()
                {
                    "api://marioszp-jmeter-test-application-backend/testbackend"
                },
                ClientSecrets = new List<Secret>()
                {
                    // Simplified for local testing without dependencies
                    // Don't do this in production environments
                    new Secret("jmeter-test-client-secret".Sha256())
                },
                ClientClaimsPrefix = "",
                Claims = new List<Claim>()
                {
                    new Claim("automated-test", "true")
                }
            };
        }
    }
}

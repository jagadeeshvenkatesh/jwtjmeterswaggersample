using JmeterTestBackend.Services;
using JmeterTestBackend.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Collections.Generic;
using System.Diagnostics;

namespace JmeterTestBackend
{
    public class Startup
    {
        private const string IDPUSE_CONFIG = "ActiveIDP";
        private const string AAD_CONFIGSECTION = "AzureAd";
        private const string IDSRV_CONFIGSECTION = "IdentityServer";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            // Read which IDP should be used, actively
            var activeIdp = Configuration.GetValue<string>(IDPUSE_CONFIG);
            if (string.IsNullOrWhiteSpace(activeIdp)) activeIdp = IDSRV_CONFIGSECTION;

            // Adding JWT Bearer Token authentication and validation settings based on appsettings.json
            if (activeIdp.ToLower().Equals(IDSRV_CONFIGSECTION.ToLower()))
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.Authority = Configuration.GetSection(IDSRV_CONFIGSECTION).GetValue<string>("Url");
                            options.Audience = Configuration.GetSection(IDSRV_CONFIGSECTION).GetValue<string>("AudienceUri");
                            options.RequireHttpsMetadata = false;
                        });
            }
            else if (activeIdp.ToLower().Equals(AAD_CONFIGSECTION.ToLower()))
            {
                services.AddAuthentication(AzureADDefaults.JwtBearerAuthenticationScheme)
                        .AddAzureADBearer(options => Configuration.Bind(key: AAD_CONFIGSECTION, instance: options));
                services.Configure<AzureADOptions>(options => Configuration.Bind(key: AAD_CONFIGSECTION, instance: options));
                services.Configure<JwtBearerOptions>(options =>
                {
                    options.Audience = Configuration.GetSection(AAD_CONFIGSECTION).GetValue<string>("AudienceUri");
                });
            }
            else
            {
                throw new System.Exception($"Invalid configuration for element {IDPUSE_CONFIG}. Allowed values are '{AAD_CONFIGSECTION}' or '{IDSRV_CONFIGSECTION}'");
            }

            // Allow access to the HTTP Context from controllers / API implementations
            services.AddHttpContextAccessor();

            // Add the service that implements the business logic
            services.AddTransient<TestService>();

            // Now activate the API controllers
            services.AddControllers();

            // Adding Swashbuckle for Swagger UI Gen
            SwaggerConfig.ConfigureSwaggerServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            SwaggerConfig.ConfigureSwaggerGen(app, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

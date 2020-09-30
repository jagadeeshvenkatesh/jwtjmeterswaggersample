using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace JmeterTestBackend.Swagger
{
    public class SwaggerConfig
    {
        public static void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // Add the definition for the doc and schema generation
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please provide a bearer token for accessing the API",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http
                });
                //// Add the security requirement details
                c.OperationFilter<SwaggerSecurityOperationFilter>();
                // The code below adds security globally regardless of whether operations need it or not
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            },
                //            Scheme = "Bearer",
                //            Name = "Bearer",
                //            In = ParameterLocation.Header
                //        },
                //        new List<string>() { }
                //    }
                //});
            });
        }

        public static void ConfigureSwaggerGen(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((d, r) =>
                {
                    Trace.WriteLine(d.Paths);
                });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API v1");
                //c.OAuthClientId("jmeter-test-driver");
                //c.OAuthClientSecret("jmeter-test-client-secret");
                //c.OAuthRealm("");
                //c.OAuthAppName("");
            });
        }
    }
}

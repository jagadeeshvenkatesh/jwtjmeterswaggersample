using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace JmeterTestBackend.Swagger
{
    public class SwaggerSecurityOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Apply the security requirement for methods with the AuthorizeAttribute, only

            var authAttr = context.MethodInfo
                                  .GetCustomAttributes(true)
                                  .OfType<AuthorizeAttribute>()
                                  .Distinct()
                                  .Concat(context.MethodInfo
                                               .DeclaringType
                                               .GetCustomAttributes(true)
                                               .OfType<AuthorizeAttribute>());
                                  //.Select(a => a.Policy)
                                  //.Distinct();

            if (authAttr.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>()
                {
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "Bearer",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>() { }
                        }
                    }
                };
            }
        }
    }
}

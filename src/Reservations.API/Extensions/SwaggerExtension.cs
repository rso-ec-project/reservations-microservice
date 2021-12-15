using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reservations.API.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "ChargingStations.API", Version = "v1" });

                config.DocumentFilter<ResolvePathVersionFilter>();
                config.OperationFilter<RemovePathVersionFilter>();
                config.OperationFilter<ContentTypeFilter>();
            });

            return services;
        }
    }

    public class ResolvePathVersionFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new OpenApiPaths();

            foreach (var (key, value) in swaggerDoc.Paths)
            {
                if (!key.Contains("v{version}"))
                {
                    paths.Remove(key);

                    continue;
                }

                paths.Add(key.Replace("v{version}", swaggerDoc.Info.Version), value);
            }

            swaggerDoc.Paths = paths;
        }
    }

    internal class RemovePathVersionFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name == "version");
            if (versionParameter != null)
            {
                operation.Parameters.Remove(versionParameter);
            }
        }
    }

    internal class ContentTypeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var responseContent = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new()
            };

            if (operation.Responses.TryGetValue("200", out var successResponse))
            {
                successResponse.Content = responseContent;
            }

            if (operation.Responses.TryGetValue("201", out var createdResponse))
            {
                createdResponse.Content = responseContent;
            }

            if (operation.Responses.TryGetValue("400", out var badRequestResponse))
            {
                badRequestResponse.Content = responseContent;
            }

            if (operation.Responses.TryGetValue("404", out var notFoundResponse))
            {
                notFoundResponse.Content = responseContent;
            }
        }
    }
}

using System.Linq;
using KidsPrize.Http.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace KidsPrize.Http.Swagger
{
    public static class SwaggerExtentions
    {
        public static void SetupDocs(this SwaggerGenOptions opts)
        {
            foreach (var version in ApiVersions.All())
            {
                opts.SwaggerDoc($"v{version}", new Info
                {
                    Title = $"KidsPrize API v{version}",
                    Version = $"{version}"
                });
            }
            opts.SwaggerDoc("latest", new Info
            {
                Title = $"KidsPrize API latest",
                Version = $"{ApiVersions.Latest()}"
            });
            opts.DocInclusionPredicate((docName, apiDesc) =>
            {
                var docVersion = ApiVersion.Parse(
                    docName == "latest" ? ApiVersions.Latest() : docName.Substring(1));
                var actionDesc = apiDesc.ActionDescriptor;

                if (actionDesc.IsApiVersionNeutral())
                {
                    return true;
                }
                if (actionDesc.IsMappedTo(docVersion))
                {
                    return true;
                }
                if (actionDesc.IsImplicitlyMappedTo(docVersion))
                {
                    return true;
                }
                return false;
            });
        }

        public static void SetupEndpoints(this SwaggerUIOptions opts)
        {
            opts.SwaggerEndpoint($"/swagger/latest/swagger.json", $"latest");
            foreach (var version in ApiVersions.All().Reverse())
            {
                var deprecated = ApiVersions.Deprecated().Contains(version);
                opts.SwaggerEndpoint($"/swagger/v{version}/swagger.json", deprecated ? $"v{version} - deprecated" : $"v{version}");
            }
        }
    }
}
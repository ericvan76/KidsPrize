using System.Collections.Generic;
using System.Linq;
using KidsPrize.Http.Versioning;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KidsPrize.Http.Swagger
{
    public class SetVersionInPath : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var version = swaggerDoc.Info.Version;
            var deprecated = ApiVersions.Deprecated().Contains(version);
            var newPaths = new Dictionary<string, PathItem>();
            foreach (var path in swaggerDoc.Paths)
            {
                var key = path.Key;
                if (path.Key.StartsWith("/v{version}"))
                {
                    var operations = new List<Operation>();
                    if (path.Value.Get != null) operations.Add(path.Value.Get);
                    if (path.Value.Post != null) operations.Add(path.Value.Post);
                    if (path.Value.Put != null) operations.Add(path.Value.Put);
                    if (path.Value.Delete != null) operations.Add(path.Value.Delete);
                    foreach (var operation in operations)
                    {
                        var parameter = operation.Parameters.FirstOrDefault(p => p.Required && p.Name == "version" && p.In == "path");
                        operation.Parameters.Remove(parameter);
                        operation.Deprecated = deprecated || operation.Deprecated == true;
                    }
                    key = path.Key.Replace("/v{version}", version == "1" ? string.Empty : $"/v{version}");
                }
                newPaths.Add(key, path.Value);
            }

            swaggerDoc.Paths.Clear();
            foreach (var path in newPaths)
            {
                swaggerDoc.Paths.Add(path.Key, path.Value);
            }

        }
    }
}

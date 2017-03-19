using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KidsPrize.Http.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Http.Versioning
{
    public static class ApiVersions
    {
        public static IEnumerable<string> All()
        {
            return typeof(VersionedController).GetTypeInfo()
                .GetCustomAttributes(typeof(InheritableApiVersionAttribute))
                .OfType<ApiVersionAttribute>()
                .SelectMany(a => a.Versions)
                .OrderBy(v => v.MajorVersion)
                .ThenBy(v => v.MinorVersion)
                .Select(v => v.ToString());
        }

        public static IEnumerable<string> Deprecated()
        {
            return typeof(VersionedController).GetTypeInfo()
                .GetCustomAttributes(typeof(InheritableApiVersionAttribute))
                .OfType<ApiVersionAttribute>()
                .Where(a => a.Deprecated)
                .SelectMany(a => a.Versions)
                .Select(v => v.ToString());
        }

        public static string Latest()
        {
            return All().Last();
        }

    }
}
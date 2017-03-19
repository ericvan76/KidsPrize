using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace KidsPrize.Http.Versioning
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MapToApiVersionFromAttribute : ApiVersionsBaseAttribute, IApiVersionProvider
    {
        public MapToApiVersionFromAttribute(string version)
            : base(ApiVersions.All().SkipWhile(v => v != version).ToArray())
        {
        }

        bool IApiVersionProvider.AdvertiseOnly => false;

        bool IApiVersionProvider.Deprecated => false;
    }
}
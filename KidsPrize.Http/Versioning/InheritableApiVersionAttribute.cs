using System;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Http.Versioning
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class InheritableApiVersionAttribute : ApiVersionAttribute
    {
        public InheritableApiVersionAttribute(string version) : base(version) { }

    }
}
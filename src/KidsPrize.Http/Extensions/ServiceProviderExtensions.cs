using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace KidsPrize.Http.Extensions
{
    public static class GetServices
    {
        public static IEnumerable<object> GetRequiredServices(this IServiceProvider provider, Type serviceType)
        {
            return (IEnumerable<object>)provider.GetRequiredService(typeof(IEnumerable<>).MakeGenericType(serviceType));
        }
    }
}
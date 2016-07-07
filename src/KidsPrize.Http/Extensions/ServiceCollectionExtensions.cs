using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KidsPrize.Bus;
using Microsoft.Extensions.DependencyInjection;

namespace KidsPrize.Http.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AutoRegistration(this IServiceCollection services)
        {
            var assemblies = GetAllAssemblies("KidsPrize", "KidsPrize.Http");
            foreach (var type in assemblies.SelectMany(a => a.ExportedTypes))
            {
                var interfaces = type.GetInterfaces();
                var interfaceType = interfaces.FirstOrDefault(i =>
                    i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>));
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, type);
                    continue;
                }

                if (type.Name.EndsWith("Service"))
                {
                    interfaceType = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
                    if (interfaceType != null)
                    {
                        services.AddScoped(interfaceType, type);
                        continue;
                    }
                }
            }
        }

        private static IEnumerable<Assembly> GetAllAssemblies(params string[] names)
        {
            var assemblies = new List<Assembly>();
            var assembly = Assembly.GetEntryAssembly();
            if (names.Contains(assembly.GetName().Name))
            {
                assemblies.Add(assembly);
            }
            foreach (var ass in assembly.GetReferencedAssemblies().Where(n=>names.Contains(n.Name)))
            {
                assemblies.Add(Assembly.Load(ass));
            }
            return assemblies;
        }
    }
}
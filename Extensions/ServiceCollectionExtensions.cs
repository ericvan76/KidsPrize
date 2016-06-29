using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace KidsPrize.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetEntryAssembly();
            foreach (var type in assembly.GetTypes().Where(t => t.Name.EndsWith("Service")))
            {
                var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, type);
                }
            }
        }

        public static void AddHandlers(this IServiceCollection services)
        {
            var assembly = Assembly.GetEntryAssembly();
            foreach (var type in assembly.GetTypes())
            {
                var interfaceType = type.GetInterfaces().FirstOrDefault(i =>
                    i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>));
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, type);
                }
            }
        }
    }
}
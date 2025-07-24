using Microsoft.Extensions.DependencyInjection;
using PatronusR.Interfaces;
using System.Reflection;

namespace PatronusR.Extensions
{
    /// <summary>
    /// Service registration helper for PatronusR
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers PatronusR services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection for chaining</returns>
        public static IServiceCollection AddPatronusR(this IServiceCollection services)
        {
            services.AddScoped<IPatronusR, Implementation.PatronusR>();
            services.AddScoped<ISender, Implementation.PatronusR>();
            return services;
        }

        /// <summary>
        /// Registers PatronusR services and scans assemblies for handlers
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Assemblies to scan for handlers</param>
        /// <returns>Service collection for chaining</returns>
        public static IServiceCollection AddPatronusR(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped<IPatronusR, Implementation.PatronusR>();
            services.AddScoped<ISender, Implementation.PatronusR>();

            if (assemblies?.Length > 0)
            {
                RegisterHandlersFromAssemblies(services, assemblies);
            }

            return services;
        }

        private static void RegisterHandlersFromAssemblies(IServiceCollection services, Assembly[] assemblies)
        {
            var handlerType = typeof(IRequestHandler<,>);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract)
                    .ToList();

                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();

                    foreach (var @interface in interfaces)
                    {
                        if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == handlerType)
                        {
                            services.AddScoped(@interface, type);
                        }
                    }
                }
            }
        }
    }
}

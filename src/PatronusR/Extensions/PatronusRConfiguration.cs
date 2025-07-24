using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PatronusR.Extensions
{
    /// <summary>
    /// Configuration class for PatronusR services
    /// </summary>
    public class PatronusRConfiguration
    {
        internal IServiceCollection Services { get; }
        internal List<Assembly> AssembliesToScan { get; } = new();

        internal PatronusRConfiguration(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Register services from the specified assembly
        /// </summary>
        /// <param name="assembly">Assembly to scan</param>
        /// <returns>Configuration for chaining</returns>
        public PatronusRConfiguration RegisterServicesFromAssembly(Assembly assembly)
        {
            if (assembly != null && !AssembliesToScan.Contains(assembly))
            {
                AssembliesToScan.Add(assembly);
            }
            return this;
        }

        /// <summary>
        /// Register services from multiple assemblies
        /// </summary>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>Configuration for chaining</returns>
        public PatronusRConfiguration RegisterServicesFromAssemblies(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                RegisterServicesFromAssembly(assembly);
            }
            return this;
        }

        /// <summary>
        /// Register services from assembly containing the specified type
        /// </summary>
        /// <typeparam name="T">Type to get assembly from</typeparam>
        /// <returns>Configuration for chaining</returns>
        public PatronusRConfiguration RegisterServicesFromAssemblyContaining<T>()
        {
            return RegisterServicesFromAssembly(typeof(T).Assembly);
        }

        /// <summary>
        /// Register services from assembly containing the specified type
        /// </summary>
        /// <param name="type">Type to get assembly from</param>
        /// <returns>Configuration for chaining</returns>
        public PatronusRConfiguration RegisterServicesFromAssemblyContaining(Type type)
        {
            return RegisterServicesFromAssembly(type.Assembly);
        }
    }
}

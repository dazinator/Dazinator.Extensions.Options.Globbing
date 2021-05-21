// ReSharper disable once CheckNamespace
// Put in this namespace for convenience of having extension methods auto light up for consumer.

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Dazinator.Extensions.Options.Globbing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class GlobMatchingNamedOptionsExtensions
    {
        public static IServiceCollection AddGlobMatchingNamedOptions<TOptions>(this IServiceCollection services)
            where TOptions : class
        {
            var innerFactoryTypeDescriptor = services.LastOrDefault(a =>
            {
                var descriptorServiceType = a.ServiceType;
                if (descriptorServiceType == typeof(IOptionsFactory<>))
                {
                    return true;
                }

                if (descriptorServiceType == typeof(IOptionsFactory<TOptions>))
                {
                    return true;
                }

                return false;
            });

            if (innerFactoryTypeDescriptor?.ImplementationType == null)
            {
                throw new Exception($"Call .AddOptions() to register required options services first.");
            }

            Type innerFactoryType = innerFactoryTypeDescriptor.ImplementationType;

            if (innerFactoryType.IsGenericTypeDefinition)
            {
                innerFactoryType = innerFactoryType.MakeGenericType(typeof(TOptions));
            }

            // var innerInstance = inner ??
            return services.AddTransient<IOptionsFactory<TOptions>, GlobMatchingNamedOptionsFactory<TOptions>>(
                sp =>
                {
                    var innerFactoryInstance =
                        ActivatorUtilities.CreateInstance(sp, innerFactoryType);
                    var args = new object[] { innerFactoryInstance };
                    return (GlobMatchingNamedOptionsFactory<TOptions>)ActivatorUtilities.CreateInstance(sp,
                        typeof(GlobMatchingNamedOptionsFactory<TOptions>), args);
                });
        }
    }
}

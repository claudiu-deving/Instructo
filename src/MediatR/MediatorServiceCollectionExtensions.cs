using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Messager;

public static class MediatorServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatorServiceConfiguration> configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var serviceConfig = new MediatorServiceConfiguration();
        configuration(serviceConfig);

        // Register Messager services as Scoped instead of Singleton to support scoped dependencies
        services.TryAddScoped<IMediator, Mediator>();
        services.TryAddScoped<ISender>(provider => provider.GetRequiredService<IMediator>());
        services.TryAddScoped<IPublisher>(provider => provider.GetRequiredService<IMediator>());

        foreach(var assembly in serviceConfig.AssembliesToScan)
        {
            RegisterHandlersFromAssembly(services, assembly);
        }

        return services;
    }

    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddMediator(config => config.RegisterServicesFromAssemblies(assemblies));
    }

    private static void RegisterHandlersFromAssembly(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract&&!t.IsInterface&&t.Name.EndsWith("Handler"))
            .ToArray();

        foreach(var type in types)
        {
            RegisterRequestHandlers(services, type);
            RegisterNotificationHandlers(services, type);
        }
    }

    private static void RegisterRequestHandlers(IServiceCollection services, Type type)
    {
        var requestHandlerInterfaces = type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .Where(i =>
            {
                var genericTypeDefinition = i.GetGenericTypeDefinition();
                return genericTypeDefinition==typeof(IRequestHandler<,>)||
                       genericTypeDefinition==typeof(IRequestHandler<>)||
                       IsRequestHandlerInterface(i);
            })
            .ToArray();

        foreach(var @interface in requestHandlerInterfaces)
        {
            services.AddTransient(@interface, type);
        }
    }

    private static void RegisterNotificationHandlers(IServiceCollection services, Type type)
    {
        var notificationHandlerInterfaces = type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .Where(i =>
            {
                var genericTypeDefinition = i.GetGenericTypeDefinition();
                return genericTypeDefinition==typeof(INotificationHandler<>)||
                       IsNotificationHandlerInterface(i);
            })
            .ToArray();

        foreach(var @interface in notificationHandlerInterfaces)
        {
            services.AddTransient(@interface, type);
        }
    }

    private static bool IsRequestHandlerInterface(Type interfaceType)
    {
        if(!interfaceType.IsGenericType)
            return false;

        var baseInterfaces = interfaceType.GetInterfaces();
        return baseInterfaces.Any(baseInterface =>
            baseInterface.IsGenericType&&
            (baseInterface.GetGenericTypeDefinition()==typeof(IRequestHandler<,>)||
             baseInterface.GetGenericTypeDefinition()==typeof(IRequestHandler<>)));
    }

    private static bool IsNotificationHandlerInterface(Type interfaceType)
    {
        if(!interfaceType.IsGenericType)
            return false;

        var baseInterfaces = interfaceType.GetInterfaces();
        return baseInterfaces.Any(baseInterface =>
            baseInterface.IsGenericType&&
            baseInterface.GetGenericTypeDefinition()==typeof(INotificationHandler<>));
    }
}

public class MediatorServiceConfiguration
{
    internal List<Assembly> AssembliesToScan { get; } = new List<Assembly>();

    public MediatorServiceConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        AssembliesToScan.Add(assembly);
        return this;
    }

    public MediatorServiceConfiguration RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        AssembliesToScan.AddRange(assemblies);
        return this;
    }

    public MediatorServiceConfiguration RegisterServicesFromAssemblies(IEnumerable<Assembly> assemblies)
    {
        AssembliesToScan.AddRange(assemblies);
        return this;
    }
}
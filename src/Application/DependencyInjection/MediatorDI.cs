using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.DependencyInjection;

internal static partial class ApplicationDependencyInjection
{
    private static bool IsHandlerInterface(Type type)
    {
        return type == typeof(IRequestHandler<,>) ||
               type == typeof(ICommandHandler<,>) ||
               type == typeof(ICommandHandler<>) ||
               type == typeof(IQueryHandler<,>) ||
               type == typeof(IDomainEventHandler<>);
    }

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the mediator pipeline: dispatcher, handlers, validators and behaviors (Logging, Validation).
        /// </summary>
        public IServiceCollection AddMediator()
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddScoped<IDispatcher, Dispatcher>();
            services.AddHandlersFromAssembly(assembly);
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

            services.AddScoped(typeof(IPipelineBehavior<>), typeof(LoggingBehavior<>));
            services.AddScoped(typeof(IPipelineBehavior<>), typeof(ValidationBehavior<>));

            return services;
        }

        private IServiceCollection AddHandlersFromAssembly(Assembly assembly)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false })
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && IsHandlerInterface(i.GetGenericTypeDefinition()))
                    .Select(i => new { Implementation = t, Interface = i }))
                .ToList();

            foreach (var handler in handlerTypes)
            {
                services.AddScoped(handler.Interface, handler.Implementation);
            }

            return services;
        }
    }
}

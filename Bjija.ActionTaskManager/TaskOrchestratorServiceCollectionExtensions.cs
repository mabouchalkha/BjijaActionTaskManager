using System.Reflection;
using Bjija.ActionTaskManager.Abstractions.Mediator;
using Bjija.ActionTaskManager.Mediator;
using Bjija.ActionTaskManager.Mediator.Behaviors;
using Bjija.TaskOrchestrator.Abstractions;
using Bjija.TaskOrchestrator.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bjija.TaskOrchestrator.Models
{
    public static class TaskOrchestratorServiceCollectionExtensions
    {
        public static IServiceCollection AddBjijaActionTaskManager(this IServiceCollection services, Action<TaskOrchestratorOptions> setupAction = null)
        {
            var options = new TaskOrchestratorOptions();
            setupAction?.Invoke(options);

            services.AddSingleton(options);

            if (options.LoggerFactory is not null)
            {
                services.AddSingleton<ILoggerFactory>(options.LoggerFactory);
            }
            else if (!services.Any(desc => desc.ServiceType == typeof(ILoggerFactory)))
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                });
            }

            services.AddSingleton(typeof(ITaskPipeline<>), typeof(TaskPipeline<>));
            services.AddSingleton<ITaskFactory, Helpers.TaskFactory>();
            services.AddSingleton<ITaskPipelineFactory, TaskPipelineFactory>();
            services.AddSingleton<IActionTaskOrchestrator, TaskOrchestrator>();

            services.AddSingleton<IMediator, Mediator>();

            services.AddTransient(typeof(IRequestBehavior<,>), typeof(LoggingBehavior<,>));
            //services.AddTransient(typeof(IRequestBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IRequestBehavior<,>), typeof(PerformanceBehavior<,>));

            if (options.AutoRegisterHandlers)
            {
                RegisterHandlers(services, options.HandlerAssemblies);
            }


            return services;
        }
        private static void RegisterHandlers(IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null || !assemblies.Any())
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            foreach (var assembly in assemblies)
            {
                var requestHandlerTypes = assembly.GetTypes()
                    .Where(t => t.GetInterfaces()
                        .Any(i => i.IsGenericType &&
                             i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

                foreach (var handlerType in requestHandlerTypes)
                {
                    var handlerInterface = handlerType.GetInterfaces()
                        .First(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

                    services.AddTransient(handlerInterface, handlerType);
                }

                var notificationHandlerTypes = assembly.GetTypes()
                    .Where(t => t.GetInterfaces()
                        .Any(i => i.IsGenericType &&
                             i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)));

                foreach (var handlerType in notificationHandlerTypes)
                {
                    var handlerInterfaces = handlerType.GetInterfaces()
                        .Where(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(INotificationHandler<>));

                    foreach (var handlerInterface in handlerInterfaces)
                    {
                        services.AddTransient(handlerInterface, handlerType);
                    }
                }
            }
        }
    }
}

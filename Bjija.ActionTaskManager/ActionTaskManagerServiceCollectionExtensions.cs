using Bjija.ActionTaskManager.Abstractions;
using Bjija.ActionTaskManager.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bjija.ActionTaskManager.Models
{
    public static class ActionTaskManagerServiceCollectionExtensions
    {
        public static IServiceCollection AddBjijaActionTaskManager(this IServiceCollection services, Action<ActionTaskManagerOptions> setupAction = null)
        {
            var options = new ActionTaskManagerOptions();
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
            services.AddSingleton<IActionTaskManager, ActionTaskManager>();

            return services;
        }
    }

}

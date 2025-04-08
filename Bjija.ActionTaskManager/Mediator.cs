using Bjija.ActionTaskManager.Abstractions.Mediator;
using Bjija.ActionTaskManager.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Bjija.ActionTaskManager.Mediator
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TaskOrchestratorOptions _options;

        public Mediator(IServiceProvider serviceProvider, TaskOrchestratorOptions options)
        {
            _serviceProvider = serviceProvider;
            _options = options;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);

            // Get the concrete handler type and instance
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            var handler = _serviceProvider.GetRequiredService(handlerType);

            // Create a method to execute the handler
            async Task<TResponse> ExecuteHandler()
            {
                // Use reflection to call the Handle method
                var method = handlerType.GetMethod("Handle");
                var resultTask = (Task<TResponse>)method.Invoke(handler, new object[] { request });
                return await resultTask;
            }

            // Check if behaviors should be applied
            if (_options.EnableBehaviorPipeline)
            {
                // Get the concrete behavior type
                var behaviorType = typeof(IRequestBehavior<,>).MakeGenericType(requestType, responseType);

                // Get all behaviors
                var behaviors = _serviceProvider.GetServices(behaviorType);

                // Create the initial handler delegate
                RequestHandlerDelegate<TResponse> pipeline = ExecuteHandler;

                // Build the pipeline in reverse order
                foreach (var behavior in behaviors.Reverse())
                {
                    var currentBehavior = behavior;
                    var currentPipeline = pipeline;

                    pipeline = async () =>
                    {
                        // Use reflection to call the Process method
                        var processMethod = behaviorType.GetMethod("Process");
                        var task = (Task<TResponse>)processMethod.Invoke(
                            currentBehavior,
                            [request, currentPipeline]);

                        return await task;
                    };
                }

                // Execute the pipeline
                return await pipeline();
            }

            // Execute the handler directly
            return await ExecuteHandler();
        }

        public async Task Publish<TNotification>(TNotification notification) where TNotification : INotification
        {
            var notificationType = notification.GetType();
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

            var handlers = _serviceProvider.GetServices(handlerType);

            var method = handlerType.GetMethod("Handle");
            if (method == null)
            {
                throw new InvalidOperationException($"Handler method not found for {handlerType.Name}");
            }

            var tasks = handlers.Select(handler =>
                (Task?)method.Invoke(handler, [notification]));

            await Task.WhenAll(tasks.Where(task => task != null)!);
        }
    }
}
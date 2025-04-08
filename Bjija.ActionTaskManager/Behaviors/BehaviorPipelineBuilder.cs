using Bjija.ActionTaskManager.Abstractions.Mediator;

namespace Bjija.ActionTaskManager.Behaviors
{
    /// <summary>
    /// A behavior pipeline builder for request handlers.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class BehaviorPipelineBuilder<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<object> _behaviors;
        private readonly Type _behaviorType;
        private readonly RequestHandlerDelegate<TResponse> _handlerDelegate;

        public BehaviorPipelineBuilder(
            IEnumerable<object> behaviors,
            RequestHandlerDelegate<TResponse> handlerDelegate)
        {
            _behaviors = behaviors;
            _behaviorType = typeof(IRequestBehavior<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));
            _handlerDelegate = handlerDelegate;
        }

        public async Task<TResponse> Execute(TRequest request)
        {
            // Build the pipeline in reverse order
            RequestHandlerDelegate<TResponse> pipeline = _handlerDelegate;

            foreach (var behavior in _behaviors.Reverse())
            {
                var currentBehavior = behavior;
                var currentPipeline = pipeline;

                pipeline = async () =>
                {
                    // Use reflection to call the Process method
                    var processMethod = _behaviorType.GetMethod("Process");
                    var task = (Task<TResponse>)processMethod.Invoke(
                        currentBehavior,
                        new object[] { request, currentPipeline });

                    return await task;
                };
            }

            // Execute the pipeline
            return await pipeline();
        }
    }
}

using Bjija.ActionTaskManager.Abstractions.Mediator;
using Microsoft.Extensions.Logging;

namespace Bjija.ActionTaskManager.Mediator.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IRequestBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Process(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            var requestType = typeof(TRequest).Name;
            _logger.LogInformation("Handling {RequestType}", requestType);

            try
            {
                var result = await next();
                _logger.LogInformation("Handled {RequestType}", requestType);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {RequestType}", requestType);
                throw;
            }
        }
    }
}
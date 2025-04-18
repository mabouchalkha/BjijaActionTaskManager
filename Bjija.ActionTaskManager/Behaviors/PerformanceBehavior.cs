﻿using Bjija.ActionTaskManager.Abstractions.Mediator;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Bjija.ActionTaskManager.Mediator.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IRequestBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly Stopwatch _timer;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }

        public async Task<TResponse> Process(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogWarning("Long Running Request: {RequestName} ({ElapsedMilliseconds} milliseconds)",
                    requestName, elapsedMilliseconds);
            }

            return response;
        }
    }
}
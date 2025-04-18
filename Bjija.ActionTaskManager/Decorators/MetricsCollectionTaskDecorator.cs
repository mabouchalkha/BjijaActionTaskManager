﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjija.TaskOrchestrator.Abstractions;
using Bjija.TaskOrchestrator.Models;

namespace Bjija.TaskOrchestrator.Decorators
{
    public interface IMetricsService
    {
        void Collect(string metricName, double value = 1.0);
    }

    internal class MetricsCollectionTaskDecorator<T> : TaskDecorator<T>
    {
        private readonly IMetricsService _metricsService;

        public MetricsCollectionTaskDecorator(IActionTask<T> decoratedTask, IMetricsService metricsService)
            : base(decoratedTask)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        }

        protected override async Task ExecuteCoreAsync(ActionEventArgs<T> args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await _decoratedTask.ExecuteAsync(args);
            watch.Stop();

            _metricsService.Collect($"{typeof(T).Name}.ExecutionTime", watch.ElapsedMilliseconds);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjija.ActionTaskManager.Abstractions;
using Bjija.ActionTaskManager.Models;

namespace Bjija.ActionTaskManager.Decorators
{
    public interface IMetricsService
    {
        void Collect(string metricName, double value = 1.0);
    }

    internal class MetricsCollectionTaskDecorator<T> : TaskDecorator<T>
    {
        private readonly IMetricsService _metricsService;

        public MetricsCollectionTaskDecorator(ITask<T> decoratedTask, IMetricsService metricsService)
            : base(decoratedTask)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        }

        protected override async Task ExecuteCoreAsync(object sender, ActionEventArgs<T> args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await _decoratedTask.ExecuteAsync(sender, args);
            watch.Stop();

            _metricsService.Collect($"{typeof(T).Name}.ExecutionTime", watch.ElapsedMilliseconds);
        }
    }

}

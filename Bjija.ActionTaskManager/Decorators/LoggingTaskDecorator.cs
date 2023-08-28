using System.Text.Json;
using Bjija.ActionTaskManager.Abstractions;
using Bjija.ActionTaskManager.Models;
using Microsoft.Extensions.Logging;

namespace Bjija.ActionTaskManager.Decorators
{
    public class LoggingTaskDecorator<T> : TaskDecorator<T>
    {
        private readonly ILogger _logger;

        public LoggingTaskDecorator(ITask<T> decoratedTask, ILogger<LoggingTaskDecorator<T>> logger)
            : base(decoratedTask)
        {
            _logger = logger;
        }

        protected override async Task ExecuteCoreAsync(object sender, ActionEventArgs<T> args)
        {
            var taskName = _decoratedTask.GetType().Name;
            var dataInfo = args != null ? SerializeObjectToString(args.Data) : "No data";

            _logger.LogInformation($"Starting task execution for '{taskName}' with data: {dataInfo}...");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await _decoratedTask.ExecuteAsync(sender, args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while executing '{taskName}'.");
                throw;
            }
            finally
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                _logger.LogInformation($"Finished task execution for '{taskName}' in {elapsedMs}ms.");
            }
        }

        private string SerializeObjectToString(object obj)
        {
            if (obj == null) return "No data";

            try
            {
                return JsonSerializer.Serialize(obj);
            }
            catch
            {
                return $"Failed to serialize {obj.GetType().Name}.";
            }
        }
    }
}

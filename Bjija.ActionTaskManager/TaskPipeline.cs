using System.Threading.Tasks;
using Bjija.TaskOrchestrator.Abstractions;
using Bjija.TaskOrchestrator.Models;
using Microsoft.Extensions.Logging;

namespace Bjija.TaskOrchestrator
{
    public class TaskPipeline<TData> : ITaskPipeline<TData>
    {
        public Action<PipelineEventArgs<TData>> PipelineStarted { get; set; }
        public Action<PipelineEventArgs<TData>> PipelineCompleted { get; set; }
        public Action<PipelineErrorEventArgs<TData>> PipelineFailed { get; set; }

        private readonly SortedList<int, IPipelineTask<TData>> _tasks = new();
        private readonly ILogger<TaskPipeline<TData>> _logger;

        public TaskPipeline(ILogger<TaskPipeline<TData>> logger)
        {
            _logger = logger;
        }

        public void RegisterTask(IPipelineTask<TData> task, int priority = 0, Func<ActionEventArgs<TData>, bool> predicate = null)
        {
            task.Predicate = predicate ?? (_ => true);
            _tasks.Add(priority, task);
        }

        public void RemoveTask(Type taskType)
        {
            var key = _tasks.Keys.First(k => _tasks[k].GetType() == taskType);
            _tasks.Remove(key);
        }

        public void ReplaceTask(Type existingTaskType, IPipelineTask<TData> newTask)
        {
            var key = _tasks.Keys.First(k => _tasks[k].GetType() == existingTaskType);
            _tasks[key] = newTask;
        }

        public async Task ExecutePipelineAsync(ActionEventArgs<TData> args)
        {
            _logger.LogInformation("Executing pipeline...");

            // TODO: Start a transaction here, if needed
            try
            {
                PipelineStarted?.Invoke(new PipelineEventArgs<TData>(args.Data));

                // Optimization: avoid unnecessary LINQ operations and improve performance
                var tasksCount = _tasks.Count;
                if (tasksCount == 0)
                {
                    PipelineCompleted?.Invoke(new PipelineEventArgs<TData>(args.Data));
                    return;
                }

                // Get the tasks in priority order
                var orderedTasks = _tasks.Values.ToArray();

                // Set up the chain
                var firstTask = orderedTasks[0];
                var currentTask = firstTask;

                for (int i = 1; i < orderedTasks.Length; i++)
                {
                    currentTask = currentTask.SetNext(orderedTasks[i]);
                }

                // Execute the chain
                await firstTask.ExecuteChainAsync(args);

                PipelineCompleted?.Invoke(new PipelineEventArgs<TData>(args.Data));
            }
            catch (Exception ex)
            {
                PipelineFailed?.Invoke(new PipelineErrorEventArgs<TData>(args.Data, ex));
                _logger.LogError(ex, "Error executing task: {TaskName}", ex.Message);
                // TODO: Handle error, e.g., rollback transaction
                throw;
            }
            // TODO: Commit transaction here, if needed

            _logger.LogInformation("Pipeline execution completed.");
        }
    }

}

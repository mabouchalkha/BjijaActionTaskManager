using System.Threading.Tasks;
using Bjija.ActionTaskManager.Abstractions;
using Bjija.ActionTaskManager.Models;
using Microsoft.Extensions.Logging;

namespace Bjija.ActionTaskManager
{
    public class TaskPipeline<TData> : ITaskPipeline<TData>
    {
        public event EventHandler<PipelineEventArgs<TData>> PipelineStarted;
        public event EventHandler<PipelineEventArgs<TData>> PipelineCompleted;
        public event EventHandler<PipelineErrorEventArgs<TData>> PipelineFailed;

        private readonly SortedList<int, IChainableTask<TData>> _tasks = new();
        private readonly ILogger<TaskPipeline<TData>> _logger;

        public TaskPipeline(ILogger<TaskPipeline<TData>> logger)
        {
            _logger = logger;
        }

        public void RegisterTask(IChainableTask<TData> task, int priority = 0, Func<ActionEventArgs<TData>, bool> predicate = null)
        {
            task.Predicate = predicate;
            _tasks.Add(priority, task);
        }

        public void RemoveTask(Type taskType)
        {
            var key = _tasks.Keys.First(k => _tasks[k].GetType() == taskType);
            _tasks.Remove(key);
        }

        public void ReplaceTask(Type existingTaskType, IChainableTask<TData> newTask)
        {
            var key = _tasks.Keys.First(k => _tasks[k].GetType() == existingTaskType);
            _tasks[key] = newTask;
        }

        public async Task ExecutePipelineAsync(object sender, ActionEventArgs<TData> args)
        {
            _logger.LogInformation("Executing pipeline...");

            // TODO: Start a transaction here, if needed
            try
            {
                PipelineStarted?.Invoke(this, new PipelineEventArgs<TData>(args.Data));

                IChainableTask<TData> firstTask = null;
                IChainableTask<TData> lastTask = null;

                foreach (var task in _tasks.Values.OfType<IChainableTask<TData>>())
                {
                    if (firstTask == null)
                    {
                        firstTask = task;
                        lastTask = task;
                    }
                    else
                    {
                        lastTask = lastTask.SetNext(task);
                    }
                }

                if (firstTask != null)
                {
                    await firstTask.ExecuteChainAsync(sender, args);
                }

                PipelineCompleted?.Invoke(this, new PipelineEventArgs<TData>(args.Data));
            }
            catch (Exception ex)
            {
                PipelineFailed?.Invoke(this, new PipelineErrorEventArgs<TData>(args.Data, ex));
                _logger.LogError(ex, "Error executing task: {TaskName}", ex.ToString());
                // TODO: Handle error, e.g., rollback transaction
                throw;
            }
            // TODO: Commit transaction here, if needed

            _logger.LogInformation("Pipeline execution completed.");
        }
    }

}

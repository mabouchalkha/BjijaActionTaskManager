using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjija.ActionTaskManager.Models;

namespace Bjija.ActionTaskManager.Abstractions
{
    /// <summary>
    /// Represents a task pipeline.
    /// </summary>
    public interface ITaskPipeline
    {
    }

    /// <summary>
    /// Represents a task pipeline that can be executed with a payload of type TData.
    /// </summary>
    /// <typeparam name="TData">The type of the payload data.</typeparam>
    public interface ITaskPipeline<TData> : ITaskPipeline
    {
        /// <summary>
        /// Event triggered when the pipeline execution starts.
        /// </summary>
        Action<PipelineEventArgs<TData>> PipelineStarted { get; set; }

        /// <summary>
        /// Event triggered when the pipeline execution completes successfully.
        /// </summary>
        Action<PipelineEventArgs<TData>> PipelineCompleted { get; set; }

        /// <summary>
        /// Event triggered when the pipeline execution fails.
        /// </summary>
        Action<PipelineErrorEventArgs<TData>> PipelineFailed { get; set; }

        /// <summary>
        /// Registers a chainable task to the pipeline.
        /// </summary>
        /// <param name="task">The chainable task to register.</param>
        /// <param name="priority">The priority level of the task.</param>
        /// <param name="predicate">The predicate used to determine whether the task should be executed.</param>
        void RegisterTask(IChainableTask<TData> task, int priority = 0, Func<ActionEventArgs<TData>, bool> predicate = null);

        /// <summary>
        /// Removes a task from the pipeline.
        /// </summary>
        /// <param name="taskType">The type of the task to remove.</param>
        void RemoveTask(Type taskType);

        /// <summary>
        /// Replaces an existing task in the pipeline.
        /// </summary>
        /// <param name="existingTaskType">The type of the existing task to replace.</param>
        /// <param name="newTask">The new task to add.</param>
        void ReplaceTask(Type existingTaskType, IChainableTask<TData> newTask);

        /// <summary>
        /// Executes the task pipeline asynchronously.
        /// </summary>
        /// <param name="args">The arguments containing the payload data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ExecutePipelineAsync(ActionEventArgs<TData> args);
    }

}

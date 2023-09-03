using Bjija.ActionTaskManager.Models;

namespace Bjija.ActionTaskManager.Abstractions
{
    /// <summary>
    /// Defines methods for managing tasks based on triggered actions.
    /// </summary>
    public interface IActionTaskManager
    {
        /// <summary>
        /// Adds a universal decorator that will be applied to all tasks.
        /// </summary>
        /// <param name="decoratorType">The type of the decorator.</param>
        /// <returns>The current IActionTaskManager instance.</returns>
        IActionTaskManager AddUniversalDecorator(Type decoratorType);

        /// <summary>
        /// Registers a task to be executed when a specific action is triggered.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <param name="decoratorTypes">An array of decorator types to apply to the task.</param>
        /// <returns>The current IActionTaskManager instance.</returns>
        IActionTaskManager Register<TAction, TData, TTask>(params Type[] decoratorTypes)
                        where TAction : IActionTrigger<TData>
                        where TTask : ITask<TData>, new();

        /// <summary>
        /// Registers a task with a condition to be executed when a specific action is triggered.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <param name="predicate">A function to determine whether the task should be executed.</param>
        /// <param name="decoratorTypes">An array of decorator types to apply to the task.</param>
        /// <returns>The current IActionTaskManager instance.</returns>
        IActionTaskManager Register<TAction, TData, TTask>(Func<ActionEventArgs<TData>, bool> predicate, params Type[] decoratorTypes)
                        where TAction : IActionTrigger<TData>
                        where TTask : ITask<TData>, new();

        /// <summary>
        /// Unregisters a specific task for a specific action.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <returns>The current IActionTaskManager instance.</returns>
        IActionTaskManager UnregisterTask<TAction, TData, TTask>();

        /// <summary>
        /// Registers a profile-based task.
        /// </summary>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <param name="profileName">The name of the profile.</param>
        /// <param name="task">The task to register.</param>
        /// <param name="decoratorTypes">An array of decorator types to apply to the task.</param>
        /// <returns>A ProfileTaskBuilder instance for chaining further configuration.</returns>
        ProfileTaskBuilder<TData> RegisterProfileTask<TData>(string profileName, ITask<TData> task, params Type[] decoratorTypes);

        /// <summary>
        /// Registers a profile-based task with a condition.
        /// </summary>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <param name="profileName">The name of the profile.</param>
        /// <param name="task">The task to register.</param>
        /// <param name="predicate">A function to determine whether the task should be executed.</param>
        /// <param name="decoratorTypes">An array of decorator types to apply to the task.</param>
        /// <returns>A ProfileTaskBuilder instance for chaining further configuration.</returns>
        ProfileTaskBuilder<TData> RegisterProfileTask<TData>(string profileName, ITask<TData> task, Func<ActionEventArgs<TData>, bool> predicate = null, params Type[] decoratorTypes);

        /// <summary>
        /// Registers a task pipeline for a specific action.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <param name="pipeline">The task pipeline to register.</param>
        /// <returns>The current IActionTaskManager instance.</returns>
        IActionTaskManager RegisterPipeline<TAction, TData>(ITaskPipeline<TData> pipeline) where TAction : IActionTrigger<TData>;

        /// <summary>
        /// Registers or adds a task to an existing pipeline for a specific action.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <param name="task">The task to register or add.</param>
        /// <param name="priority">The priority order of the task in the pipeline.</param>
        /// <returns>The current IActionTaskManager instance.</returns>
        IActionTaskManager RegisterOrAddTaskToPipeline<TAction, TData>(IChainableTask<TData> task, int priority = 0) where TAction : IActionTrigger<TData>;

        /// <summary>
        /// Removes a task from a pipeline for a specific action.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <param name="taskType">The type of the task to remove.</param>
        void RemoveTaskFromPipeline<TAction, TData>(Type taskType) where TAction : IActionTrigger<TData>;

        /// <summary>
        /// Replaces an existing task in a pipeline with a new task.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <param name="existingTaskType">The type of the existing task to replace.</param>
        /// <param name="newTask">The new task to add to the pipeline.</param>
        void ReplaceTaskInPipeline<TAction, TData>(Type existingTaskType, IChainableTask<TData> newTask) where TAction : IActionTrigger<TData>;

        /// <summary>
        /// Links an action trigger to its corresponding tasks.
        /// </summary>
        /// <typeparam name="TData">The type of the data payload.</typeparam>
        /// <param name="action">The action to link.</param>
        /// <param name="profileName">The optional profile name for profile-based tasks.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task LinkActionToTasksAsync<TData>(IActionTrigger<TData> action, string profileName = null);
    }
}
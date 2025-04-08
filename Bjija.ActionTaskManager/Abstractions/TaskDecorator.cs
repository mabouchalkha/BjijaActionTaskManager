namespace Bjija.TaskOrchestrator.Abstractions
{
    /// <summary>
    /// Represents a base decorator class for tasks that allows additional behaviors to be added.
    /// </summary>
    /// <typeparam name="T">The type of the payload data.</typeparam>
    public abstract class TaskDecorator<T> : ActionTask<T>
    {
        /// <summary>
        /// The task being decorated.
        /// </summary>
        protected readonly IActionTask<T> _decoratedTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDecorator{T}"/> class.
        /// </summary>
        /// <param name="decoratedTask">The task to be decorated.</param>
        public TaskDecorator(IActionTask<T> decoratedTask)
        {
            _decoratedTask = decoratedTask;
        }
    }
}


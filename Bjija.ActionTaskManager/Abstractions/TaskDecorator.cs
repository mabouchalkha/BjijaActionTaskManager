namespace Bjija.ActionTaskManager.Abstractions
{
    public abstract class TaskDecorator<T> : Task<T>
    {
        protected readonly ITask<T> _decoratedTask;

        public TaskDecorator(ITask<T> decoratedTask)
        {
            _decoratedTask = decoratedTask;
        }
    }
}

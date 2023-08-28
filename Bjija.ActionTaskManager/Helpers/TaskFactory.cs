using Microsoft.Extensions.DependencyInjection;

namespace Bjija.ActionTaskManager.Helpers
{
    public interface ITaskFactory
    {
        T Create<T>(Type type, params object[] parameters);
    }

    public class TaskFactory : ITaskFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create<T>(Type type, params object[] parameters)
        {
            return (T)ActivatorUtilities.CreateInstance(_serviceProvider, type, parameters);
        }
    }
}

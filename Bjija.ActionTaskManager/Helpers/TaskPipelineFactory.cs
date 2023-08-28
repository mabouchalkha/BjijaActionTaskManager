using Bjija.ActionTaskManager.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bjija.ActionTaskManager.Helpers
{
    public interface ITaskPipelineFactory
    {
        ITaskPipeline<TData> CreatePipeline<TData>();
    }

    public class TaskPipelineFactory : ITaskPipelineFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskPipelineFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITaskPipeline<TData> CreatePipeline<TData>()
        {
            return _serviceProvider.GetRequiredService<ITaskPipeline<TData>>();
        }
    }

}

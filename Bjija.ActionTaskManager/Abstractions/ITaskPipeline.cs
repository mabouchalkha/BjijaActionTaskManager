using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjija.ActionTaskManager.Models;

namespace Bjija.ActionTaskManager.Abstractions
{
    public interface ITaskPipeline
    {
    }

    public interface ITaskPipeline<TData> : ITaskPipeline
    {
        event EventHandler<PipelineEventArgs<TData>> PipelineStarted;
        event EventHandler<PipelineEventArgs<TData>> PipelineCompleted;
        event EventHandler<PipelineErrorEventArgs<TData>> PipelineFailed;
        void RegisterTask(IChainableTask<TData> task, int priority = 0, Func<ActionEventArgs<TData>, bool> predicate = null);
        void RemoveTask(Type taskType);
        void ReplaceTask(Type existingTaskType, IChainableTask<TData> newTask);
        Task ExecutePipelineAsync(object sender, ActionEventArgs<TData> args);
    }
}

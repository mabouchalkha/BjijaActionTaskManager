using Bjija.ActionTaskManager.Models;

namespace Bjija.ActionTaskManager.Abstractions
{
    public interface IActionTaskManager
    {
        IActionTaskManager AddUniversalDecorator(Type decoratorType);

        IActionTaskManager Register<TAction, TData, TTask>(params Type[] decoratorTypes) 
                        where TAction : IActionTrigger<TData>
                        where TTask : ITask<TData>, new();
        IActionTaskManager Register<TAction, TData, TTask>(Func<ActionEventArgs<TData>, bool> predicate, params Type[] decoratorTypes)
                        where TAction : IActionTrigger<TData>
                        where TTask : ITask<TData>, new();
        IActionTaskManager UnregisterTask<TAction, TData, TTask>();
        ProfileTaskBuilder<TData> RegisterProfileTask<TData>(string profileName, ITask<TData> task, params Type[] decoratorTypes);
        ProfileTaskBuilder<TData> RegisterProfileTask<TData>(string profileName, ITask<TData> task, Func<ActionEventArgs<TData>, bool> predicate = null, params Type[] decoratorTypes);
        IActionTaskManager RegisterPipeline<TAction, TData>(ITaskPipeline<TData> pipeline) where TAction : IActionTrigger<TData>;
        IActionTaskManager RegisterOrAddTaskToPipeline<TAction, TData>(IChainableTask<TData> task, int priority = 0) where TAction : IActionTrigger<TData>;
        void RemoveTaskFromPipeline<TAction, TData>(Type taskType) where TAction : IActionTrigger<TData>;
        void ReplaceTaskInPipeline<TAction, TData>(Type existingTaskType, IChainableTask<TData> newTask) where TAction : IActionTrigger<TData>;
        Task LinkActionToTasksAsync<TData>(IActionTrigger<TData> action, string profileName = null);
    }
}
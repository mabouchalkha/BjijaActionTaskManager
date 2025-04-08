using System.Collections.Concurrent;
using Bjija.TaskOrchestrator.Abstractions;
using Bjija.TaskOrchestrator.Decorators;
using Bjija.TaskOrchestrator.Exceptions;
using Bjija.TaskOrchestrator.Helpers;
using Bjija.TaskOrchestrator.Models;

namespace Bjija.TaskOrchestrator
{
    public class TaskOrchestrator : IActionTaskOrchestrator
    {
        private readonly List<Type> _universalDecorators = new();
        private readonly ConcurrentDictionary<Type, List<(Type TaskType, List<Type> DecoratorTypes)>> _configuration = new();
        private readonly Dictionary<string, List<IActionTask>> _profileConfiguration = new();
        private readonly Dictionary<Type, ITaskPipeline> _pipelines = new();
        private Dictionary<Type, Func<IActionEventArgs, bool>> _taskPredicates = new Dictionary<Type, Func<IActionEventArgs, bool>>();

        private readonly TaskOrchestratorOptions _actionTaskManagerOptions;
        private readonly ITaskFactory _taskFactory;
        private readonly ITaskPipelineFactory _taskPipelineFactory;

        public TaskOrchestrator(TaskOrchestratorOptions actionTaskManagerOptions, ITaskFactory taskFactory, ITaskPipelineFactory taskPipelineFactory)
        {
            _actionTaskManagerOptions = actionTaskManagerOptions ?? throw new ArgumentNullException(nameof(actionTaskManagerOptions));
            _taskFactory = taskFactory ?? throw new ArgumentNullException(nameof(taskFactory));
            _taskPipelineFactory = taskPipelineFactory ?? throw new ArgumentNullException(nameof(taskPipelineFactory));
        }

        public IActionTaskOrchestrator AddUniversalDecorator(Type decoratorType)
        {
            if (decoratorType == null)
            {
                throw new ArgumentNullException(nameof(decoratorType));
            }

            if (!_universalDecorators.Contains(decoratorType))
            {
                _universalDecorators.Add(decoratorType);
            }

            return this;
        }

        public IActionTaskOrchestrator RegisterTask<TAction, TData, TTask>(params Type[] decoratorTypes) 
            where TAction : IActionTrigger<TData>
            where TTask : IActionTask<TData>, new()
        {
            return RegisterTask<TAction, TData, TTask>(null, decoratorTypes);
        }

        public IActionTaskOrchestrator RegisterTask<TAction, TData, TTask>(Func<ActionEventArgs<TData>, bool>? predicate = null, params Type[] decoratorTypes)
            where TAction : IActionTrigger<TData>
            where TTask : IActionTask<TData>, new()
        {
            predicate ??= _ => true;
            var actionType = typeof(TAction);
            var taskType = typeof(TTask);
            if (_configuration.ContainsKey(actionType))
            {
                _configuration[actionType].RemoveAll(x => x.TaskType == taskType);
                _configuration[actionType].Add((typeof(TTask), new List<Type>(decoratorTypes)));
            }
            else
            {
                _configuration[actionType] = new List<(Type TaskType, List<Type> DecoratorTypes)> { (typeof(TTask), new List<Type>(decoratorTypes)) };
            }

            _taskPredicates[taskType] = args =>
            {
                if (args is ActionEventArgs<TData> typedArgs)
                {
                    return predicate != null ? predicate(typedArgs) : false;
                }
                return false; // or some other default value or exception
            };

            return this;
        }

        public IActionTaskOrchestrator UnregisterTask<TAction, TData, TTask>()
        {
            var actionType = typeof(TAction);
            var taskType = typeof(TTask);
            if (_configuration.ContainsKey(actionType))
            {
                _configuration[actionType].RemoveAll(x => x.TaskType == taskType);
            }

            return this;
        }

        public ProfileTaskBuilder<TData> RegisterProfileTask<TData>(string profileName, IActionTask<TData> task, params Type[] decoratorTypes)
        {
            return RegisterProfileTask(profileName, task, null, decoratorTypes);
        }

        public ProfileTaskBuilder<TData> RegisterProfileTask<TData>(string profileName, IActionTask<TData> task, Func<ActionEventArgs<TData>, bool> predicate, params Type[] decoratorTypes)
        {
            if (string.IsNullOrEmpty(profileName))
            {
                throw new ArgumentException("Profile name cannot be null or empty", nameof(profileName));
            }

            predicate ??= _ => true;

            task = DecorateTask(task, decoratorTypes);

            if (_profileConfiguration.ContainsKey(profileName))
            {
                _profileConfiguration[profileName].Add(task);
            }
            else
            {
                _profileConfiguration[profileName] = new List<IActionTask> { task };
            }

            _taskPredicates[task.GetType()] = args =>
            {
                if (args is ActionEventArgs<TData> typedArgs)
                {
                    return predicate != null ? predicate(typedArgs) : false;
                }
                return false; // or some other default value or exception
            };

            return new ProfileTaskBuilder<TData>(profileName, this);
        }

        public IActionTaskOrchestrator RegisterPipeline<TAction, TData>(ITaskPipeline<TData> pipeline) where TAction : IActionTrigger<TData>
        {
            _pipelines[typeof(TAction)] = pipeline;

            return this;
        }

        public IActionTaskOrchestrator RegisterOrAddTaskToPipeline<TAction, TData>(IPipelineTask<TData> task, int priority = 0) where TAction : IActionTrigger<TData>
        {
            if (!_pipelines.ContainsKey(typeof(TAction)))
            {
                _pipelines[typeof(TAction)] = _taskPipelineFactory.CreatePipeline<TData>();
            }

            var pipeline = (TaskPipeline<TData>)_pipelines[typeof(TAction)];
            pipeline.RegisterTask(task, priority);

            return this;
        }

        public void RemoveTaskFromPipeline<TAction, TData>(Type taskType) where TAction : IActionTrigger<TData>
        {
            if (_pipelines.TryGetValue(typeof(TAction), out var pipeline))
            {
                ((ITaskPipeline<TData>)pipeline).RemoveTask(taskType);
            }
            else
            {
                throw new PipelineNotFoundException(typeof(TAction));
            }
        }

        public void ReplaceTaskInPipeline<TAction, TData>(Type existingTaskType, IPipelineTask<TData> newTask) where TAction : IActionTrigger<TData>
        {
            if (_pipelines.TryGetValue(typeof(TAction), out var pipeline))
            {
                ((ITaskPipeline<TData>)pipeline).ReplaceTask(existingTaskType, newTask);
            }
            else
            {
                throw new PipelineNotFoundException(typeof(TAction));
            }
        }

        public Task LinkActionToTasksAsync<TData>(IActionTrigger<TData> action, string profileName = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (_configuration.Count == 0 && _profileConfiguration.Count == 0 && _pipelines.Count == 0)
            {
                return Task.CompletedTask;
            }

            var actionType = action.GetType();
            profileName ??= action.ProfileName;

            if (_actionTaskManagerOptions.EnableLoggingDecorator)
            {
                AddUniversalDecorator(typeof(LoggingTaskDecorator<TData>));
            }

            if (_pipelines.TryGetValue(actionType, out var pipelineObj) && pipelineObj is ITaskPipeline<TData> pipeline)
            {
                action.ActionOccurred += async args => await pipeline.ExecutePipelineAsync(args).ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(profileName) && _profileConfiguration.ContainsKey(profileName))
            {
                foreach (var taskBase in _profileConfiguration[profileName])
                {
                    if (taskBase is IActionTask<TData> task)
                    {
                        Func<ActionEventArgs<TData>, bool> predicate = null;
                        if (_taskPredicates.TryGetValue(task.GetType(), out var storedPredicate))
                        {
                            predicate = args => storedPredicate(args);
                        }

                        task = DecorateTask(task, _universalDecorators);
                        action.ActionOccurred += async args =>
                        {
                            if (predicate == null || predicate(args))
                            {
                                await task.ExecuteAsync(args).ConfigureAwait(false);
                            }
                        };
                    }
                }
            }

            if (_configuration.ContainsKey(actionType))
            {
                foreach (var (taskType, decoratorTypes) in _configuration[actionType])
                {
                    var taskInstance = _taskFactory.Create<IActionTask<TData>>(taskType);
                    taskInstance = DecorateTask(taskInstance, _universalDecorators.Concat(decoratorTypes));

                    Func<ActionEventArgs<TData>, bool> predicate = null;
                    if (_taskPredicates.TryGetValue(taskType, out var storedPredicate))
                    {
                        predicate = args => storedPredicate(args);
                    }

                    action.ActionOccurred += async args =>
                    {
                        if (predicate == null || predicate(args))
                        {
                            await taskInstance.ExecuteAsync(args).ConfigureAwait(false);
                        }
                    };
                }
            }

            return Task.CompletedTask;
        }

        private IActionTask<TData> DecorateTask<TData>(IActionTask<TData> task, IEnumerable<Type> decoratorTypes)
        {
            foreach (var decoratorType in decoratorTypes)
            {
                task = _taskFactory.Create<IActionTask<TData>>(decoratorType, task);
            }
            return task;
        }
    }
}

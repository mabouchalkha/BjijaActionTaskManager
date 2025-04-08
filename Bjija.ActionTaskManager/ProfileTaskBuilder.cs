using Bjija.TaskOrchestrator.Abstractions;
using Bjija.TaskOrchestrator.Models;

namespace Bjija.TaskOrchestrator
{
    public class ProfileTaskBuilder<TData>
    {
        private readonly string _profileName;
        private readonly IActionTaskOrchestrator _taskManager;

        public ProfileTaskBuilder(string profileName, IActionTaskOrchestrator taskManager)
        {
            ArgumentNullException.ThrowIfNull(nameof(profileName));
            ArgumentNullException.ThrowIfNull(nameof(taskManager));

            _profileName = profileName;
            _taskManager = taskManager;
        }

        public ProfileTaskBuilder<TData> Register(IActionTask<TData> task, Func<ActionEventArgs<TData>, bool> predicate = null, params Type[] decoratorTypes)
        {
            _taskManager.RegisterProfileTask(_profileName, task, predicate, decoratorTypes);
            return this;
        }

        public ProfileTaskBuilder<TData> Register(IActionTask<TData> task, params Type[] decoratorTypes)
        {
            return Register(task, null, decoratorTypes);
        }
    }
}

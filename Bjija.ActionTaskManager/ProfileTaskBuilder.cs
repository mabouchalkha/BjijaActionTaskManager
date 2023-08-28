using Bjija.ActionTaskManager.Abstractions;
using Bjija.ActionTaskManager.Models;

namespace Bjija.ActionTaskManager
{
    public class ProfileTaskBuilder<TData>
    {
        private readonly string _profileName;
        private readonly IActionTaskManager _taskManager;

        public ProfileTaskBuilder(string profileName, IActionTaskManager taskManager)
        {
            ArgumentNullException.ThrowIfNull(nameof(profileName));
            ArgumentNullException.ThrowIfNull(nameof(taskManager));

            _profileName = profileName;
            _taskManager = taskManager;
        }

        public ProfileTaskBuilder<TData> Register(ITask<TData> task, Func<ActionEventArgs<TData>, bool> predicate = null, params Type[] decoratorTypes)
        {
            _taskManager.RegisterProfileTask(_profileName, task, predicate, decoratorTypes);
            return this;
        }

        public ProfileTaskBuilder<TData> Register(ITask<TData> task, params Type[] decoratorTypes)
        {
            return Register(task, null, decoratorTypes);
        }
    }
}

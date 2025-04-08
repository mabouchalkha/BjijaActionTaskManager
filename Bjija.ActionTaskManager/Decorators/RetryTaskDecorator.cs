using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjija.TaskOrchestrator.Abstractions;
using Bjija.TaskOrchestrator.Models;

namespace Bjija.ActionTaskManager.Decorators
{
    public class RetryTaskDecorator<T> : TaskDecorator<T>
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _delay;

        public RetryTaskDecorator(IActionTask<T> decoratedTask, int maxRetries = 3, TimeSpan? delay = null)
            : base(decoratedTask)
        {
            _maxRetries = maxRetries;
            _delay = delay ?? TimeSpan.FromSeconds(1);
        }

        protected override async Task ExecuteCoreAsync(ActionEventArgs<T> args)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    await _decoratedTask.ExecuteAsync(args);
                    return;
                }
                catch (Exception) when (attempts <= _maxRetries)
                {
                    await Task.Delay(_delay);
                }
            }
        }
    }
}

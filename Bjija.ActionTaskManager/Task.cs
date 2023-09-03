using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjija.ActionTaskManager.Models;

namespace Bjija.ActionTaskManager
{
    public abstract class Task<TData> : ITask<TData>
    {
        public Predicate<ActionEventArgs<TData>> Predicate { get; protected set; }

        public Task(Predicate<ActionEventArgs<TData>> predicate = null)
        {
            Predicate = predicate;
        }

        public virtual async Task ExecuteAsync(ActionEventArgs<TData> args)
        {
            if (Predicate == null || Predicate(args))
            {
                await ExecuteCoreAsync(args);
            }
        }

        protected abstract Task ExecuteCoreAsync(ActionEventArgs<TData> args);
    }
}

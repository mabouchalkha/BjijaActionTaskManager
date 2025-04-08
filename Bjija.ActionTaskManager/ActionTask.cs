using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjija.TaskOrchestrator.Models;

namespace Bjija.TaskOrchestrator
{
    public abstract class ActionTask<TData> : IActionTask<TData>
    {
        public Predicate<ActionEventArgs<TData>> Predicate { get; protected set; }

        public ActionTask(Predicate<ActionEventArgs<TData>> predicate = null)
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

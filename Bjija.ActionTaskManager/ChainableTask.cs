using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjija.ActionTaskManager.Models;

namespace Bjija.ActionTaskManager
{
    public abstract class ChainableTask<TData> : IChainableTask<TData>
    {
        private IChainableTask<TData> _next;

        public Func<ActionEventArgs<TData>, bool> Predicate { get; set; }

        public IChainableTask<TData> Next => _next;

        public IChainableTask<TData> SetNext(IChainableTask<TData> nextTask)
        {
            _next = nextTask;
            return _next;
        }

        public async Task ExecuteChainAsync(ActionEventArgs<TData> args)
        {
            if (Predicate == null || Predicate(args))
            {
                await ExecuteAsync(args);
            }

            if (_next != null)
            {
                if (_next.Predicate == null || _next.Predicate(args))
                {
                    await _next.ExecuteChainAsync(args);
                }
            }
        }

        public abstract Task ExecuteAsync(ActionEventArgs<TData> args);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjija.TaskOrchestrator.Models;

namespace Bjija.TaskOrchestrator
{
    public abstract class PipelineTask<TData> : IPipelineTask<TData>
    {
        private IPipelineTask<TData> _next;

        public Func<ActionEventArgs<TData>, bool> Predicate { get; set; }

        public IPipelineTask<TData> Next => _next;

        public IPipelineTask<TData> SetNext(IPipelineTask<TData> nextTask)
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

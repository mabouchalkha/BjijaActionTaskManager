using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bjija.TaskOrchestrator.Exceptions
{
    public class PipelineNotFoundException : Exception
    {
        public PipelineNotFoundException(Type actionType)
            : base($"No pipeline found for action type {actionType.FullName}")
        {
        }
    }
}

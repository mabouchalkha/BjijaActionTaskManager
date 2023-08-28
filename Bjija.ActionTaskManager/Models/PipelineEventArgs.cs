using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bjija.ActionTaskManager.Models
{
    public class PipelineEventArgs<TData> : EventArgs
    {
        public TData Data { get; }

        public PipelineEventArgs(TData data)
        {
            Data = data;
        }
    }

    public class PipelineErrorEventArgs<TData> : PipelineEventArgs<TData>
    {
        public Exception Error { get; }

        public PipelineErrorEventArgs(TData data, Exception error) : base(data)
        {
            Error = error;
        }
    }
}

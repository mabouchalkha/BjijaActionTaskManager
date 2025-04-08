namespace Bjija.TaskOrchestrator.Models;

/// <summary>
/// Provides a strongly-typed version of <see cref="EventArgs"/> for pipeline events.
/// </summary>
/// <typeparam name="TData">The type of the data associated with the pipeline event.</typeparam>
public class PipelineEventArgs<TData> : EventArgs
{
    /// <summary>
    /// Gets the data associated with the pipeline event.
    /// </summary>
    public TData Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineEventArgs{TData}"/> class.
    /// </summary>
    /// <param name="data">The data to associate with the pipeline event.</param>
    public PipelineEventArgs(TData data)
    {
        Data = data;
    }
}

/// <summary>
/// Provides a strongly-typed version of <see cref="PipelineEventArgs{TData}"/> for pipeline error events.
/// </summary>
/// <typeparam name="TData">The type of the data associated with the pipeline event.</typeparam>
public class PipelineErrorEventArgs<TData> : PipelineEventArgs<TData>
{
    /// <summary>
    /// Gets the exception that caused the pipeline to fail.
    /// </summary>
    public Exception Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineErrorEventArgs{TData}"/> class.
    /// </summary>
    /// <param name="data">The data to associate with the pipeline event.</param>
    /// <param name="error">The exception that caused the pipeline to fail.</param>
    public PipelineErrorEventArgs(TData data, Exception error) : base(data)
    {
        Error = error;
    }
}





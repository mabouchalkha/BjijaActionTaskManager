using System.Reflection;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides options for configuring the TaskOrchestrator.
/// </summary>
public class TaskOrchestratorOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to enable the logging decorator for all tasks.
    /// </summary>
    public bool EnableLoggingDecorator { get; set; } = false;

    /// <summary>
    /// Gets or sets the logger factory to use.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically scan for and register handlers.
    /// </summary>
    public bool AutoRegisterHandlers { get; set; } = true;

    /// <summary>
    /// Gets or sets the assemblies to scan for handlers.
    /// </summary>
    public IEnumerable<Assembly> HandlerAssemblies { get; set; } = Enumerable.Empty<Assembly>();

    /// <summary>
    /// Gets or sets a value indicating whether to enable behavior pipeline for request handlers.
    /// </summary>
    public bool EnableBehaviorPipeline { get; set; } = true;
}


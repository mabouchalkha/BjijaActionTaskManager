using Microsoft.Extensions.Logging;

/// <summary>
/// Provides options for configuring the ActionTaskManager.
/// </summary>
public class ActionTaskManagerOptions
{
    public bool EnableLoggingDecorator { get; set; } = false;

    public ILoggerFactory LoggerFactory { get; set; }
}


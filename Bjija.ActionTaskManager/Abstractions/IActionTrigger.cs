using Bjija.ActionTaskManager.Models;

/// <summary>
/// Represents an action that can trigger one or more tasks.
/// </summary>
/// <typeparam name="TData">The type of the payload data that will be passed to the tasks.</typeparam>
public interface IActionTrigger<TData>
{
    /// <summary>
    /// Occurs when an action is triggered.
    /// </summary>
    Action<ActionEventArgs<TData>> ActionOccurred { get; set; }

    /// <summary>
    /// Gets the profile name associated with this action.
    /// </summary>
    /// <value>
    /// The name of the profile.
    /// </value>
    string ProfileName { get; }
}


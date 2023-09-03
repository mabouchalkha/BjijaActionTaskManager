using Bjija.ActionTaskManager.Models;

/// <summary>
/// Represents a task that can be executed.
/// </summary>
public interface ITask
{
}

/// <summary>
/// Represents a task that can be executed with a payload of type TData.
/// </summary>
/// <typeparam name="TData">The type of the payload data.</typeparam>
public interface ITask<TData> : ITask
{
    /// <summary>
    /// Gets the predicate used to determine whether the task should be executed.
    /// </summary>
    Predicate<ActionEventArgs<TData>> Predicate { get; }

    /// <summary>
    /// Executes the task asynchronously.
    /// </summary>
    /// <param name="args">The arguments containing the payload data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteAsync(ActionEventArgs<TData> args);
}

/// <summary>
/// Represents a chainable task that can be executed with a payload of type TData.
/// </summary>
/// <typeparam name="TData">The type of the payload data.</typeparam>
public interface IChainableTask<TData> : ITask
{
    /// <summary>
    /// Sets the next task in the chain.
    /// </summary>
    /// <param name="handler">The next task in the chain.</param>
    /// <returns>The current task.</returns>
    IChainableTask<TData> SetNext(IChainableTask<TData> handler);

    /// <summary>
    /// Gets the next task in the chain.
    /// </summary>
    IChainableTask<TData> Next { get; }

    /// <summary>
    /// Gets or sets the predicate used to determine whether the task should be executed.
    /// </summary>
    Func<ActionEventArgs<TData>, bool> Predicate { get; set; }

    /// <summary>
    /// Executes the task chain asynchronously.
    /// </summary>
    /// <param name="args">The arguments containing the payload data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteChainAsync(ActionEventArgs<TData> args);
}

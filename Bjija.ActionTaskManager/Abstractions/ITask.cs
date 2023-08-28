using Bjija.ActionTaskManager.Models;

public interface ITask
{
}

public interface ITask<TData> : ITask
{
    Predicate<ActionEventArgs<TData>> Predicate { get; }
    Task ExecuteAsync(object sender, ActionEventArgs<TData> args);
}

public interface IChainableTask<TData> : ITask
{
    IChainableTask<TData> SetNext(IChainableTask<TData> handler);
    IChainableTask<TData> Next { get; }
    Func<ActionEventArgs<TData>, bool> Predicate { get; set; }
    Task ExecuteChainAsync(object sender, ActionEventArgs<TData> args);
}
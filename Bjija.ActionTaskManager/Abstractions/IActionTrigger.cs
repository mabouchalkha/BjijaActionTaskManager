using Bjija.ActionTaskManager.Models;

public interface IActionTrigger<TData>
{
    event EventHandler<ActionEventArgs<TData>> ActionOccurred;
    string ProfileName { get; }
}

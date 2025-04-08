namespace Bjija.TaskOrchestrator.Models
{
    /// <summary>
    /// Defines an interface for action event arguments that can hold shared data.
    /// </summary>
    public interface IActionEventArgs
    {
        /// <summary>
        /// Gets a dictionary of shared data that can be accessed by multiple tasks.
        /// </summary>
        Dictionary<string, object> SharedData { get; }
    }

    /// <summary>
    /// Provides a strongly-typed version of <see cref="EventArgs"/> for action events.
    /// </summary>
    /// <typeparam name="TData">The type of the data associated with the action.</typeparam>
    public class ActionEventArgs<TData> : EventArgs, IActionEventArgs
    {
        /// <summary>
        /// Gets the data associated with the action event.
        /// </summary>
        public TData Data { get; }

        /// <summary>
        /// Gets a dictionary of shared data that can be accessed by multiple tasks.
        /// </summary>
        public Dictionary<string, object> SharedData { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventArgs{TData}"/> class.
        /// </summary>
        /// <param name="data">The data to associate with the action event.</param>
        public ActionEventArgs(TData data)
        {
            Data = data;
        }
    }
}

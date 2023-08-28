namespace Bjija.ActionTaskManager.Models
{
    public interface IActionEventArgs
    {
        Dictionary<string, object> SharedData { get; }
    }

    public class ActionEventArgs<TData> : EventArgs, IActionEventArgs
    {
        public TData Data { get; }
        public Dictionary<string, object> SharedData { get; } = new();

        public ActionEventArgs(TData data)
        {
            Data = data;
        }
    }
}

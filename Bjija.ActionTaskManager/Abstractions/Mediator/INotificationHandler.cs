namespace Bjija.ActionTaskManager.Abstractions.Mediator
{
    public interface INotificationHandler<TNotification> where TNotification : INotification
    {
        Task Handle(TNotification notification);
    }
}
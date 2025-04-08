namespace Bjija.ActionTaskManager.Abstractions.Mediator
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request);
        Task Publish<TNotification>(TNotification notification) where TNotification : INotification;
    }
}
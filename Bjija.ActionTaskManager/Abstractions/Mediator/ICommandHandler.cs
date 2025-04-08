namespace Bjija.ActionTaskManager.Abstractions.Mediator
{
    public interface ICommandHandler<TCommand, TResult>
    {
        Task<TResult> Handle(TCommand command);
    }
}

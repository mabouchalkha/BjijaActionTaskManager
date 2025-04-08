namespace Bjija.ActionTaskManager.Abstractions.Mediator
{
    public interface IQueryHandler<TQuery, TResult>
    {
        Task<TResult> Handle(TQuery query);
    }
}

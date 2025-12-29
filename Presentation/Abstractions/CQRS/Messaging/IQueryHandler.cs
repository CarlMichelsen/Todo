namespace Presentation.Abstractions.CQRS.Messaging;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : class?
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
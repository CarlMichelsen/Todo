namespace Presentation.Abstractions.CQRS.Messaging;

public interface ISender
{
    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        where TResponse : class;
    
    Task Send(ICommand command, CancellationToken cancellationToken);
}
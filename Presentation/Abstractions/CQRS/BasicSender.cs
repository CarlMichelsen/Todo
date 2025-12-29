using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Abstractions.CQRS.Messaging;

namespace Presentation.Abstractions.CQRS;

public class BasicSender(
    IServiceScopeFactory serviceScopeFactory,
    Channel<ICommand> commandChannel) : ISender
{
    public async Task<TResponse> Send<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        // Queries should be scoped
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        
        // Get appropriate query handler identifier type
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
        
        // Instantiate the handler from identifier type
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod(nameof(IQueryHandler<,>.Handle))!;
        
        // Execute query
        var queryTask = (Task<TResponse>)handleMethod.Invoke(handler, [query, cancellationToken])!;
        
        // Await completion of query execution and return result
        return await queryTask;
    }

    public async Task Send(
        ICommand command,
        CancellationToken cancellationToken)
    {
        // This can throw:
        // - ChannelClosedException if channel is closed
        // - OperationCanceledException if cancellationToken is cancelled
        await commandChannel.Writer.WriteAsync(command, cancellationToken);
    }
}
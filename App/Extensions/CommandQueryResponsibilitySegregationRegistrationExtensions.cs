using System.Threading.Channels;
using Presentation.Abstractions.CQRS;
using Presentation.Abstractions.CQRS.Messaging;

namespace App.Extensions;

public static class CommandQueryResponsibilitySegregationRegistrationExtensions
{
    // ReSharper disable once InconsistentNaming
    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        return services
            .AddHostedService<CommandConsumer>()
            .AddSingleton(
                _ => Channel.CreateUnbounded<ICommand>(new UnboundedChannelOptions
                {
                    SingleReader = true,
                    AllowSynchronousContinuations = false,
                }))
            .AddSingleton<ISender, BasicSender>();
    }

    public static IServiceCollection AddCommandHandler<TCommandHandler, TCommand>(this IServiceCollection services)
        where TCommandHandler : class, ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        return services.AddScoped<ICommandHandler<TCommand>, TCommandHandler>();
    }
    
    public static IServiceCollection AddQueryHandler<TQueryHandler, TQuery, TResponse>(this IServiceCollection services)
        where TQueryHandler : class, IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : class?
    {
        return services.AddScoped<IQueryHandler<TQuery, TResponse>, TQueryHandler>();
    }
}
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

    public static IServiceCollection AddCommandHandler<TCommand, TCommandHandler>(this IServiceCollection services)
        where TCommand : ICommand
        where TCommandHandler : class, ICommandHandler<TCommand>
    {
        return services.AddScoped<ICommandHandler<TCommand>, TCommandHandler>();
    }
    
    public static IServiceCollection AddQueryHandler<TQuery, TResponse, TQueryHandler>(this IServiceCollection services)
        where TQuery : IQuery<TResponse>
        where TResponse : class
        where TQueryHandler : class, IQueryHandler<TQuery, TResponse>
    {
        return services.AddScoped<IQueryHandler<TQuery, TResponse>, TQueryHandler>();
    }
}
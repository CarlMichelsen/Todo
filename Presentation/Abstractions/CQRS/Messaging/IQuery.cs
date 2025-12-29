namespace Presentation.Abstractions.CQRS.Messaging;

#pragma warning disable S2326 // Unused type parameters should be removed
#pragma warning disable CA1040 // Avoid empty interfaces
public interface IQuery<out TResponse>
    where TResponse : class;
#pragma warning restore CA1040
#pragma warning restore S2326
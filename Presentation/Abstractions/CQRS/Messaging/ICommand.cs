namespace Presentation.Abstractions.CQRS.Messaging;

public interface ICommand
{
    public string Type { get; }
    
    public Guid TransactionId { get; }

    public JwtUser User { get; }
}
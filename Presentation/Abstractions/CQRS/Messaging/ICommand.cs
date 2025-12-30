namespace Presentation.Abstractions.CQRS.Messaging;

public interface ICommand
{
    public string Type { get; }
    
    public Guid CommandId { get; }

    public JwtUser User { get; }
}
namespace Presentation.Abstractions.CQRS.Messaging;

public interface ICommand
{
    string Type { get; }
}
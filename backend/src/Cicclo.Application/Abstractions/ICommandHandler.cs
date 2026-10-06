namespace Cicclo.Application.Abstractions;

// Comandos alteram estado.
public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}

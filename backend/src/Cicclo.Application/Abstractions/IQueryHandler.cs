namespace Cicclo.Application.Abstractions;

// Consultas apenas leem dados.
public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}

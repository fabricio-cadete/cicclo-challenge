using Cicclo.Application.Exceptions;
using Cicclo.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cicclo.Api.ExceptionHandling;

// Converte exceções conhecidas em respostas HTTP; qualquer outra vira 500.
public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            ServiceNotFoundException => (StatusCodes.Status404NotFound, "Serviço não encontrado"),
            RequestIdConflictException => (StatusCodes.Status409Conflict, "Solicitação em conflito"),
            InsufficientBalanceException => (StatusCodes.Status422UnprocessableEntity, "Saldo insuficiente"),
            DomainException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            _ => (0, null)
        };

        if (title is null)
            return false;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = status, Title = title, Detail = exception.Message },
            cancellationToken);

        return true;
    }
}

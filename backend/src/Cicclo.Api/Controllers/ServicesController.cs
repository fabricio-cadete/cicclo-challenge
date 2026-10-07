using Cicclo.Api.Contracts;
using Cicclo.Application.Abstractions;
using Cicclo.Application.Commands;
using Cicclo.Application.Dtos;
using Cicclo.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Cicclo.Api.Controllers;

[ApiController]
[Route("services")]
public sealed class ServicesController(
    IQueryHandler<ListServicesQuery, IReadOnlyList<ServiceDto>> listServicesHandler,
    ICommandHandler<ExecuteServiceCommand, ServiceExecutionDto> executeServiceHandler)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ServiceDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var services = await listServicesHandler.Handle(
            new ListServicesQuery(),
            cancellationToken);

        return Ok(services);
    }

    [HttpPost("{serviceId:guid}/execute")]
    public async Task<ActionResult<ServiceExecutionDto>> Execute(
        Guid serviceId,
        ExecuteServiceRequest request,
        CancellationToken cancellationToken)
    {
        var execution = await executeServiceHandler.Handle(
            new ExecuteServiceCommand(serviceId, request.RequestId),
            cancellationToken);

        return Ok(execution);
    }
}

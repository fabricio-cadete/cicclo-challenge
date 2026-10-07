using Cicclo.Api.Contracts;
using Cicclo.Application.Abstractions;
using Cicclo.Application.Commands;
using Cicclo.Application.Dtos;
using Cicclo.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Cicclo.Api.Controllers;

[ApiController]
[Route("wallet")]
public sealed class WalletController(
    IQueryHandler<GetWalletQuery, WalletDto> getWalletHandler,
    ICommandHandler<AddBalanceCommand, WalletDto> addBalanceHandler)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WalletDto>> Get(CancellationToken cancellationToken)
    {
        var wallet = await getWalletHandler.Handle(
            new GetWalletQuery(),
            cancellationToken);

        return Ok(wallet);
    }

    [HttpPost("deposit")]
    public async Task<ActionResult<WalletDto>> Deposit(
        AddBalanceRequest request,
        CancellationToken cancellationToken)
    {
        var wallet = await addBalanceHandler.Handle(
            new AddBalanceCommand(request.Amount),
            cancellationToken);

        return Ok(wallet);
    }
}

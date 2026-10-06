using Cicclo.Application.Abstractions;
using Cicclo.Application.Dtos;
using Cicclo.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Cicclo.Api.Controllers;

[ApiController]
[Route("wallet")]
public sealed class WalletController(
    IQueryHandler<GetWalletQuery, WalletDto> getWalletHandler)
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
}

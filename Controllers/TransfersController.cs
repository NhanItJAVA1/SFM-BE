using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Transfers;
using SFM_BE.Services.Transfers;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFM_BE.Controllers;

[ApiController]
[Authorize]
[Route("api/transfers")]
public class TransfersController : ControllerBase
{
    private readonly ITransferService _transferService;

    public TransfersController(ITransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransfers()
    {
        return Ok(await _transferService.GetTransfersAsync(GetUserId()));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetTransfer(long id)
    {
        return Ok(await _transferService.GetTransferAsync(GetUserId(), id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransfer(CreateTransferDto dto)
    {
        var result = await _transferService.CreateAsync(GetUserId(), dto);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTransfer(long id)
    {
        await _transferService.DeleteAsync(GetUserId(), id);
        return NoContent();
    }

    private long GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User id is missing from token.");
    }
}

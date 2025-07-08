using KeyManager.API.Extensions;
using KeyManager.Application.Contracts;
using KeyManager.Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace KeyManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter<ServiceAuthorizationActionFilter>]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("reserve")]
    public async Task<ActionResult> ReserveSumAsync([FromBody] ReserveSumRequest request)
    {
        var result = await _transactionService.ReserveSumAsync(request.ApiKeiId, request.Sum);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("confirm")]
    public async Task<ActionResult> ConfirmAsync([FromBody] ConfirmTransactionRequest request)
    {
        var result = await _transactionService.ConfirmTransactionAsync(request.ApiKeiId, request.TransactionId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
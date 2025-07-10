using KeyManager.API.Utils;
using KeyManager.Application.Contracts;
using KeyManager.Application.Dtos;
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
    public async Task<ReverseSumSuccessDto> ReserveSumAsync([FromBody] ReserveSumRequest request)
    {
        return await _transactionService.ReserveSumAsync(request.ApiKeiId, request.Sum);
    }

    [HttpPost("confirm")]
    public async Task ConfirmAsync([FromBody] ConfirmTransactionRequest request)
    {
        await _transactionService.ConfirmTransactionAsync(request.ApiKeiId, request.TransactionId);
    }
}
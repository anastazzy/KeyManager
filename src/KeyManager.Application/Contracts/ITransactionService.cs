using KeyManager.Application.Dtos;

namespace KeyManager.Application.Contracts;

public interface ITransactionService
{
    Task<ResultDto> ReserveSumAsync(Guid apiKeyId, decimal amount);
    Task<ResultDto> ConfirmTransaction(Guid apiKeyId, long transactionId);
}
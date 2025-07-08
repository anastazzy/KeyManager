using KeyManager.Application.Dtos;

namespace KeyManager.Application.Contracts;

public interface ITransactionService
{
    Task<ResultDto> ReserveSumAsync(Guid apiKeyId, decimal amount);
    Task<ResultDto> ConfirmTransactionAsync(Guid apiKeyId, long transactionId);
}
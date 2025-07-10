using KeyManager.Application.Dtos;

namespace KeyManager.Application.Contracts;

public interface ITransactionService
{
    Task<ReverseSumSuccessDto> ReserveSumAsync(Guid apiKeyId, decimal amount);
    Task ConfirmTransactionAsync(Guid apiKeyId, long transactionId);
}
using KeyManager.Application.Contracts;
using KeyManager.Application.Dtos;
using KeyManager.DataAccess;
using KeysManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyManager.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly KeyManagerDbContext _dbContext;
    private readonly IUserService _userService;

    private const string ErrorLowAccountBalance = "The transaction amount is more than the account balance. Transaction is not possible";
    private const string ErrorNotFoundTransaction = "The transaction was not reserved";
    private const string ErrorExpiredTransactionTime = "The transaction confirmation time has expired";
    private const int TransactionTimeOutInMinuts = 10;

    public TransactionService(KeyManagerDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<ResultDto> ReserveSumAsync(Guid apiKeyId, decimal amount)
    {
        var transaction = new Transaction(amount, apiKeyId);
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        var user = await _userService.GetByApiKeyAsync(apiKeyId);
        if (user.Balance < amount)
            return new ResultDto(false, ErrorLowAccountBalance);

        user.Balance -= amount;
        await _dbContext.SaveChangesAsync();

        return new ResultDto();
    }

    public async Task<ResultDto> ConfirmTransactionAsync(Guid apiKeyId, long transactionId)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == transactionId);
        if (transaction is null)
            return new ResultDto(false, ErrorNotFoundTransaction);

        var current = DateTime.UtcNow;
        var isAlive = DateTime.UtcNow - transaction.CreateDateTime < TimeSpan.FromMinutes(TransactionTimeOutInMinuts);
        if (isAlive)
            return new ResultDto(false, ErrorExpiredTransactionTime);

        transaction.ConfirmedAt = current;
        await _dbContext.SaveChangesAsync();

        return new ResultDto();
    }
}
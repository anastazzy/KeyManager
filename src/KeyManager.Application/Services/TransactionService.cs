using KeyManager.Application.Contracts;
using KeyManager.Application.Dtos;
using KeyManager.Application.Utils;
using KeyManager.DataAccess;
using KeysManager.Domain.Models;
using Laraue.Core.Exceptions.Web;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace KeyManager.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly KeyManagerDbContext _dbContext;
    private readonly IUserService _userService;

    private const string ErrorLowAccountBalance = "The transaction amount is more than the account balance. Transaction is not possible";
    private const string ErrorNotFoundTransaction = "The transaction was not reserved";
    private const string ErrorExpiredTransactionTime = "The transaction confirmation time has expired";

    public TransactionService(KeyManagerDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<ReverseSumSuccessDto> ReserveSumAsync(Guid apiKeyId, decimal amount)
    {
        var user = await _userService.GetByApiKeyAsync(apiKeyId);
        if (user is null || user.Balance < amount)
        {
            Log.Warning("Refused to create a transaction with low balance for user {@user}, amount {amount}", user, amount);
            throw new BadRequestException(nameof(apiKeyId), ErrorLowAccountBalance);
        }

        var transaction = new Transaction(amount, apiKeyId);
        await _dbContext.Transactions.AddAsync(transaction);

        user.Balance -= amount;
        await _dbContext.SaveChangesAsync();

        Log.Information("Created transaction {@transaction} for user {@user}", transaction, user);
        return new ReverseSumSuccessDto(transaction.Id);
    }

    public async Task ConfirmTransactionAsync(Guid apiKeyId, long transactionId)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == transactionId);
        if (transaction is null)
        {
            Log.Error("Requested transaction not found {transactionId}", transactionId);
            throw new BadRequestException(nameof(transactionId), ErrorNotFoundTransaction);
        }

        var current = DateTime.UtcNow;
        var isAlive = DateTime.UtcNow - transaction.CreateDateTime < TimeSpan.FromMinutes(CommonConstants.TransactionTimeOutInMinutes);
        if (!isAlive)
        {
            Log.Warning("Requested transaction is expired {@transaction}", transaction);
            throw new BadRequestException(nameof(transactionId), ErrorExpiredTransactionTime);
        }

        transaction.ConfirmedAt = current;
        await _dbContext.SaveChangesAsync();

        Log.Information("Transaction is confirmed {@transaction}", transaction);
    }
}
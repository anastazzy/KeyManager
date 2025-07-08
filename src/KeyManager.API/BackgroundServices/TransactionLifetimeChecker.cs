using System.Text;
using KeyManager.Application.Utils;
using KeyManager.DataAccess;
using KeysManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace KeyManager.API.BackgroundServices;

public class TransactionLifetimeChecker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceProviderFactory;

    public TransactionLifetimeChecker(IServiceScopeFactory serviceProviderFactory)
    {
        _serviceProviderFactory = serviceProviderFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _serviceProviderFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<KeyManagerDbContext>();

            var now = DateTime.UtcNow;
            var forCheck = dbContext.Transactions
                .Where(x => x.ConfirmedAt == null && x.Amount != 0)
                .ToList();

            if (forCheck.Count == 0)
            {
                Log.Information("Background TLC: not found transactions for check");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                continue;
            }

            var forReturnAmount = forCheck
                .Where(x => now - x.CreateDateTime > TimeSpan.FromMinutes(CommonConstants.TransactionTimeOutInMinutes))
                .ToArray();

            if (forReturnAmount.Length > 0)
            {
                await CancelTransactionAsync(forReturnAmount, dbContext, stoppingToken);
            }
            else
            {
                Log.Information("Background TLC: found transaction for check {transactionsForCheck}, but not found toReturnAmount", string.Join(", ", forCheck.Select(x => x.Id)));
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task CancelTransactionAsync(Transaction[] cancelledTransactions, KeyManagerDbContext dbContext, CancellationToken stoppingToken)
    {
        var builder = new StringBuilder("Background TLC: found transaction for return amount for:\n");

        foreach (var item in cancelledTransactions)
        {
            var apiKey = await dbContext.ApiKeys
                .Where(x => x.Id == item.ApiKeyId)
                .Include(x => x.User)
                .FirstAsync(stoppingToken);

            if (apiKey.User == null) continue;

            apiKey.User.Balance += item.Amount;
            item.Amount = 0;
            builder.AppendJoin(", ", $"transactionId:{item.Id}_apiKeyId:{item.ApiKeyId}_userId:{apiKey.User.Id}_amount:{item.Amount}_newUserBalance:{apiKey.User.Balance}");
        }

        await dbContext.SaveChangesAsync(stoppingToken);
        Log.Information(builder.ToString());
    }
}
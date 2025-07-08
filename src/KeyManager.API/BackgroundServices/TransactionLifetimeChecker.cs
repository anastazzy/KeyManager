using KeyManager.DataAccess;
using Microsoft.EntityFrameworkCore;

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
            var transactionsForCheck = dbContext.Transactions
                .Where(x => x.ConfirmedAt == null && x.Amount != 0)
                .ToList();

            if (transactionsForCheck.Count == 0) 
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            var toReturnAmount = transactionsForCheck
                .Where(x => now - x.CreateDateTime > TimeSpan.FromMinutes(10)).ToArray();

            if (toReturnAmount.Length > 0)
            {
                foreach (var item in toReturnAmount)
                {
                    var apiKey = await dbContext.ApiKeys
                        .Where(x => x.Id == item.ApiKeyIdId)
                        .Include(x => x.User)
                        .FirstAsync(stoppingToken);

                    if (apiKey.User != null) apiKey.User.Balance += item.Amount;
                    item.Amount = 0;
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
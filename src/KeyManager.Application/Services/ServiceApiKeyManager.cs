using KeyManager.Application.Contracts;
using KeyManager.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace KeyManager.Application.Services;

public class ServiceApiKeyManager : IServiceApiKeyManager
{
    private readonly KeyManagerDbContext _dbContext;

    public ServiceApiKeyManager(KeyManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsRegisteredAsync(Guid apiKey)
    {
        var service = await _dbContext.ServiceApiKeys.FirstOrDefaultAsync(x => x.ApiKey == apiKey);
        return service is not null;
    }
}
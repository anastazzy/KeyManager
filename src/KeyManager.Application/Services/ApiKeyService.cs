using KeyManager.Application.Contracts;
using KeyManager.Application.Dtos;
using KeyManager.DataAccess;
using KeysManager.Domain.Models;
using Laraue.Core.Exceptions.Web;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace KeyManager.Application.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly KeyManagerDbContext _dbContext;
    private const string ApiKeyNotExists = "Api key not exists in system";

    public ApiKeyService(KeyManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Array> GetUserKeysAsync(Guid userId)
    {
        var result = await _dbContext.ApiKeys
            .Where(x => x.UserId == userId)
            .Select(x => new KeyDto(x.Id, x.Name))
            .ToArrayAsync();

        return result;
    }

    public async Task AddKeyAsync(Guid userId, string name)
    {
        var model = new ApiKey
        {
            Name = name,
            UserId = userId
        };

        await _dbContext.AddAsync(model);
        await _dbContext.SaveChangesAsync();

        Log.Information("Added new ApiKey {@model} for userId {userId}", model, userId);
    }

    public async Task RemoveKeyAsync(Guid id)
    {
        var toDelete = await _dbContext.ApiKeys.FirstOrDefaultAsync(x => x.Id == id);
        if (toDelete is null) return;

        _dbContext.ApiKeys.Remove(toDelete);
        await _dbContext.SaveChangesAsync();
        Log.Information("Removed ApiKey {@model}", toDelete);
    }

    public async Task UpdateNameAsync(Guid id, string name)
    {
        var toUpdate = await _dbContext.ApiKeys.FirstOrDefaultAsync(x => x.Id == id);
        if (toUpdate is null)
            throw new BadRequestException(nameof(id), ApiKeyNotExists);

        toUpdate.Name = name;
        _dbContext.Update(toUpdate);
        await _dbContext.SaveChangesAsync();

        Log.Information("Renamed ApiKey {@model}", toUpdate);
    }
}
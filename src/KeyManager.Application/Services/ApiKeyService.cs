using KeyManager.Application.Contracts;
using KeyManager.Application.Dtos;
using KeyManager.DataAccess;
using KeysManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyManager.Application.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly KeyManagerDbContext _dbContext;
    private const string ApiKeyNotExists = "Api key not exists in system";

    public ApiKeyService(KeyManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<KeyDto>> GetUserKeysAsync(Guid userId)
    {
        var result = await _dbContext.ApiKeys
            .Where(x => x.UserId == userId)
            .Select(x => new KeyDto(x.Id, x.Name))
            .ToListAsync();

        return result;
    }

    public async Task<ResultDto> AddKeyAsync(Guid userId, string name)
    {
        var model = new ApiKey
        {
            Name = name,
            UserId = userId
        };

        await _dbContext.AddAsync(model);
        await _dbContext.SaveChangesAsync();

        return new ResultDto(model.Id == Guid.Empty);
    }

    public async Task RemoveKeyAsync(Guid id)
    {
        var toDelete = await _dbContext.ApiKeys.FirstOrDefaultAsync(x => x.Id == id);
        if (toDelete is null) return;

        _dbContext.ApiKeys.Remove(toDelete);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ResultDto> UpdateNameAsync(Guid id, string name)
    {
        var toUpdate = await _dbContext.ApiKeys.FirstOrDefaultAsync(x => x.Id == id);
        if (toUpdate is null)
            return new ResultDto(false, ApiKeyNotExists);

        toUpdate.Name = name;
        _dbContext.Update(toUpdate);
        await _dbContext.SaveChangesAsync();
        
        return new ResultDto();
    }
}
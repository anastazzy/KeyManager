using KeyManager.Application.Dtos;

namespace KeyManager.Application.Contracts;

public interface IApiKeyService
{
    Task<List<KeyDto>> GetUserKeysAsync(Guid userId);
    Task AddKeyAsync(Guid userId, string name);
    Task RemoveKeyAsync(Guid id);
    Task UpdateNameAsync(Guid id, string name);
}
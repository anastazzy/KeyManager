namespace KeyManager.Application.Contracts;

public interface IApiKeyService
{
    Task<Array> GetUserKeysAsync(Guid userId);
    Task AddKeyAsync(Guid userId, string name);
    Task RemoveKeyAsync(Guid id);
    Task UpdateNameAsync(Guid id, string name);
}
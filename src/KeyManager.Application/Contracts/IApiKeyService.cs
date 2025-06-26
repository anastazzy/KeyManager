using KeyManager.Application.Dtos;

namespace KeyManager.Application.Contracts;

public interface IApiKeyService
{
    Task<List<KeyDto>> GetUserKeysAsync(Guid userId);
    Task<ResultDto> AddKeyAsync(Guid userId, string name);
    Task RemoveKeyAsync(Guid id);
    Task<ResultDto> UpdateNameAsync(Guid id, string name);
}
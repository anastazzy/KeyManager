using KeyManager.Application.Requests;
using KeysManager.Domain.Models;

namespace KeyManager.Application.Contracts;

public interface IUserService
{
    Task RegisterAsync(LoginUserRequest request, string link);
    Task<string> LoginAsync(LoginUserRequest request);
    Task<string> ConfirmEmailAsync(string token);
    Task<User?> GetByApiKeyAsync(Guid apiKeyGuid);
}
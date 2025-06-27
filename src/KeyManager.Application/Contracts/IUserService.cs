using KeyManager.Application.Dtos;
using KeyManager.Application.Requests;
using KeysManager.Domain.Models;

namespace KeyManager.Application.Contracts;

public interface IUserService
{
    Task<ResultDto> RegisterAsync(LoginUserRequest request, string link);
    Task<LoginResultDto> LoginAsync(LoginUserRequest request);
    Task<ResultDto> ConfirmEmailAsync(string token);

    Task<User> GetByApiKeyAsync(Guid apiKeyGuid);
}
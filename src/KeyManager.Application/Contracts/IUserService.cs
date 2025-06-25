using KeyManager.Application.Dtos;
using KeyManager.Application.Requests;

namespace KeyManager.Application.Contracts;

public interface IUserService
{
    Task<ResultDto> RegisterAsync(LoginUserRequest request, string link);
    Task<string> LoginAsync(LoginUserRequest request);
    Task<bool> ConfirmEmailAsync(string token);
}
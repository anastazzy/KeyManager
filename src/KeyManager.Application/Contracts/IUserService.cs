using KeyManager.Application.Dtos;
using KeyManager.Application.Requests;

namespace KeyManager.Application.Contracts;

public interface IUserService
{
    Task<ResultDto> RegisterAsync(LoginUserRequest request, string link);
    Task<LoginResultDto> LoginAsync(LoginUserRequest request);
    Task<ResultDto> ConfirmEmailAsync(string token);
}
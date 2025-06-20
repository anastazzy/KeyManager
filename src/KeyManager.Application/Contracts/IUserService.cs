using KeyManager.Application.Requests;

namespace KeyManager.Application.Contracts;

public interface IUserService
{
    Task<Guid> RegisterAsync(LoginUserRequest request, string link);
    Task<string> LoginAsync(LoginUserRequest request);
    Task<bool> ConfirmEmailAsync(string token);
}
using KeyManager.Application.Requests;

namespace KeyManager.Application.Contracts;

public interface IUserService
{
    Task<Guid> RegisterAsync(LoginUserRequest request);
    Task<string> LoginAsync(LoginUserRequest request);
}
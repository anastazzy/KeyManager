using KeyManager.Application.Contracts;
using KeyManager.Application.Requests;
using KeyManager.DataAccess;
using KeyManager.Infrastructure.Contracts;
using KeysManager.Domain.Models;

namespace KeyManager.Application.Services;

public class UserService : IUserService
{
    private readonly KeyManagerDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(KeyManagerDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<Guid> RegisterAsync(LoginUserRequest request)
    {
        var hash = _passwordHasher.GetHash(request.Password);
        var user = new User(request.Email, hash);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user.Id;
    }

    public Task<string> LoginAsync(LoginUserRequest request)
    {
        throw new NotImplementedException();
    }
}
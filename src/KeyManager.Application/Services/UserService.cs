using KeyManager.Application.Contracts;
using KeyManager.Application.Requests;
using KeyManager.DataAccess;
using KeyManager.Infrastructure.Contracts;
using KeyManager.MailService;
using KeysManager.Domain.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace KeyManager.Application.Services;

public class UserService : IUserService
{
    private readonly KeyManagerDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IEmailService _emailService;

    public UserService(KeyManagerDbContext dbContext, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IEmailService emailService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _emailService = emailService;
    }

    public async Task<Guid> RegisterAsync(LoginUserRequest request)
    {
        var hash = _passwordHasher.GetHash(request.Password);
        var user = new User(request.Email, hash);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        await _emailService.SendEmailAsync(user.Email, "info", "yesssss, you registered!");
        return user.Id;
    }

    public async Task<string> LoginAsync(LoginUserRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null) return string.Empty;

        var isPasswordMatches = _passwordHasher.Verify(request.Password, user.Password);
        if (!isPasswordMatches) return string.Empty;

        var token = _jwtProvider.GenerateAccessJwtToken(user.Id, user.Email);
        return token;
    }
}
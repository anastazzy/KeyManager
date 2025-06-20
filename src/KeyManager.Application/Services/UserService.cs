using System.Text;
using KeyManager.Application.Contracts;
using KeyManager.Application.Requests;
using KeyManager.DataAccess;
using KeyManager.Infrastructure.Contracts;
using KeyManager.MailService;
using KeysManager.Domain.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace KeyManager.Application.Services;

public class UserService : IUserService
{
    private readonly KeyManagerDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IEmailService _emailService;
    private const string MessageSubject = "Confirmetion message";
    private const string MessageText = "For confirmed your email for complete of registration, follow link: \n<a href=\"{0}\" title=\"click here\">{0}</a>";
    private const string DateTimeFormat = "MM/dd/yyyy HH:mm:ss";

    public UserService(KeyManagerDbContext dbContext, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IEmailService emailService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _emailService = emailService;
    }

    public async Task<Guid> RegisterAsync(LoginUserRequest request, string link)
    {
        var hash = _passwordHasher.GetHash(request.Password);
        var confirmationCode = Convert.ToBase64String(Encoding.ASCII
            .GetBytes(DateTime.UtcNow.ToString(DateTimeFormat))
            .Concat(Guid.NewGuid().ToByteArray())
            .ToArray());

        var user = new User(request.Email, hash, confirmationCode);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var confirmationLink = QueryHelpers.AddQueryString(link, new Dictionary<string, string?> { { "token", user.ConfirmationCode } });
        await _emailService.SendEmailAsync(user.Email, MessageSubject, string.Format(MessageText, confirmationLink));
        return user.Id;
    }

    public async Task<string> LoginAsync(LoginUserRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null || !user.IsEmailConfirmed) return string.Empty;

        var isPasswordMatches = _passwordHasher.Verify(request.Password, user.Password);
        if (!isPasswordMatches) return string.Empty;

        var token = _jwtProvider.GenerateAccessJwtToken(user.Id, user.Email);
        return token;
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.ConfirmationCode == token);
        if (user is null)
            return false;

        user.IsEmailConfirmed = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
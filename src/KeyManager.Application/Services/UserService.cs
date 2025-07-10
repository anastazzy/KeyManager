using System.Text;
using KeyManager.Application.Contracts;
using KeyManager.Application.Requests;
using KeyManager.DataAccess;
using KeyManager.Infrastructure.Contracts;
using KeyManager.MailService;
using KeysManager.Domain.Models;
using Laraue.Core.Exceptions.Web;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
    private const string ErrorUserAlreadyExists = "User with such email already exists";
    private const string ErrorWhenMailWasNotSend = "Аn error occurred while sending the message";

    private const string ErrorThenWrongToken = "Error of confirmation, wrong tokem";
    private const string ErrorThenEmailNotConfirmed = "Email not confirmed";
    private const string ErrorThenWrongPassword = "Wrong passsword";

    private const string EmailSuccessfulConfirmed = "Email successful confirmed";

    public UserService(KeyManagerDbContext dbContext, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IEmailService emailService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _emailService = emailService;
    }

    public async Task RegisterAsync(LoginUserRequest request, string link)
    {
        if (await _dbContext.Users.AnyAsync(x => x.Email == request.Email))
            throw new BadRequestException(nameof(request.Email), ErrorUserAlreadyExists);

        var hash = _passwordHasher.GetHash(request.Password);
        var confirmationCode = Convert.ToBase64String(Encoding.ASCII
            .GetBytes(DateTime.UtcNow.ToString(DateTimeFormat))
            .Concat(Guid.NewGuid().ToByteArray())
            .ToArray());

        var user = new User(request.Email, hash, confirmationCode);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        Log.Information("Created user {@user}", user);

        var confirmationLink = QueryHelpers.AddQueryString(link, new Dictionary<string, string?> { { "token", user.ConfirmationCode } });
        try
        {
            await _emailService.SendEmailAsync(user.Email, MessageSubject, string.Format(MessageText, confirmationLink));

            Log.Information("Delivered email for confirmation to user {@user}", user);
        }
        catch (Exception e)
        {
            Log.Error("Error when delivered email for confirmation to user {@user} {@e}", user, e);
            throw new BadRequestException(string.Empty, ErrorWhenMailWasNotSend);
        }
    }

    public async Task<string> LoginAsync(LoginUserRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null)
            throw new BadRequestException(nameof(request.Email), ErrorThenWrongToken);

        if (!user.IsEmailConfirmed)
        {
            Log.Warning("User try to login. The email is not confirmed for user {@user}", user.Id);
            throw new BadRequestException(nameof(request.Email), ErrorThenEmailNotConfirmed);
        }

        var isPasswordMatches = _passwordHasher.Verify(request.Password, user.Password);
        if (!isPasswordMatches)
        {
            Log.Warning("Passwords don`t match for user {user}", user.Id);
            throw new BadRequestException(nameof(request.Password), ErrorThenWrongPassword);
        }

        Log.Information("Login in system {user}", user.Id);
        return _jwtProvider.GenerateAccessJwtToken(user.Id, user.Email);
    }

    public async Task<string> ConfirmEmailAsync(string token)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.ConfirmationCode == token);
        if (user is null)
            throw new BadRequestException(nameof(token), ErrorThenWrongToken);

        user.IsEmailConfirmed = true;
        await _dbContext.SaveChangesAsync();

        Log.Information("Success confirmed email for user {user}", user.Id);
        return EmailSuccessfulConfirmed;
    }

    public async Task<User?> GetByApiKeyAsync(Guid apiKeyGuid)
    {
        var key = await _dbContext.ApiKeys
            .Where(x => x.Id == apiKeyGuid)
            .Include(x => x.User)
            .FirstAsync();

        return key.User;
    }
}
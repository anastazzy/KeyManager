using System.Text;
using KeyManager.Application.Contracts;
using KeyManager.Application.Dtos;
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
    private const string ErrorUserAlreadyExists = "User with such email already exists";
    private const string ErrorWhenMailWasNotSend = "Аn error occurred while sending the message";
    private const string SuccessSendMessage = "An email has been sent to the specified email address. Confirm your login by clicking on the link from the email.";

    private const string ErrorThenEmailNotExists = "Email not registered in system";
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

    public async Task<ResultDto> RegisterAsync(LoginUserRequest request, string link)
    {
        if (await _dbContext.Users.AnyAsync(x => x.Email == request.Email))
            return new ResultDto(false, ErrorUserAlreadyExists);

        var hash = _passwordHasher.GetHash(request.Password);
        var confirmationCode = Convert.ToBase64String(Encoding.ASCII
            .GetBytes(DateTime.UtcNow.ToString(DateTimeFormat))
            .Concat(Guid.NewGuid().ToByteArray())
            .ToArray());

        var user = new User(request.Email, hash, confirmationCode);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var result = new ResultDto();
        var confirmationLink = QueryHelpers.AddQueryString(link, new Dictionary<string, string?> { { "token", user.ConfirmationCode } });
        try
        {
            await _emailService.SendEmailAsync(user.Email, MessageSubject, string.Format(MessageText, confirmationLink));
            result.Message = SuccessSendMessage;
        }
        catch (Exception e)
        {
            result.IsSuccess = false;
            result.Message = ErrorWhenMailWasNotSend;
            Console.WriteLine(e);
            throw;
        }

        return result;
    }

    public async Task<LoginResultDto> LoginAsync(LoginUserRequest request)
    {
        var result = new LoginResultDto();
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null)
        {
            result.IsSuccess = false;
            result.Message = ErrorThenEmailNotExists;
            return result;
        }

        if (!user.IsEmailConfirmed)
        {
            result.IsSuccess = false;
            result.Message = ErrorThenEmailNotConfirmed;
            return result;
        }

        var isPasswordMatches = _passwordHasher.Verify(request.Password, user.Password);
        if (!isPasswordMatches)
        {
            result.IsSuccess = false;
            result.Message = ErrorThenWrongPassword;
            return result;
        }

        result.Token = _jwtProvider.GenerateAccessJwtToken(user.Id, user.Email);
        return result;
    }

    public async Task<ResultDto> ConfirmEmailAsync(string token)
    {
        var result = new ResultDto();
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.ConfirmationCode == token);
        if (user is null)
        {
            result.IsSuccess = false;
            result.Message = ErrorThenEmailNotExists;
            return result;
        }

        user.IsEmailConfirmed = true;
        await _dbContext.SaveChangesAsync();
        result.Message = EmailSuccessfulConfirmed;
        return result;
    }
}
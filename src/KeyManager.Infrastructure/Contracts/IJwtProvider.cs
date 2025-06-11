namespace KeyManager.Infrastructure.Contracts;

public interface IJwtProvider
{
    string GenerateAccessJwtToken(string userId, string username);
}
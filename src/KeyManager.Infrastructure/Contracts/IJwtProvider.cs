namespace KeyManager.Infrastructure.Contracts;

public interface IJwtProvider
{
    string GenerateAccessJwtToken(Guid userId, string email);
}
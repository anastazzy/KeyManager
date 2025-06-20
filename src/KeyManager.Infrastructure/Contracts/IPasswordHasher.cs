namespace KeyManager.Infrastructure.Contracts;

public interface IPasswordHasher
{
    string GetHash(string password);
    bool Verify(string password, string hash);
}
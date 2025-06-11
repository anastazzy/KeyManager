using KeyManager.Infrastructure.Contracts;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace KeyManager.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int IterationCount = 1000;

    /// <summary>
    ///     Требуемая длина (в байтах) производного ключа.
    ///     https://learn.microsoft.com/ru-ru/dotnet/api/microsoft.aspnetcore.cryptography.keyderivation.keyderivation.pbkdf2?view=aspnetcore-9.0
    /// </summary>
    private const int KeyLenght = 256 / 8;

    public string GetHash(string password) => GenerateHash(password);

    public bool Verify(string password, string hash)
    {
        var enteredHash = GenerateHash(password);
        return enteredHash == hash;
    }

    private string GenerateHash(string password) =>
        Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            [],
            KeyDerivationPrf.HMACSHA256,
            IterationCount,
            KeyLenght));
}
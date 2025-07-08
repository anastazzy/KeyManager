namespace KeyManager.Application.Contracts;

public interface IServiceApiKeyManager
{
    Task<bool> IsRegisteredAsync(Guid apiKey);
}
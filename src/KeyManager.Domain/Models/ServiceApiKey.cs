namespace KeysManager.Domain.Models;

public class ServiceApiKey
{
    public Guid Id { get; set; }

    public Guid ApiKey { get; set; }

    public required string ServiceName { get; set; }
}
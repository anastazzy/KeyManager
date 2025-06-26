namespace KeysManager.Domain.Models;

public class ApiKey
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public Guid UserId { get; set; }

    public User? User { get; set; }
}
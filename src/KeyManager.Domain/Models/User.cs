namespace KeysManager.Domain.Models;

public class User
{
    public Guid Id { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public string? ConfirmationCode { get; set; }

    public decimal Balance { get; set; }
}
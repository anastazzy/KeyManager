namespace KeysManager.Domain.Models;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public string? ConfirmationCode { get; set; }

    public decimal Balance { get; set; }

    public User()
    {
    }

    public User(string email, string password, string confirmationCode)
    {
        Email = email;
        Password = password;
        ConfirmationCode = confirmationCode;
    }
}
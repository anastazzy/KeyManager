namespace KeysManager.Domain.Models;

public class Transaction
{
    public long Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime? ConfirmedAt { get; set; }
    public DateTime CreateDateTime { get; set; } = DateTime.UtcNow;

    public Guid ApiKeyId { get; set; }
    public ApiKey? ApiKey { get; set; }

    public Transaction()
    {
    }

    public Transaction(decimal amount, Guid apiKeyId)
    {
        Amount = amount;
        ApiKeyId = apiKeyId;
    }
}
namespace KeysManager.Domain.Models;

public class Transaction
{
    public long Id { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime? ConfirmedAt { get; set; }
}
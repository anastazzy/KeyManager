namespace KeyManager.Application.Dtos;

public class ReverseSumSuccessDto(long id)
{
    public long TransactionId { get; set; } = id;
}
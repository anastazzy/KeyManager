namespace KeyManager.Application.Dtos;

public class CreateTransactionResultDto(long id) : ResultDto
{
    public long Id { get; set; } = id;
}
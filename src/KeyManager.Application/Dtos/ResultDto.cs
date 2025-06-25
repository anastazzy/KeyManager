namespace KeyManager.Application.Dtos;

public class ResultDto(bool isSuccess = true, string? message = null)
{
    public bool IsSuccess { get; set; } = isSuccess;
    public string? Message { get; set; } = message;
}
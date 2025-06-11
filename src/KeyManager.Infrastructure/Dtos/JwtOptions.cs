namespace KeyManager.Infrastructure.Dtos;

public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int HourLifeTime { get; set; } = 24;
}
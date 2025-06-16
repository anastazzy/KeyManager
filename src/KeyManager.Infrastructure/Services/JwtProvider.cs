using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KeyManager.Infrastructure.Contracts;
using KeyManager.Infrastructure.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KeyManager.Infrastructure.Services;

public class JwtProvider(IOptions<JwtOptions> settings) : IJwtProvider
{
    private const string SigningAlgorithm = SecurityAlgorithms.HmacSha256;
    private readonly JwtOptions _options = settings.Value;

    public string GenerateAccessJwtToken(Guid userId, string email)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.SecretKey));
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Sid, userId.ToString()),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_options.HourLifeTime),
            signingCredentials: new SigningCredentials(securityKey, SigningAlgorithm)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
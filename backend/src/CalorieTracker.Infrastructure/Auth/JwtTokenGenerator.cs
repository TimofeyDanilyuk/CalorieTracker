using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CalorieTracker.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CalorieTracker.Infrastructure.Auth;

public class JwtTokenGenerator
{
    private readonly string _secret;
    private readonly string _issuer;

    public JwtTokenGenerator(string secret, string issuer)
    {
        _secret = secret;
        _issuer = issuer;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30), // долгий срок - это личное приложение на 2 человек
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Supermercado.Services.DTOs;
using Supermercado.Services.Interfaces;

namespace Supermercado.Services.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenResponseDTO? Autenticar(LoginDTO dto)
    {
        // Validação mock para testes (em produção, validar via banco/hash de senha)
        if (dto.Usuario != "Admin" || dto.Senha != "123456")
            return null;

        var secretKey = _configuration["Jwt:SecretKey"] ?? "SuaChaveSecretaSuperSeguraParaJWT1234567890!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiracao = DateTime.UtcNow.AddHours(2);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, dto.Usuario),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "SupermercadoAPI",
            audience: _configuration["Jwt:Audience"] ?? "SupermercadoClientes",
            claims: claims,
            expires: expiracao,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResponseDTO(tokenString, expiracao);
    }
}
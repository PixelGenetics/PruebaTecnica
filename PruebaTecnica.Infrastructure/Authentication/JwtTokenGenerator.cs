using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(
        Usuario usuario,
        DateTime expiration)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "No se encontró la configuración Jwt:Key.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "No se encontró la configuración Jwt:Issuer.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "No se encontró la configuración Jwt:Audience.");

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                usuario.Id.ToString()),

            new(
                JwtRegisteredClaimNames.UniqueName,
                usuario.NombreUsuario),

            new(
                JwtRegisteredClaimNames.Email,
                usuario.Correo),

            new(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),

            new(
                ClaimTypes.Name,
                usuario.Nombre)
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
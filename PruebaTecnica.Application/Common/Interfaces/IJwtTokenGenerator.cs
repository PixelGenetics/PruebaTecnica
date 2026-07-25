using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        Usuario usuario,
        DateTime expiration);
}
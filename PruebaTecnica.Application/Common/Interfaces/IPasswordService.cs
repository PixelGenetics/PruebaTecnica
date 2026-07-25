using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Common.Interfaces;

public interface IPasswordService
{
    string HashPassword(
        Usuario usuario,
        string password);

    bool VerifyPassword(
        Usuario usuario,
        string password,
        string passwordHash);
}
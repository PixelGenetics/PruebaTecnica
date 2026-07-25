using Microsoft.AspNetCore.Identity;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Infrastructure.Authentication;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public string HashPassword(
        Usuario usuario,
        string password)
    {
        return _passwordHasher.HashPassword(
            usuario,
            password);
    }

    public bool VerifyPassword(
    Usuario usuario,
    string password,
    string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        try
        {
            var result = _passwordHasher.VerifyHashedPassword(
                usuario,
                passwordHash,
                password);

            return result is
                PasswordVerificationResult.Success or
                PasswordVerificationResult.SuccessRehashNeeded;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
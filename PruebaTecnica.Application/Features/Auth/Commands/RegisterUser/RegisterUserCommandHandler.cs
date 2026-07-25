using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordService _passwordService;

    public RegisterUserCommandHandler(
        IAppDbContext context,
        IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var nombreUsuario = request.NombreUsuario.Trim();
        var correo = request.Correo.Trim();

        var userExists = await _context.Usuario
            .AnyAsync(
                usuario =>
                    usuario.NombreUsuario == nombreUsuario ||
                    usuario.Correo == correo,
                cancellationToken);

        if (userExists)
        {
            return new RegisterUserResult
            {
                Success = false,
                Message = "El nombre de usuario o correo ya está registrado."
            };
        }

        var usuario = new Usuario
        {
            NombreUsuario = nombreUsuario,
            Nombre = request.Nombre.Trim(),
            Correo = correo,
            Estado = true
        };

        usuario.Contrasenia = _passwordService.HashPassword(
            usuario,
            request.Contrasenia);

        await _context.Usuario.AddAsync(
            usuario,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterUserResult
        {
            Success = true,
            Message = "Usuario registrado correctamente.",
            UsuarioId = usuario.Id
        };
    }
}
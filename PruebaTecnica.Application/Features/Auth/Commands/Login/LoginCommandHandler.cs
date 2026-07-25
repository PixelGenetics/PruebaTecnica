using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Auth.DTOs;

namespace PruebaTecnica.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        IAppDbContext context,
        IPasswordService passwordService,
        IJwtTokenGenerator jwtTokenGenerator,
        IConfiguration configuration)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _configuration = configuration;
    }

    public async Task<LoginResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var nombreUsuario = request.NombreUsuario.Trim();

        var usuario = await _context.Usuario
            .AsNoTracking()
            .FirstOrDefaultAsync(
                usuario => usuario.NombreUsuario == nombreUsuario,
                cancellationToken);

        if (usuario is null)
        {
            return new LoginResult
            {
                Success = false,
                Message = "Usuario o contraseña incorrectos."
            };
        }

        if (!usuario.Estado)
        {
            return new LoginResult
            {
                Success = false,
                Message = "El usuario se encuentra desactivado."
            };
        }

        var passwordIsValid = _passwordService.VerifyPassword(
            usuario,
            request.Contrasenia,
            usuario.Contrasenia);

        if (!passwordIsValid)
        {
            return new LoginResult
            {
                Success = false,
                Message = "Usuario o contraseña incorrectos."
            };
        }

        var expirationMinutes =
            _configuration.GetValue<int?>("Jwt:ExpirationMinutes") ?? 60;

        var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var token = _jwtTokenGenerator.GenerateToken(
            usuario,
            expiration);

        return new LoginResult
        {
            Success = true,
            Message = "Inicio de sesión exitoso.",
            Data = new LoginResponseDto
            {
                UsuarioId = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Token = token,
                Expiracion = expiration
            }
        };
    }
}
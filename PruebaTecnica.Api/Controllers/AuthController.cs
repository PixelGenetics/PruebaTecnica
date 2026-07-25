using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Application.Features.Auth.Commands.Login;
using PruebaTecnica.Application.Features.Auth.Commands.RegisterUser;
using PruebaTecnica.Application.Features.Auth.DTOs;

namespace PruebaTecnica.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RegisterUserCommand(
                dto.NombreUsuario,
                dto.Contrasenia,
                dto.Nombre,
                dto.Correo),
            cancellationToken);

        if (!result.Success)
        {
            return Conflict(new
            {
                message = result.Message
            });
        }

        return Created(string.Empty, new
        {
            message = result.Message,
            usuarioId = result.UsuarioId
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new LoginCommand(
                dto.NombreUsuario,
                dto.Contrasenia),
            cancellationToken);

        if (!result.Success)
        {
            return Unauthorized(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message,
            usuario = result.Data
        });
    }
}
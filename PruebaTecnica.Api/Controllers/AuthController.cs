using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Application.Features.Auth.Commands.Login;
using PruebaTecnica.Application.Features.Auth.Commands.RegisterUser;
using PruebaTecnica.Application.Features.Auth.DTOs;
using PruebaTecnica.Application.Features.Categories.DTOs;

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
    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <remarks>
    /// Crea una cuenta de usuario utilizando el nombre de usuario, contraseña,
    /// nombre completo y correo electrónico proporcionados.
    ///
    /// 🔐 Este endpoint no requiere autenticación.
    ///
    /// ❌ Si el nombre de usuario o el correo electrónico ya están registrados,
    /// devuelve una respuesta HTTP 409.
    /// </remarks>
    /// <param name="dto">
    /// Datos necesarios para registrar al nuevo usuario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 201 con el mensaje de confirmación y el identificador
    /// del usuario creado; HTTP 400 si los datos enviados son inválidos; o
    /// HTTP 409 si ocurre un conflicto durante el registro.
    /// </returns>
    /// <response code="201">
    /// El usuario fue registrado correctamente.
    /// </response>
    /// <response code="400">
    /// Los datos enviados no cumplen las reglas de validación.
    /// </response>
    /// <response code="409">
    /// El nombre de usuario o el correo electrónico ya están registrados,
    /// o existe otro conflicto que impide completar el registro.
    /// </response>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponseDto),201)]
    [ProducesResponseType(typeof(MessageCategoryDto), 400)]
    [ProducesResponseType(typeof(MessageCategoryDto), 409)]
    public async Task<IActionResult> Register(
        RegisterUserDto dto)
    {
        var result = await _sender.Send(
            new RegisterUserCommand(
                dto.NombreUsuario,
                dto.Contrasenia,
                dto.Nombre,
                dto.Correo));

        if (!result.Success)
        {
            return Conflict(new
            {
                message = result.Message
            });
        }

        return Created(string.Empty, new RegisterUserResponseDto
        {
            Message = result.Message,
            UsuarioId = result.UsuarioId
        });
    }
    /// <summary>
    /// Autentica a un usuario en el sistema.
    /// </summary>
    /// <remarks>
    /// Valida el nombre de usuario y la contraseña proporcionados.
    ///
    /// 🔐 Este endpoint no requiere autenticación.
    ///
    /// ❌ Si las credenciales son válidas, devuelve la información del usuario
    /// autenticado.
    /// 
    /// ❌ Si son incorrectas, devuelve una respuesta HTTP 401.
    /// </remarks>
    /// <param name="dto">
    /// Credenciales necesarias para autenticar al usuario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con el mensaje de confirmación y los datos del usuario;
    /// HTTP 400 si los datos enviados son inválidos; o HTTP 401 si las credenciales
    /// no son correctas.
    /// </returns>
    /// <response code="201">
    /// El usuario fue autenticado correctamente.
    /// </response>
    /// <response code="400">
    /// Los datos enviados no cumplen las reglas de validación.
    /// </response>
    /// <response code="401">
    /// El nombre de usuario o la contraseña son incorrectos.
    /// </response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseSuccessfulDto), 201)]
    [ProducesResponseType(typeof(MessageCategoryDto), 400)]
    [ProducesResponseType(typeof(MessageCategoryDto), 401)]
    public async Task<IActionResult> Login(
        LoginDto dto)
    {
        var result = await _sender.Send(
            new LoginCommand(
                dto.NombreUsuario,
                dto.Contrasenia));

        if (!result.Success)
        {
            return Unauthorized(new
            {
                message = result.Message
            });
        }

        return Ok(new LoginResponseSuccessfulDto
        {
            Message = result.Message,
            Usuario = result.Data
        });
    }
}